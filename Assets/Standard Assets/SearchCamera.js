
	// Grab the camera's view when this variable is true.
	private var grab: boolean;
	private var visible: boolean;
	public var threshold: float = 5;
	public var frequency: float; //how many seconds to wait betwen checks
	private var timer: float;
	public var tsize: int = 16;  //needs to match up with size of RenderTexture or camera window
	
	function Awake(){
		timer = frequency;
	}
	
	function UpdateLook(){
		grab = true;
	}
	
	public function CanSee(){
		return visible;
	}
	
	function Update(){
		timer -= Time.deltaTime;
		if (timer <= 0){
			UpdateLook();
			timer = frequency;
		}
	}

	// The "display" is the object whose texture will be set
	// to the captured image.
	//var display: Renderer;

	function OnPostRender() {
		if (grab) {  //grab can be set to true only when you want to do a check, i.e. maybe once per second....
			// Make a new texture of the right size and
			// read the camera image into it.
			
			var tex = new Texture2D(tsize, tsize, TextureFormat.ARGB32, false);
			//tex.ReadPixels(new Rect(0, 0, tsize, tsize), 0, 0);
			
			/*if (false){  //pattern for debugging 
					// Fill the texture with Sierpinski's fractal pattern!
				for (var y : int = 0; y < tex.height; ++y) {
					for (var x : int = 0; x < tex.width; ++x) {
						//var color = (x&y) ? Color.white : Color.gray;
						var color = (y/32) ? Color.white : Color.gray;
						tex.SetPixel (x, y, color);
					}
				}
			}*/
			var targetHit : int = 0;
			var targetThresh : int = threshold;  //number of pixels needed to register visible.
			tex.ReadPixels(new Rect(0, 0, tsize, tsize), 0, 0);
			tex.Apply();
			if (true){
			  var xx : int =0;
			  var zz : int =0;
			  for (xx=0;xx<tsize;xx++){
  			     for (zz=0;zz<tsize;zz++){
  			        var pixcol : Color = tex.GetPixel(xx,zz);
			        //Debug.Log("texture col[" + xx + ", " + zz + "] = " + pixcol);
			        if (pixcol.Equals(Color.red)){
			           targetHit++;
//			           Debug.Log("TARGET hit");
			        }
			     }
			  }
			}
			visible = false;
			if (targetHit > targetThresh)
			{
						           visible = true;
			}
			targetHit = 0;
			
			// Set the display texture to the newly captured image.
			//display.material.mainTexture = tex;
			
			// Reset the grab variable to avoid making multiple
			// captures.
			grab = false;
		}
	}