public var mat: Material; // drag a semitransparent material here
public var range: float = 10; // range of the capsule cast
 
private var shape: Transform; 
 
function RenderVolume(p1: Vector3, p2: Vector3, radius: float, dir: Vector3, distance: float){
    if (!shape){ // if shape doesn't exist yet, create it
        shape = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        Destroy(shape.collider); // no collider, please!
        shape.renderer.material = mat; // assign the selected material to it
    }
    var scale: Vector3; // calculate desired scale
    var diam: float = 2 * radius; // calculate capsule diameter
    scale.x = diam; // width = capsule diameter
    scale.y = Vector3.Distance(p2, p1) + diam; // capsule height
    scale.z = distance + diam; // volume length
    shape.localScale = scale; // set the rectangular volume size
    // set volume position and rotation
    shape.position = (p1 + p2 + dir.normalized * distance) / 2;
    shape.rotation = Quaternion.LookRotation(dir, p2 - p1);
    shape.renderer.enabled = true; // show it
}
 
function HideVolume(){ // hide the volume
    if (shape) shape.renderer.enabled = false;
}
 
// Example:
 
private var freeDistance : float = 0;
 
function Update () {

	var forward: Vector3 = transform.TransformDirection(Vector3.forward) ;
		if(Physics.Raycast(transform.position, transform.forward,  50)){
			Debug.DrawRay(transform.position, transform.forward*5, Color.green);
		}

	var hit : RaycastHit;
	if (Physics.SphereCast(transform.position, 2f, transform.forward, hit, 100f)) {                                                                            
			if(hit.collider.gameObject.tag == "Player"){                             
				Debug.DrawRay(transform.position, transform.forward*5, Color.red);    
				transform.LookAt(hit.transform);
			}                                                                                                                                                       
	}

    if (Input.GetKey("p")){ // while P pressed...
        
        var charContr : CharacterController = GetComponent(CharacterController);
        var radius = charContr.radius;
        // find centers of the top/bottom hemispheres
        var p1 : Vector3 = transform.position + charContr.center;
        var p2 = p1;
        var h = charContr.height/2 - radius;
        p2.y += h;
        p1.y -= h;
        // draw CapsuleCast volume:
        RenderVolume(p1, p2, radius, transform.forward, range);
        // cast character controller shape range meters forward:
        if (Physics.CapsuleCast(p1, p2, radius, transform.forward, hit, range)){
            // if some obstacle inside range, save its distance 
            freeDistance = hit.distance; 
        } else { 
            // otherwise shows that the way is clear up to range distance
            freeDistance = range;
        }
    }
    if (Input.GetKeyUp("p")){
        HideVolume(); // hide volume when P is released
    }
}
 
function OnGUI(){ // shows the free distance ahead:
    GUI.Label(Rect(10, 60, 150, 40), "Free distance = " + freeDistance .ToString("F2"));
}