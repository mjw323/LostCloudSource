#pragma strict
public var normalChroma : float = 0f;
public var normalBlur : float = 1.0f;
public var normalAlpha : float = 0.1;
public var boostChroma : float = 13.0f;
public var boostBlur : float = 5.0f;
public var speed : float = 0f;
public var boost : float = 0f;
public var boostFade : float = 0.25f;

private var vig: Vignetting;
private var part: ParticleSystem;

function Start () {
vig = GetComponent(Vignetting);
part = this.transform.GetChild(0).GetComponent(ParticleSystem);
}

function Update () {
	if (boost > 0f){
		boost -= Time.deltaTime;
	}
	var boostRat:float = Mathf.Min(1,boost/boostFade);
	
	vig.blur = Mathf.Max((normalBlur*speed),(boostRat*boostBlur));
	vig.chromaticAberration = Mathf.Max((normalChroma*speed),(boostRat*boostChroma));
	part.startColor.a = Mathf.Max((normalAlpha*speed),(boostRat*1.0f));
}

function setSpeed (spd : float){
	speed = spd;
}

function Boost (spd : float){
	boost = spd;
}