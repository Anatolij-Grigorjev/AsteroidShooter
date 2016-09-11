using UnityEngine;
using System;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

	public float thrustSpeed;
    public float sideThrustSpeed;
	public float smokeStartThreshold;
    public float rotationSpeed;
    public float maxBreakingDelay;
    public float breakingMultiplier = 2.5f;
    //minimum required velocity to activate ship sideburns animation
    public float minSideburnsVelocity = 0.666f; 
    //rotation speed obtained by thrusting
    private float activeRotationSpeed;
	private Rigidbody2D shipBody;
	private SpriteRenderer shipImage;
	public ParticleSystem engineSmoke;
    public Animator shipBreakingAnimator;
    public Animator shipThrustingAnimator;
    public SpriteRenderer shipBreakingGraphics;
    public AudioSource shipEngineSource;

    public ShipEngineController engineBack;
    public ShipEngineController engineLeft;
    public ShipEngineController engineRight;
	
    //multipliers for movement given movement modes
    // 0 - multiplier for regular movement
    // 1 - multiplier for turbo mode movement
    public List<float> modeMultipliers;

    private bool isBreaking;
    private bool isTurboMode;
    private bool prevTurboPressed;
    private float regularDrag;
    private float regularAngularDrag;

    //currently active multiplier
    private float activeMultiplier;

    private bool isMorphing = false;

    private float breakingDelay; //how gradually does breaking happen overtime

	// Use this for initialization
	void Awake () {
		shipBody = GetComponent<Rigidbody2D> ();
//		engineSmoke = GetComponentInChildren<ParticleSystem> ();
		shipImage = GetComponent<SpriteRenderer> ();
		
        //init internal handling vars
        activeRotationSpeed = 2 * rotationSpeed / 3;
        regularDrag = shipBody.drag;
        regularAngularDrag = shipBody.angularDrag;
        isBreaking = false;
        shipBreakingAnimator.enabled = false;
        shipBreakingGraphics.enabled = false;
        prevTurboPressed = false;
        breakingDelay = maxBreakingDelay;
        activeMultiplier = modeMultipliers [0];
	}
	
    void FixedUpdate() {

//        float horizontalAxis = Input.GetAxis ("Horizontal");
        float verticalAxis = Input.GetAxis ("Vertical");
        if (!isMorphing) {
            bool pressedTurbo = Input.GetButton ("Booster");
            ProcessTurbo (pressedTurbo);
        }

        ProcessBreaking ();
        if (!isBreaking) {

            //FASHION: PROCESS THRUST ANIMATIONS AND SOUND
            var verticalFwd = verticalAxis > 0 ? verticalAxis : 0.0f;
            var verticalBack = verticalAxis < 0 ? verticalAxis : 0.0f;
            engineBack.ProcessThrust (verticalFwd);
//            var thrustLeft = horizontalAxis < 0 ? horizontalAxis : 0.0f;
//            var thrustRight = horizontalAxis > 0 ? horizontalAxis : 0.0f;
            engineLeft.ProcessThrust (verticalBack);
            engineRight.ProcessThrust (verticalBack);
            var em = engineSmoke.emission;
            if (isBreaking || engineSmoke.isPlaying && verticalFwd < smokeStartThreshold) {
                engineSmoke.Stop ();
                em.enabled = false;
            }
            if (engineSmoke.isStopped && verticalFwd > smokeStartThreshold && !isBreaking) {
                engineSmoke.Play ();
                em.enabled = true;
            }
            PlaceSmoke ();

            //MOVEMENT: fly forward back using rigid body force and sideways via transform translation
            var flightVector = new Vector2 (transform.up.x, transform.up.y);
//            var sideVector = new Vector2 (transform.right.x, transform.right.y);

            shipBody.AddForce (flightVector * verticalAxis * thrustSpeed * activeMultiplier);
        
//            Vector2 mult = -sideVector * horizontalAxis * sideThrustSpeed * Time.deltaTime * activeMultiplier;
//            transform.position = transform.position + new Vector3 (mult.x, mult.y, 0);

        } else {
            //during breaking just play the side engines continuously
            engineLeft.ProcessThrust (0.5f);
            engineRight.ProcessThrust (0.5f);
            engineBack.ProcessThrust (0.0f);
            
            if (!shipBreakingAnimator.enabled && shipBody.velocity.magnitude > minSideburnsVelocity) {
                shipBreakingAnimator.enabled = true;
                shipBreakingGraphics.enabled = true;
            } else if (shipBreakingAnimator.enabled && shipBody.velocity.magnitude < minSideburnsVelocity) {
                shipBreakingAnimator.enabled = false;
                shipBreakingGraphics.enabled = false;
            }
        }


        //ROTATION: rotate ship according to mouse look
        Vector2 mousePos = Input.mousePosition;
        var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        var offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        bool shipIsThrusting = Mathf.Abs (verticalAxis) > 0.0f;

        var wantedRotation = Quaternion.Euler (0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp (transform.rotation, wantedRotation, 
            Time.deltaTime * (shipIsThrusting? (activeRotationSpeed * activeMultiplier): (rotationSpeed * activeMultiplier)));

	}

    void SetMorphing(int morph) {
        isMorphing = morph > 0? true : false;
    }

    void StopEngineSmoke() {
        if (engineSmoke.isPlaying) {
            engineSmoke.Stop ();
            var em = engineSmoke.emission;
            em.enabled = false;
        }
    }

    void ProcessTurbo (bool pressedTurbo) {
        //only process stuff if animation is in a calm state
        var state = shipThrustingAnimator.GetCurrentAnimatorStateInfo(0);
        // if (state.IsName("EngagingThrusters") || state.IsName("HidingThrusters")) {
        //     Debug.Log("[TURBO] At " + DateTime.Now.Ticks 
        //         + " turbo pressed=" + pressedTurbo 
        //         + " prevTurbo=" + prevTurboPressed
        //     );
        // }
        //check if the current state is currently being played  
        if (
            (state.IsName("EngagingThrusters") || state.IsName("HidingThrusters")) 
                && state.length > state.normalizedTime
        ) {
            return;
        }
        //TODO: solve double shift turbo animation problem before shipping
        if (pressedTurbo != prevTurboPressed) {
            shipThrustingAnimator.ResetTrigger("Thruster"); //cancel previous animation commands
            // Debug.Log("[TURBO] At " + DateTime.Now.Ticks 
            //     + ": setting trigger! turbo=" + pressedTurbo
            //     + " prevTurbo=" + prevTurboPressed
            // );
            shipThrustingAnimator.SetTrigger ("Thruster");
        }
        isTurboMode = pressedTurbo;
        if (isTurboMode) {
            activeMultiplier = modeMultipliers [1];
        }
        else {
            activeMultiplier = modeMultipliers [0];
        }
        prevTurboPressed = pressedTurbo;
    }

    void PlayThrustSound(AudioClip clip) {
        shipEngineSource.clip = clip;
        shipEngineSource.Play ();
    }

    void ProcessBreaking () {
        //BREAKING: break by holding Space
        bool isBreakingPressed = Input.GetButton ("Stabilizers");
        if (isBreakingPressed) {
            //the ship was already breaking - keep increasing drag to break harder
            if (isBreaking) {
                breakingDelay -= Time.deltaTime;
                if (breakingDelay <= 0.0f) {
                    breakingDelay = maxBreakingDelay;
                    shipBody.drag *= breakingMultiplier;
                    shipBody.angularDrag *= breakingMultiplier;
                }
            }
            else {
                //ship isnt breaking yet, lets save current drag and start breaking
                regularDrag = shipBody.drag;
                regularAngularDrag = shipBody.angularDrag;
                isBreaking = true;
                breakingDelay = maxBreakingDelay;
                if (shipBody.velocity.magnitude > minSideburnsVelocity) {
                    shipBreakingAnimator.enabled = true;
                    shipBreakingGraphics.enabled = true;
                }
            }
        }
        else {
            //ship was breaking and stopped - return state and drag
            if (isBreaking) {
                breakingDelay = maxBreakingDelay;
                shipBody.drag = regularDrag;
                shipBody.angularDrag = regularAngularDrag;
                isBreaking = false;
                shipBreakingAnimator.enabled = false;
                shipBreakingGraphics.enabled = false;
            }
            else {
                //breaking wasnt pressed and the ship wasnt even breaking, nothing to do  
            }
        }
    }
		
	void PlaceSmoke ()
	{
		if (engineSmoke.emission.enabled && engineSmoke.IsAlive()) {
			Vector3 position = new Vector3 (
			//position is set by rotating nozzle of aircraft
			//+90 becuase nozzle 90 degrees misplaced from rotation origin
				                  transform.position.x + (shipImage.bounds.extents.x * Mathf.Cos (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z - 90)))
			, transform.position.y + (shipImage.bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z - 90)))
			, transform.position.z
			                  );
			engineSmoke.transform.position = position;
			engineSmoke.transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 90));
		}
	}

    public void KillEngines () {
        foreach (ShipEngineController sec in new ShipEngineController[] {engineBack, engineLeft, engineRight}) {
            sec.ProcessThrust (0);
        }
        if (isTurboMode) {
            isTurboMode = false;
            shipThrustingAnimator.SetTrigger("Thruster");
        }
        StopEngineSmoke();
        shipBody.angularDrag = 15.0f;
        shipBody.drag = 15.0f;
    }
}
