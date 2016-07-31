using UnityEngine;
using System.Collections;
using System;

public class ShipController : MonoBehaviour {


	public float thrustSpeed;
    public float sideThrustSpeed;
	public float smokeStartThreshold;
    public float rotationSpeed;
    public float maxBreakingDelay;
    public float breakingMultiplier = 2.0f;
    //minimum required velocity to activate ship sideburns animation
    public float minSideburnsVelocity = 0.5f; 
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
	
    private bool isBreaking;
    private float regularDrag;
    private float regularAngularDrag;

    private float breakingDelay; //how gradually does breaking happen overtime

	// Use this for initialization
	void Start () {
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
        breakingDelay = maxBreakingDelay;
	}
	
    void FixedUpdate() {

        float horizontalAxis = Input.GetAxis ("Horizontal");
        float verticalAxis = Input.GetAxis ("Vertical");
        bool pressedTurbo = Input.GetButtonUp ("Booster");
        ProcessBreaking ();
        if (!isBreaking) {

            //FASHION: PROCESS THRUST ANIMATIONS AND SOUND
            engineBack.ProcessThrust (verticalAxis);
            var thrustLeft = horizontalAxis < 0 ? horizontalAxis : 0.0f;
            var thrustRight = horizontalAxis > 0 ? horizontalAxis : 0.0f;
            engineLeft.ProcessThrust (thrustLeft);
            engineRight.ProcessThrust (thrustRight);
            var em = engineSmoke.emission;
            if (isBreaking || engineSmoke.isPlaying && Mathf.Abs (verticalAxis) < smokeStartThreshold) {
                engineSmoke.Stop ();
                em.enabled = false;
            }
            if (engineSmoke.isStopped && Mathf.Abs (verticalAxis) > smokeStartThreshold && !isBreaking) {
                engineSmoke.Play ();
                em.enabled = true;
            }
            var state = shipThrustingAnimator.GetCurrentAnimatorStateInfo (0);
            if (pressedTurbo && (state.IsName("Idling") || state.IsName("Thrusting"))) {
                shipThrustingAnimator.SetTrigger ("Thruster");
            }
            PlaceSmoke ();

            //MOVEMENT: fly forward back using rigid body force and sideways via transform translation
            var flightVector = new Vector2 (transform.up.x, transform.up.y);
            var sideVector = new Vector2 (transform.right.x, transform.right.y);

            shipBody.AddForce (flightVector * verticalAxis * thrustSpeed);
        
            Vector2 mult = -sideVector * horizontalAxis * sideThrustSpeed * Time.deltaTime;
            transform.position = transform.position + new Vector3 (mult.x, mult.y, 0);

            Debug.Log ("[FIXED1]Ship Position: " + transform.position);
        } else {
            //during breaking just play the side engines continuously
            engineLeft.ProcessThrust (0.5f);
            engineRight.ProcessThrust (0.5f);
            engineBack.ProcessThrust (0.0f);
            if (engineSmoke.isPlaying) {
                engineSmoke.Stop ();
                var em = engineSmoke.emission;
                em.enabled = false;
            }
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
        bool shipIsThrusting = Mathf.Abs (verticalAxis) > 0.0f || Mathf.Abs (horizontalAxis) > 0.0f;

        var wantedRotation = Quaternion.Euler (0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp (transform.rotation, wantedRotation, 
            Time.deltaTime * (shipIsThrusting?activeRotationSpeed : rotationSpeed));

        Debug.Log ("[FIXED2]Ship Position: " + transform.position);
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
    }

    void LateUpdate() {
        Debug.Log ("[LATE]Ship Position: " + transform.position);
    }
}
