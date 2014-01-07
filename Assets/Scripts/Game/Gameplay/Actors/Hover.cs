using UnityEngine;
using System;

public class Hover : MonoBehaviour
{
        [Serializable]
        public class HoverProperties
        {
                public float hoverHeight;
                public float hoverDamping;
        }
        public HoverProperties hoverProperties;
	
		public GameObject boardObj;

        public float thrusterScale = 1.0f;

        public float thrustPower;
        public float steerPower;
        public float angleClamp = 50.0f;
        public float airAngleClamp = 10.0f;
        private float currentClamp;

        [HideInInspector] Transform[] m_sensors;
        RaycastHit[] m_hits;
        Transform m_thruster;
        float m_thrust;
        float m_lean;
        float m_glide;
        private bool m_jump;
        private bool detach;

        private float jumpPower; // measures how long jump was held, 0 - 1
        public float jumpLength = 3.0f; //how many seconds to build maximum jump
        public float jumpForce = 20.0f; //The max force applied when jumping
        public float glideForce = 400.0f;

        public float glideLeft = 0.0f;
        public float glideLength = 4.0f;

        public float glideApex = 1.0f; //how many seconds after you start gliding at which it reaches its apex
        public float glideApexForce = 600.0f; //glide power ranges from power*1 to power*(1+ApexAdd), with most power at apex time

        private float hoverMod;
        private float steerMod;
        private float airTime = 0.0f;
        private float glidePower;

        private bool onGround;
        private Vector3 clampVector;


        private bool grinding = false;
        private Transform grindRail = null;
        private Transform[] grindPoints;
        private Transform grindPoint = null;
        private Vector3 grindDir;
        private Vector3 initialGrindDir = Vector3.zero;
        public float grindHeight = 1.0f;
        public float grindSpeed = 10000;

        // Uses a temporary BoxCollider (unless there already is one attached) to compute the dimensions of the board.
        Vector3 CalculateBoardDimensions()
        {
                bool createdBoxCollider = false;
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                if (!boxCollider)
                {
                        boxCollider = gameObject.AddComponent<BoxCollider>();
                        createdBoxCollider = true;
                }
                Vector3 boxDimensions = new Vector3(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y, boxCollider.size.z * transform.localScale.z);
                if (createdBoxCollider)
                {
                        Destroy(boxCollider);
                }
                return boxDimensions;
        }

        Vector3[] CalculateSensorPositions(Vector3 boardDimensions)
        {
                Vector3[] sensorPositions = new Vector3[4];
                sensorPositions[0] = new Vector3(transform.position.x - boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z + boardDimensions.z / 2);
                sensorPositions[1] = new Vector3(transform.position.x + boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z + boardDimensions.z / 2);
                sensorPositions[2] = new Vector3(transform.position.x - boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z - boardDimensions.z / 2);
                sensorPositions[3] = new Vector3(transform.position.x + boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z - boardDimensions.z / 2);
                return sensorPositions;
        }

        void CreateSensors(Vector3[] sensorPositions)
        {
                m_sensors = new Transform[sensorPositions.Length];
                for (int i = 0; i < m_sensors.Length; i++) {
                        GameObject sensor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sensor.name = "Sensor";
                        sensor.transform.parent = transform;
                        sensor.transform.localPosition = transform.InverseTransformPoint(sensorPositions[i]);
                        sensor.transform.localScale *= thrusterScale;
                        Destroy(sensor.GetComponent<MeshRenderer>());
                        Destroy(sensor.GetComponent<Collider>());
                        m_sensors[i] = sensor.transform;
                }
        }

        void CreateThruster(Vector3 boardDimensions)
        {
                Vector3 thrusterPosition = transform.position;
                thrusterPosition.z = transform.position.z + boardDimensions.z / 2;
                GameObject thruster = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                thruster.name = "Thruster";
                thruster.transform.parent = transform;
                thruster.transform.localPosition = transform.InverseTransformPoint(thrusterPosition);
                thruster.transform.localScale *= thrusterScale;
                Destroy(thruster.GetComponent<MeshRenderer>());
                Destroy(thruster.GetComponent<Collider>());
                m_thruster = thruster.transform;
        }

        public void Move(float thrust, float lean, bool jump, float glide, bool glide2)
        {
                m_thrust = thrust;
                m_lean = lean;
                m_jump = jump;
                if (glide2){m_glide = 1.0f;}
                else{m_glide = Mathf.Abs (glide);}
        }

        /// <summary>
        /// Remove Noke from her board here.
        /// </summary>
        public delegate void DismissedBoardHandler();

        /// <summary>
        /// Fired when Noke should get off of her board.
        /// </summary>
        public event DismissedBoardHandler OnDismissBoard;

        public void DismissBoard()
        {
                if (true) // Under what conditions should this be allowed?
                        shouldDismount = true;
        }

        void Awake()
        {
                transform = GetComponent<Transform>();
                renderer = boardObj.GetComponent<Renderer>();//GetComponentInChildren<Renderer>();

                GameObject noke = GameObject.FindWithTag("Player");
                nokeAnimator = noke.GetComponent<Animator>();
                ridingId = Animator.StringToHash("Riding");

                hoverParticlesA = this.transform.GetComponentsInChildren<ParticleSystem>()[0];
				hoverParticlesA.startLifetime = 0.0f;
		
				hoverParticlesB = this.transform.GetComponentsInChildren<ParticleSystem>()[1];
				hoverParticlesB.startLifetime = 0.0f;
		
				grindParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[2];
				grindParticles.startLifetime = 0.0f;
		
				waterParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[3];
				waterParticles.startLifetime = 0.0f;
		
				splashParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[4];

                Vector3 boardDimensions = CalculateBoardDimensions();
                CreateSensors(CalculateSensorPositions(boardDimensions));
                CreateThruster(boardDimensions);
                m_hits = new RaycastHit[m_sensors.Length];
		
				GameObject cam = GameObject.FindWithTag("MainCamera");
				theCamera = cam.GetComponent<Camera>();
				cameraFOV = theCamera.fieldOfView;
        }

        void OnEnable()
        {
                renderer.enabled = true;
                rigidbody.isKinematic = false;
                nokeAnimator.SetBool(ridingId, true);
        }

        void OnDisable()
        {
                rigidbody.isKinematic = true;
                renderer.enabled = false;
                nokeAnimator.SetBool(ridingId, false);
        }

        void OnCollisionEnter(Collision collision) {
                detach = false;
                grinding = false;
				if (collision.transform.tag == "Water"){splashParticles.Emit (30);}
        }

        // Nearing rail
        void OnTriggerEnter(Collider col) {
                if(col.gameObject.tag == "Grind"){ 
                                grindRail = col.gameObject.transform;
                                // BUG: There seems to bug a bug that causes grinding to break if the transform isnt actually a bone.
                                // Skipping the first two transforms fixes this on the current build. (Uses: joint1 .. jointN)
                                grindPoints = grindRail.GetChild(0).GetChild(0).GetComponentsInChildren<Transform>();

                                initialGrindDir = rigidbody.velocity;
                                initialGrindDir.y = 0.0f;
                                initialGrindDir.Normalize();

                                Debug.Log("found a rail!");
                }
        }

        // On rail
        /*void OnTriggerStay(Collider col) {
                if(col.gameObject.tag == "Grind") {
                        grindPoint = nearestGrindPoint();

                        float mag = Vector3.Magnitude(rigidbody.velocity);
                        rigidbody.velocity = grindDir.normalized * mag;
                }
        }*/

        // Leaving Rail
	
        void OnTriggerExit(Collider col) {
                if(col.gameObject.transform == grindRail){ 
                        grinding = false;
                        grindRail = null;
						rigidbody.AddForce(grindDir.normalized * jumpForce/2, ForceMode.Impulse);
                        initialGrindDir = Vector3.zero;
			Debug.Log("left rail collision");
                }
        }
	

        void Update()
        {
                if (shouldDismount)
                        OnDismissBoard();
                shouldDismount = false; // Reset to avoid boarding at unexpected time
        }

        void FixedUpdate()
        {
                onGround = false;

                hoverMod = 1;
                if (detach){hoverMod = 0.5f;}
		
				bool waterSpray = false;

                for (int i = 0; i < m_sensors.Length; i++)
                {
                        if (Physics.Raycast(m_sensors[i].position, -m_sensors[i].up, out m_hits[i], hoverProperties.hoverHeight*(hoverMod)))
                        {
                                onGround = true;
                                if (!detach && !grinding){
                                        float hoverForce = (hoverProperties.hoverHeight - m_hits[i].distance) * hoverProperties.hoverDamping * Time.deltaTime;
                                        rigidbody.AddForceAtPosition(m_sensors[i].up * hoverForce, m_sensors[i].position);
										if (m_hits[i].transform.tag == "Water"){waterSpray = true;}
                                }
                        }
                }
		
				if (waterSpray){waterParticles.startLifetime = 0.55f;}
				else{waterParticles.startLifetime = 0.0f;}

                Vector3 dir;
                // Impulse toward rail on button press
                if(Input.GetButton("Grind") && grindRail != null) {
						grindParticles.startLifetime = 0.55f;
                        if (!grinding) {
                                grindPoint = nearestGrindPoint();
                                if(grindPoint != null) {
                                        float mag = Vector3.Magnitude(rigidbody.velocity);
                                        rigidbody.velocity = grindDir.normalized * mag;
										//Quaternion rot = Quaternion.RotateTowards(Quaternion.identity, Quaternion.LookRotation(grindDir), Time.deltaTime*10);
										//transform.rotation = rot * transform.rotation;
										transform.rotation = Quaternion.LookRotation(Vector3.Slerp (transform.forward,grindDir,0.2f));
                                        // FIXME!!!!!!!!!!!
                                        dir = grindPoint.position - transform.position;
                                        dir.y = 0.0f;
                                        dir.Normalize();
                                        Debug.DrawRay(transform.position,Vector3.up,Color.green,15.0f,false);
                                        Debug.DrawRay(transform.position,dir,Color.white,15.0f,false);
                                        Debug.DrawRay(transform.position + dir,Vector3.up,Color.red,15.0f,false);
                                        if( Vector3.Magnitude(grindPoint.position - transform.position) < 1.0f && Vector3.Dot(dir,rigidbody.velocity) > 0) { // Near
                                                grinding = true;
                                        } else { // Far 
                                                transform.position = Vector3.MoveTowards(transform.position,grindPoint.position + 2.0f * Vector3.up,0.5f);
                                        }
                                }
				else{
						grindRail = null; 
					rigidbody.AddForce(grindDir.normalized * jumpForce/2, ForceMode.Impulse);
						Debug.Log ("Reached end of rail");
				}
                        }/* 
							//the "grinding" bool is never really used meaningfully, so this never comes up
							else {
                                //rigidbody.useGravity = false;
                                m_lean = 0;
                                m_thrust = 0;

                                // Find nearest point on the rail in the
                                // direction of the player velocity
                                if (grindPoint == null || !Input.GetButton("Grind")){
                                        grinding = false;
                                        grindRail = null;
                                }
                                else{
                                        // Move toward rail
                                        Vector3 fixedDir = Vector3.zero;
                                        dir = grindPoint.position - transform.position;
                                        dir.y = 0.0f;
                                        fixedDir = grindDir + dir + grindHeight * Vector3.up;

                                        // Move along rail
                                        //rigidbody.AddForce(fixedDir.normalized * grindSpeed);
                                        transform.position = Vector3.MoveTowards(transform.position,grindPoint.position + 2.0f * Vector3.up,0.5f);

                                        // Update grind dir if we passed the grind point
                                        dir = grindPoint.position - transform.position;
                                        if( Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) <= 0 ) {
                                                float mag = Vector3.Magnitude(rigidbody.velocity);
                                                grindPoint = nearestGrindPoint();
                                                if(grindPoint != null) {
                                                        dir = grindPoint.position - transform.position;
                                                        dir.y = 0.0f;
                                                        fixedDir = grindDir + dir + grindHeight * Vector3.up;
                                                        rigidbody.velocity = fixedDir.normalized * mag;
                                                }
                                        }
                                }
                        }*/
                } else {
                        rigidbody.useGravity = true;
						grindParticles.startLifetime = 0.0f;
						if (grindRail!=null){
							grindRail = null;
							rigidbody.AddForce(grindDir.normalized * jumpForce, ForceMode.Impulse);
							rigidbody.AddForce(transform.up/2, ForceMode.Impulse);
							Debug.Log ("End of grind by button release");
						}
                }

                clampVector = transform.rotation.eulerAngles;

                if (clampVector.z > 180) { clampVector.z = - 180 + (clampVector.z-180);}
                if (clampVector.x > 180) { clampVector.x = - 180 + (clampVector.x-180);}

                currentClamp = angleClamp + ((airAngleClamp-angleClamp) * Mathf.Clamp (airTime/0.5f,0.0f,1.0f));

                clampVector.z = Mathf.Clamp (clampVector.z,-currentClamp,currentClamp);
                clampVector.x = Mathf.Clamp (clampVector.x,-currentClamp,currentClamp);

                transform.localEulerAngles = clampVector;
                if (!onGround && m_glide>0.5f && glideLeft > 0){
                        glidePower = glideForce + (((glideApexForce-glideForce) * Mathf.Pow (Mathf.Clamp(1 - (Mathf.Abs ((glideLength - glideApex)-glideLeft))/(glideLength-glideApex),0,1.0f),4.0f)));
                        rigidbody.AddForce(transform.up * glidePower);
                        glideLeft = Mathf.Clamp(glideLeft-(Time.deltaTime),0.0f,glideLength);
             
                        hoverParticlesA.startLifetime = 0.55f;//0.33f+(Mathf.Pow(glideLeft/glideLength,2.0f) * 2.66f);
                        hoverParticlesA.startSpeed = 2.5f+(Mathf.Pow (glideLeft/glideLength,2.0f)*3.0f);//0.5f + (Mathf.Pow (glideLeft/glideLength,2.0f)*2.5f);
                }
                else{hoverParticlesA.startLifetime = 0.0f;}

                //transform.rotation.eulerAngles.z = Mathf.Clamp (transform.rotation.eulerAngles.z, -90.0f, 90.0f);
                //transform.rotation.eulerAngles.x = Mathf.Clamp (transform.rotation.eulerAngles.x, -90.0f, 90.0f);
		
		
				///////////////////////////////////JUMP////////////////////////////////////////
                if (!onGround && detach){detach = false;}

                if (onGround){
                        airTime = 0;
                        glideLeft = Mathf.Clamp(glideLeft+(Time.deltaTime*3),0.0f,glideLength);
                	if (m_jump){
							Debug.Log ("on ground and jump is held");
                        	jumpPower = Mathf.Clamp(jumpPower+(Time.deltaTime)/jumpLength,0.0f,1.0f);
                        	}
                	else{
                        if (jumpPower > 0.0f){
								Debug.Log ("on ground and jump was released");
                                detach = true; 
                                rigidbody.AddForce(transform.up * jumpForce * Mathf.Sqrt(jumpPower), ForceMode.Impulse);
                                jumpPower = 0.0f;
								if (waterSpray){splashParticles.Emit (30);}
                        		}
                      	}
                }
                else{
                                jumpPower = 0.0f;
                                airTime += (Time.deltaTime);
                }
				
				////////////////////////////////MOVE + STEER///////////////////////////////
				GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
				cameraDir = Vector3.Normalize (camera.transform.forward);//Vector3.Normalize(this.transform.position - camera.transform.position);
				cameraDir.y = 0;
				
				Vector3 inputDir = new Vector3(m_lean,0,m_thrust);
				float angleSign = Math.Sign (Vector3.Cross(Vector3.forward,inputDir).y);
				Vector3 moveDir = Quaternion.AngleAxis (Vector3.Angle (Vector3.forward,inputDir)*angleSign,Vector3.up)*cameraDir;
				//Debug.Log ("Camera: "+cameraDir+", input: "+inputDir+", Player: "+transform.forward+", Move: "+moveDir);
				//rigidbody.AddTorque(transform.up*(1-Vector3.Dot (moveDir,transform.forward)*Vector3.Magnitude(inputDir)));
				rigidbody.AddForceAtPosition(moveDir * Vector3.Magnitude(inputDir) * thrustPower * (1 + (0.5f * jumpPower)), m_thruster.position);
				//Debug.Log (Vector3.Magnitude(rigidbody.velocity));
				theCamera.fieldOfView = cameraFOV 
											+ (//(15*jumpPower) 
											+ (30 * Math.Max (0,Vector3.Dot (Vector3.Normalize (rigidbody.velocity),cameraDir)))
											)*Math.Min(1,Vector3.Magnitude(rigidbody.velocity)/20);
				/*
                steerMod = 1;
                if (!onGround){steerMod = 0.66f;}

                rigidbody.AddForceAtPosition(m_thruster.forward * m_thrust * thrustPower * (1 + (0.25f * jumpPower)), m_thruster.position);
                rigidbody.AddTorque(transform.up * m_lean * steerMod * steerPower * (0.5f + ((1 - jumpPower)/2)));
                //transform.Rotate(transform.up * m_lean * steerMod * steerPower * (0.5f + ((1 - jumpPower)/1)));
                */

                //m_thrust = 0;
                //m_lean = 0;
                //m_jump = false;
		
				hoverParticlesB.startLifetime = hoverParticlesA.startLifetime;
				hoverParticlesB.startSpeed = hoverParticlesA.startSpeed;
        }

        Transform nearestGrindPoint()
        {
                Vector3 dir, start, end;
                Transform nearestTrans = null;

                Vector3 nearestPoint = new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
                foreach( Transform point in grindPoints ) {

                        Debug.DrawRay(point.position,Vector3.up,Color.blue,600.0f,false);

                        dir = point.position - transform.position;
                        dir.y = 0.0f;
                        // Rail in front of where we're going?
                        if( Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0 ) {
                                // Current point closer than nearest?
                                if(Vector3.Distance(point.position,transform.position) < Vector3.Distance(nearestPoint,transform.position)) {
                                        nearestPoint = point.position;
                                        nearestTrans = point;
                                }
                        }
                }

                // Construct grind direction vector
                if( nearestTrans != null ) {
                        if( nearestTrans.childCount != 0 ) {
                                dir = nearestTrans.GetChild(0).position - transform.position;
                                dir.y = 0.0f;
                                if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) < 0) {
                                        grindDir = nearestTrans.position - nearestTrans.GetChild(0).position;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0 && nearestTrans.parent == null) {
                                        grindDir = initialGrindDir.normalized;
                                        grindDir.Normalize();
                                } else if( Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0 && nearestTrans.parent != null ) {
                                        grindDir = nearestTrans.position - nearestTrans.parent.position;
                                        grindDir.Normalize();
                                }
                        } else if( nearestTrans.parent != null ) {
                                dir = nearestTrans.parent.position - transform.position;
                                dir.y = 0.0f;
                                if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) < 0) {
                                        grindDir = nearestTrans.position - nearestTrans.parent.position;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0 && nearestTrans.childCount == 0) {
                                        grindDir = initialGrindDir.normalized;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0 && nearestTrans.childCount != 0) {
                                        grindDir = nearestTrans.position - nearestTrans.GetChild(0).position;
                                        grindDir.Normalize();
                                }
                        } else {
                                Debug.Log("You shouldn't be here! Ever >:|");
                        }

                        if(Vector3.Magnitude(grindDir) != 0.0f) {
                                initialGrindDir = grindDir;
                                initialGrindDir.y = 0.0f;
                                initialGrindDir.Normalize();
                        }
                }

			return nearestTrans;
        }

        // Internal references
        [HideInInspector] new private Transform transform;
        [HideInInspector] new private Renderer renderer;
        [HideInInspector] new private ParticleSystem hoverParticlesA;
		[HideInInspector] new private ParticleSystem hoverParticlesB;
		[HideInInspector] new private ParticleSystem grindParticles;
		[HideInInspector] new private ParticleSystem waterParticles;
		[HideInInspector] new private ParticleSystem splashParticles;
		[HideInInspector] new private Vector3 cameraDir;
		[HideInInspector] new private Camera theCamera;
		[HideInInspector] new private float cameraFOV;

        // External references
        [HideInInspector] private Animator nokeAnimator;

        // Animator parameter references
        [HideInInspector] private int ridingId;

        // Temporary state
        private bool shouldDismount;
}