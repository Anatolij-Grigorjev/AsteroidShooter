using UnityEngine;
using System.Collections;
using System;

public class ShipController : MonoBehaviour {


	public float thrustSpeed;
    public float sideThrustSpeed;
	public float smokeStartThreshold;
    public float rotationSpeed;
    public float maxBreakingDelay;
    //rotation speed obtained by thrusting
    private float activeRotationSpeed;
	private Rigidbody2D shipBody;
	private SpriteRenderer shipImage;
	public ParticleSystem engineSmoke;

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
        activeRotationSpeed = rotationSpeed / 3;
        regularDrag = shipBody.drag;
        regularAngularDrag = shipBody.angularDrag;
        isBreaking = false;
        breakingDelay = maxBreakingDelay;
	}
	
    void FixedUpdate() {

        float horizontalAxis = Input.GetAxis ("Horizontal");
        float verticalAxis = Input.GetAxis ("Vertical");
        ProcessBreaking ();
        if (!isBreaking) {

            //FASHION: PROCESS THRUST ANIMATIONS AND SOUND
            engineBack.ProcessThrust (verticalAxis);
            var thrustLeft = horizontalAxis < 0 ? horizontalAxis : 0.0f;
            var thrustRight = horizontalAxis > 0 ? horizontalAxis : 0.0f;
            engineLeft.ProcessThrust (thrustLeft);
            engineRight.ProcessThrust (thrustRight);
            var em = engineSmoke.emission;
            if (engineSmoke.isPlaying && Mathf.Abs (verticalAxis) < smokeStartThreshold) {
                engineSmoke.Stop ();
                em.enabled = false;
            }
            if (engineSmoke.isStopped && Mathf.Abs (verticalAxis) > smokeStartThreshold) {
                engineSmoke.Play ();
                em.enabled = true;
            }
            PlaceSmoke ();

            //MOVEMENT: fly forward back using rigid body force and sideways via transform translation
            var flightVector = new Vector2 (transform.up.x, transform.up.y);
            var sideVector = new Vector2 (transform.right.x, transform.right.y);
            shipBody.AddForce (flightVector * verticalAxis * thrustSpeed);
        
            Vector2 mult = -sideVector * horizontalAxis * sideThrustSpeed * Time.deltaTime;
            transform.position = transform.position + new Vector3 (mult.x, mult.y, 0);
        } else {
            //during breaking just play the side engines continuously
            engineLeft.ProcessThrust (0.5f);
            engineRight.ProcessThrust (0.5f);
            engineBack.ProcessThrust (0.0f);
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
	}

    void ProcessBreaking () {
        //BREAKING: break by holding Space
        //TODO: Need sound/visual for "stabilizers"
        bool isBreakingPressed = Input.GetKey (KeyCode.Space);
        if (isBreakingPressed) {
            //the ship was already breaking - keep increasing drag to break harder
            if (isBreaking) {
                breakingDelay -= Time.deltaTime;
                if (breakingDelay <= 0.0f) {
                    breakingDelay = maxBreakingDelay;
                    shipBody.drag *= 2;
                    shipBody.angularDrag *= 2;
                }
            }
            else {
                //ship isnt breaking yet, lets save current drag and start breaking
                regularDrag = shipBody.drag;
                regularAngularDrag = shipBody.angularDrag;
                isBreaking = true;
                breakingDelay = maxBreakingDelay;
            }
        }
        else {
            //ship was breaking and stopped - return state and drag
            if (isBreaking) {
                breakingDelay = maxBreakingDelay;
                shipBody.drag = regularDrag;
                shipBody.angularDrag = regularAngularDrag;
                isBreaking = false;
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
}
