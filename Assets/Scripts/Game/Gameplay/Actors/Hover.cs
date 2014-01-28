using UnityEngine;
using System;
using ConditionalAttribute = System.Diagnostics.ConditionalAttribute;

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
		
	public bool canGlide = true;
	public bool canGrind = true;
	public bool canWater = true;
	public float maxWaterTime = 1.0f;
	private float waterTime = 1.0f;
	public float drownWiggle = 10.0f;

        [HideInInspector] Transform[] m_sensors;
        RaycastHit[] m_hits;
        Transform m_thruster;
		Transform b_thruster;
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
        //private Transform PgrindPoint = null;
        private Vector3 grindDir;
        private Vector3 initialGrindDir = Vector3.zero;
        public float grindHeight = 1.0f;
        public float grindSpeed = 0.4f;
        private bool grindJump = false;
	
		private bool doing180 = false;
		public float spinAmount = 0.0f;
		public float flipAmount = 0.0f;
		public float spinBoost = 4000f;
        private double flipTimes = 0;
        private double spinTimes = 0;
        public float bailAngle = 60.0f;

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
		
				thrusterPosition = transform.position;
                thrusterPosition.z = transform.position.z - boardDimensions.z / 2;
                thruster = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                thruster.name = "Thruster2";
                thruster.transform.parent = transform;
                thruster.transform.localPosition = transform.InverseTransformPoint(thrusterPosition);
                thruster.transform.localScale *= thrusterScale;
                Destroy(thruster.GetComponent<MeshRenderer>());
                Destroy(thruster.GetComponent<Collider>());
                b_thruster = thruster.transform;
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

                CheckParticleSystems();

                hoverParticlesA = this.transform.GetComponentsInChildren<ParticleSystem>()[0];
				hoverParticlesA.startLifetime = 0.0f;
		
				hoverParticlesB = this.transform.GetComponentsInChildren<ParticleSystem>()[1];
				hoverParticlesB.startLifetime = 0.0f;
		
				grindParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[2];
				grindParticles.startLifetime = 0.0f;
		
				waterParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[3];
				waterParticles.startLifetime = 0.0f;
				
				drownParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[5];
				drownParticles.startLifetime = 0.0f;
		
				splashParticles = this.transform.GetComponentsInChildren<ParticleSystem>()[4];

                Vector3 boardDimensions = CalculateBoardDimensions();
                CreateSensors(CalculateSensorPositions(boardDimensions));
                CreateThruster(boardDimensions);
                m_hits = new RaycastHit[m_sensors.Length];
		
				GameObject cam = GameObject.FindWithTag("MainCamera");
				theCamera = cam.GetComponent<Camera>();
				cameraFOV = theCamera.fieldOfView;
				whoosh = cam.GetComponent("CameraWhoosh") as CameraWhoosh;
        }

        void OnEnable()
        {
                renderer.enabled = true;
                rigidbody.isKinematic = false;
                nokeAnimator.SetBool(ridingId, true);
				spinAmount = 0f;
				flipAmount = 0f;
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
                Debug.Log("hit ground @ angles "+transform.rotation.eulerAngles);
                if ((transform.rotation.eulerAngles.x>bailAngle && transform.rotation.eulerAngles.x<360f-bailAngle) || (transform.rotation.eulerAngles.z>bailAngle && transform.rotation.eulerAngles.z<360f-bailAngle)){
					DismissBoard();
		}
				if (collision.transform.tag == "Water"){splashParticles.Emit (30);}
        }

        // Nearing rail
        void OnTriggerEnter(Collider col) {
                if(col.gameObject.tag == "Grind" && grindRail==null && canGrind){ 
                                grindRail = col.gameObject.transform;
                                // BUG: There seems to bug a bug that causes grinding to break if the transform isnt actually a bone.
                                // Skipping the first two transforms fixes this on the current build. (Uses: joint1 .. jointN)
                                grindPoints = grindRail.GetChild(0).GetChild(0).GetComponentsInChildren<Transform>();
                                //PgrindPoint = null;
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
	/**/
        void OnTriggerExit(Collider col) {
                if(col.gameObject.transform == grindRail && !grinding){ 
                        grindRail = null;
						//rigidbody.AddForce(grindDir.normalized * jumpForce/2, ForceMode.Impulse);
                        initialGrindDir = Vector3.zero;
						grindPoint = null;
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
				Transform activeThruster = m_thruster;
				if (doing180){activeThruster = b_thruster;}
				GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
				cameraDir = camera.transform.forward;//Vector3.Normalize (camera.transform.forward);//Vector3.Normalize(this.transform.position - camera.transform.position);
				cameraDir.y = 0;

                hoverMod = 1;
                if (detach){hoverMod = 0.5f;}
		
				bool waterSpray = false;
                for (int i = 0; i < m_sensors.Length; i++)
                {
                        if (Physics.Raycast(m_sensors[i].position, -m_sensors[i].up, out m_hits[i], hoverProperties.hoverHeight*(hoverMod)))
                        {
                                if (Vector3.Dot(m_sensors[i].up,Vector3.up)>.5){onGround = true;}
                                if (!detach && !grinding){
                                        float hoverForce = ((hoverProperties.hoverHeight - m_hits[i].distance) * hoverProperties.hoverDamping * Time.deltaTime)+ Mathf.Max(0,-rigidbody.velocity.y*1000f);;
                                        rigidbody.AddForceAtPosition(m_sensors[i].up * hoverForce, m_sensors[i].position);
										if (m_hits[i].transform.tag == "Water"){waterSpray = true;}
                                }
                        }
                }
				////////////////////////////////BOARD DROWNING//////////////////////////////////////////////
				if (!canWater && waterSpray) {
					waterTime -= Time.deltaTime;
					drownParticles.startLifetime = 0.05f;
			Vector3 myRotation = transform.InverseTransformDirection(this.transform.rotation.eulerAngles);
			Debug.Log ("actual: "+this.transform.rotation.eulerAngles+", rotated: "+myRotation);
			this.transform.rotation = Quaternion.LookRotation(
				transform.TransformDirection (
				new Vector3(myRotation.x + ((Mathf.Sin ((waterTime)*Mathf.PI*8) - Mathf.Sin ((waterTime+Time.deltaTime)*Mathf.PI*8))*drownWiggle),
			            myRotation.y,myRotation.z)
				));
			Debug.Log ("new: "+this.transform.rotation.eulerAngles);
					if (waterTime <=0 ){
						waterTime = maxWaterTime;
						drownParticles.startLifetime = 0.0f;
						DismissBoard();
						}
				} else {
					waterTime = maxWaterTime;
					drownParticles.startLifetime = 0.0f;
				}
				

                ///////////////////////////////////ANGLE CLAMPING//////////////////////////////////
                clampVector = transform.rotation.eulerAngles;
                
                if (clampVector.z%360 > 180) { clampVector.z = - 180 + (clampVector.z%360-180);}
                if (flipAmount%360 == 0){if (clampVector.x%360 > 180) { clampVector.x = - 180 + (clampVector.x%360-180);}}

                currentClamp = angleClamp + ((airAngleClamp-angleClamp) * Mathf.Clamp (airTime/0.5f,0.0f,1.0f));

                clampVector.z = Mathf.Clamp (clampVector.z%360,-currentClamp,currentClamp)+(float)Math.Truncate(clampVector.z/360)*360f;
                if (flipAmount%360 == 0){clampVector.x = Mathf.Clamp (clampVector.x%360,-currentClamp,currentClamp)+(float)Math.Truncate(clampVector.x/360)*360f;}
                
                //Debug.Log(", angle: "+transform.rotation.eulerAngles+", clamped to: "+clampVector);
                //Debug.Log(Mathf.Abs((transform.rotation.eulerAngles.x%180)-90f));
                if (flipAmount%360 == 0){transform.localEulerAngles = clampVector;}
		
				if (waterSpray){waterParticles.startLifetime = 0.55f;}
				else{waterParticles.startLifetime = 0.0f;}

                ///////////////////////////////////GRINDING/////////////////////////////////
                Vector3 dir;
                // Impulse toward rail on button press
                if( grindRail != null) {
                    //Transform Pcand = grindPoint;
					grindPoint = nearestGrindPoint();
                    //if (grindPoint!=Pcand){PgrindPoint = Pcand;}
					if (grindPoint != null){Debug.DrawRay(grindPoint.position,Vector3.up,Color.red,600.0f,false);}
                        if (!grinding) {
                                if(grindPoint != null && Input.GetButton("Grind")) {
										grinding = true;
                                        grindJump = m_jump;
                                }
                        } 
							else {
                                rigidbody.useGravity = false;
								float mag = Vector3.Magnitude(rigidbody.velocity);
                                rigidbody.velocity = grindDir.normalized * mag;

                                // Find nearest point on the rail in the
                                // direction of the player velocity
                                if (grindPoint == null || m_jump!=grindJump){
										rigidbody.useGravity = true;
                                        grinding = false;
                                        grindRail = null;
										grindPoint = null;
										rigidbody.AddForce((Quaternion.AngleAxis (90f*m_lean,Vector3.up) *grindDir.normalized + (Vector3.up/2)) * jumpForce, ForceMode.Impulse);
										//Debug.Log ("Reached end of rail");
                                }
                                else{
                                        // Move toward rail
                                        //Vector3 fixedDir = Vector3.zero;
                                        dir = grindPoint.position - transform.position;
                                        dir.y = 0.0f;
                                        //fixedDir = grindDir + dir + grindHeight * Vector3.up;
                                        float camSign = 1.0f;//Mathf.Sign(Vector3.Cross(cameraDir,grindDir).y);
                                        Vector3 TrueGrindDir = Vector3.Normalize((grindPoint.position + (2f*Vector3.up)) - transform.position);
                                        /*if (PgrindPoint!=null){
                                            TrueGrindDir = Vector3.Normalize(grindPoint.position - PgrindPoint.position);
                                        }
                                        else{
                                            TrueGrindDir = Vector3.Normalize((grindPoint.position + (2f*Vector3.up)) - transform.position);
                                        }*/
                                        Vector3 spinAxis = Vector3.Cross(TrueGrindDir,Vector3.right);//Vector3.Cross (grindDir,Vector3.forward);
                                        // Move along rail
                                        //rigidbody.AddForce(fixedDir.normalized * grindSpeed);

										//rigidbody.AddTorque(Vector3.up* Mathf.Sign (m_lean)*Mathf.Pow(m_lean,2f) * steerPower);
										spinAmount += Mathf.Sign (m_lean)*Mathf.Pow(m_lean,2f) * steerPower/1000;//rigidbody.angularVelocity.y;


										transform.rotation = Quaternion.LookRotation(Vector3.Slerp (transform.forward,
											Quaternion.AngleAxis (spinAmount*camSign,spinAxis) * TrueGrindDir,0.2f));
                                        transform.position = Vector3.MoveTowards(transform.position,grindPoint.position + 2.0f * Vector3.up,grindSpeed);

                                        // Update grind dir if we passed the grind point
                                        /*dir = grindPoint.position - transform.position;
                                        if( Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) <= 0 ) {
                                                float mag = Vector3.Magnitude(rigidbody.velocity);
                                                grindPoint = nearestGrindPoint();
                                                if(grindPoint != null) {
                                                        dir = grindPoint.position - transform.position;
                                                        dir.y = 0.0f;
                                                        fixedDir = grindDir + dir + grindHeight * Vector3.up;
                                                        rigidbody.velocity = fixedDir.normalized * mag;
                                                }
                                        }*/
                                }
							m_lean = 0; ////////////eliminate input during grinding
                         m_thrust = 0;
                        }
                }
				if (grinding){grindParticles.startLifetime = 0.55f;}
					else{grindParticles.startLifetime = 0f;}
                
		
				/////////////////////////////////GLIDE///////////////////////////////////////////
                if (!onGround && m_glide>0.5f && glideLeft > 0 && (transform.rotation.x<90 || transform.rotation.x>270) && canGlide){
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
                        	jumpPower = Mathf.Clamp(jumpPower+(Time.deltaTime)/jumpLength,0.0f,1.0f);
                        	}
                	else{
                        if (jumpPower > 0.0f){
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
				
				Vector3 inputDir = new Vector3(m_lean,0,m_thrust);
				float angleSign = Math.Sign (Vector3.Cross(Vector3.forward,inputDir).y);
				//Vector3 moveDir = Quaternion.AngleAxis (Vector3.Angle (Vector3.forward,inputDir)*angleSign,Vector3.up)*cameraDir;
                Quaternion stickToWorld = Quaternion.FromToRotation(Vector3.forward,cameraDir.normalized);
                Vector3 moveDir = stickToWorld * inputDir;
				//Debug.Log ("Camera: "+cameraDir+", input: "+inputDir+", Player: "+transform.forward+", Move: "+moveDir);
				rigidbody.AddForceAtPosition(moveDir * Vector3.Magnitude(inputDir) * thrustPower * (1 + (0.5f * jumpPower)), activeThruster.position);
		/////////////////////////////FLIPS////////////////////////////////
	if (!grinding){
		if (!onGround){
					//Debug.Log (flipAmount);
                Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);
                float flipSpeedX = localAngularVelocity.x;
                float flipSpeedZ = localAngularVelocity.z;
                if (Math.Abs(m_lean)>.1 /*&& flipAmount%360==0*/){
					rigidbody.AddRelativeTorque(Vector3.up* Mathf.Sign (m_lean)*(m_lean*m_lean) * steerPower);
                    spinAmount += localAngularVelocity.y;
                    spinTimes = Math.Truncate(spinAmount/360)+Mathf.Sign(m_lean);
                }
                else{
                    if (Math.Truncate(spinAmount/360)!=spinTimes && Math.Abs(spinAmount%360)>90 && Math.Abs(spinAmount%360)<270){
                        rigidbody.AddRelativeTorque(Vector3.up* Mathf.Sign (spinAmount) * steerPower * 2);
                        spinAmount += localAngularVelocity.y;
                        //Debug.Log("spin floor("+spinAmount/360+") going to "+spinTimes);
                    }
                localAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);
                rigidbody.angularVelocity = transform.TransformDirection(flipSpeedX,localAngularVelocity.y,flipSpeedZ);
                    /*else{
                        //Debug.Log("at spin rest!");
                        //rigidbody.angularVelocity = transform.TransformDirection(new Vector3(localAngularVelocity.x,0,localAngularVelocity.z));
                        rigidbody.AddTorque(transform.up*((((float)spinTimes*360f)-transform.rotation.eulerAngles.y)*100f));
                        spinAmount = (float)spinTimes*180f;
                    }*/
                    
                }
					
                localAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);
				if (m_thrust<-0.75f){
					rigidbody.AddRelativeTorque(Vector3.right* Math.Min (Mathf.Pow (m_thrust,3.0f),0) * steerPower);
                    flipAmount += localAngularVelocity.x;
                    flipTimes = Math.Truncate(flipAmount/360)+Mathf.Sign(flipAmount);
				}
                else{
                    if (Math.Truncate((flipAmount+localAngularVelocity.x)/360)!=flipTimes && flipAmount%360!=0){
                        rigidbody.AddRelativeTorque(Vector3.right * -steerPower * 2);
                        flipAmount += localAngularVelocity.x;
                        //Debug.Log("flip floor("+flipAmount/360+") going to "+flipTimes+", angle "+transform.rotation.eulerAngles.x);
                    }
                    else{
                        //Debug.Log("at flip rest! angle: "+transform.rotation.eulerAngles.x+", velocity: "+localAngularVelocity.x);
                        //rigidbody.angularVelocity = transform.TransformDirection(new Vector3(0,localAngularVelocity.y,localAngularVelocity.z));
                        flipAmount = (float)flipTimes*360f;
                        rigidbody.AddRelativeTorque(-Vector3.right*(((flipAmount-transform.rotation.eulerAngles.x)*1000/360f)));
                        
                    }
                    
                }
				
					
				
				
				}
		else{
					if (Math.Abs(spinAmount)>=270f){rigidbody.AddForceAtPosition(cameraDir * spinBoost, activeThruster.position,ForceMode.Impulse); whoosh.Boost(1.0f);}
					if (Math.Abs(flipAmount)>=270f){rigidbody.AddForceAtPosition(cameraDir * spinBoost*1.25f, activeThruster.position,ForceMode.Impulse); whoosh.Boost(1.25f);}
					spinAmount = 0f;
					flipAmount = 0f;
                    flipTimes = 0f;
                    spinTimes = 0f;
				}
	}
				//Debug.Log (Vector3.Magnitude(rigidbody.velocity));
				theCamera.fieldOfView = cameraFOV 
											+ (//(15*jumpPower) 
											+ (30 * Math.Max (0,Vector3.Dot (Vector3.Normalize (rigidbody.velocity),cameraDir)))
											)*Math.Min(1,Vector3.Magnitude(rigidbody.velocity)/20);
				whoosh.setSpeed(Mathf.Pow (Math.Min(1,Vector3.Magnitude(rigidbody.velocity)/20),6.0f));
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
		//new Vector3(transform.position.x,transform.position.y+10.0f,transform.position.z);
                foreach( Transform point in grindPoints ) {

                        //Debug.DrawRay(point.position,Vector3.up,Color.blue,600.0f,false);

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
				if (grindPoint!=null){
						bool cancelGrind = true;
						if (nearestTrans==grindPoint || nearestTrans==grindPoint.parent){cancelGrind = false;}
						if (grindPoint.childCount!=0){if (nearestTrans==grindPoint.GetChild (0)){cancelGrind = false;}}
						if (cancelGrind){nearestTrans = null;}
						}
				Debug.Log ("nearest is "+nearestTrans);
                // Construct grind direction vector
                if( nearestTrans != null ) {
                        if( nearestTrans.childCount != 0 ) {
                                dir = nearestTrans.GetChild(0).position - transform.position;
                                dir.y = 0.0f;
                                if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0) {
										Debug.Log ("GrindDir set toward "+nearestTrans.GetChild(0)+" (child of "+nearestTrans+")");
                                        grindDir = dir;//nearestTrans.position - nearestTrans.GetChild(0).position;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) <= 0 && nearestTrans.parent == null) {
										Debug.Log ("dir maintained (looking at childs)");
                                        grindDir = initialGrindDir.normalized;
                                        grindDir.Normalize();
                                } else if( Vector3.Dot(initialGrindDir.normalized,dir.normalized) <= 0 && nearestTrans.parent != null ) {
										Debug.Log ("GrindDir set toward "+nearestTrans.parent+" (parent of "+nearestTrans+")");
                                        grindDir = nearestTrans.parent.position - nearestTrans.position;
                                        grindDir.Normalize();
                                }
                        } else if( nearestTrans.parent != null ) {
                                dir = nearestTrans.parent.position - transform.position;
                                dir.y = 0.0f;
								//Debug.Log (Vector3.Dot(initialGrindDir.normalized,dir.normalized));
                                if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) > 0) {
										Debug.Log ("GrindDir set toward "+nearestTrans.parent+" (parent of "+nearestTrans+")");
                                        grindDir = dir;//nearestTrans.position - nearestTrans.parent.position;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) <= 0 && nearestTrans.childCount == 0) {
										Debug.Log ("dir maintained (looking at parents)");
                                        grindDir = initialGrindDir.normalized;
                                        grindDir.Normalize();
                                } else if(Vector3.Dot(initialGrindDir.normalized,dir.normalized) <= 0 && nearestTrans.childCount != 0) {
										Debug.Log ("GrindDir set toward "+nearestTrans.GetChild(0)+" (child of "+nearestTrans+")");
                                        grindDir = nearestTrans.GetChild(0).position - nearestTrans.position;
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
						else{
							grindDir = initialGrindDir;
							nearestTrans = null;
							}
                }

			return nearestTrans;
        }

        // Checks for and reports any unassigned particle systems.
        [ConditionalAttribute("DEBUG")]
        private void CheckParticleSystems()
        {
            bool haveMissingParticleSystem = false;
            if (hoverParticlesA == null)
            {
                Debug.LogError("HoverA particles unassigned.");
                haveMissingParticleSystem = true;
            }
            if (hoverParticlesB == null)
            {
                Debug.LogError("HoverB particles unassigned.");
                haveMissingParticleSystem = true;
            }
            if (grindParticles == null)
            {
                Debug.LogError("Grind particles unassigned.");
                haveMissingParticleSystem = true;
            }
            if (waterParticles == null)
            {
                Debug.LogError("Water particles unassigned.");
                haveMissingParticleSystem = true;
            }
            if (splashParticles == null)
            {
                Debug.LogError("Splash particles unassigned.");
                haveMissingParticleSystem = true;
            }
            if (haveMissingParticleSystem)
                Debug.Break();
        }

        // Internal references
        [HideInInspector] new private Transform transform;
        [HideInInspector] new private Renderer renderer;
        [HideInInspector] new private ParticleSystem hoverParticlesA;
		[HideInInspector] new private ParticleSystem hoverParticlesB;
		[HideInInspector] new private ParticleSystem grindParticles;
		[HideInInspector] new private ParticleSystem waterParticles;
		[HideInInspector] new private ParticleSystem splashParticles;
		[HideInInspector] new private ParticleSystem drownParticles;
		[HideInInspector] new private Vector3 cameraDir;
		[HideInInspector] new private Camera theCamera;
		[HideInInspector] new private float cameraFOV;
		[HideInInspector] new private CameraWhoosh whoosh;

        // External references
        [HideInInspector] private Animator nokeAnimator;

        // Animator parameter references
        [HideInInspector] private int ridingId;

        // Temporary state
        private bool shouldDismount;
}