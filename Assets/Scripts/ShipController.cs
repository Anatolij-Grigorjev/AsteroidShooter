using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {


	public float thrustSpeed;
	public float turnSpeed;
	public float smokeStartThreshold;
	private Rigidbody2D shipBody;
	private SpriteRenderer shipImage;
	public ParticleSystem engineSmoke;

    public ShipEngineController engineBack;
    public ShipEngineController engineLeft;
    public ShipEngineController engineRight;
	

	// Use this for initialization
	void Start () {
		shipBody = GetComponent<Rigidbody2D> ();
//		engineSmoke = GetComponentInChildren<ParticleSystem> ();
		shipImage = GetComponent<SpriteRenderer> ();
		

	}
	
	void FixedUpdate() {

		var horizontalAxis = Input.GetAxis ("Horizontal");
		var verticalAxis = Input.GetAxis ("Vertical");

        engineBack.ProcessThrust (verticalAxis);
        var thrustLeft = horizontalAxis < 0 ? horizontalAxis : 0.0f;
        var thrustRight = horizontalAxis > 0 ? horizontalAxis : 0.0f;
        engineLeft.ProcessThrust (thrustLeft);
        engineRight.ProcessThrust (thrustRight);

		var em = engineSmoke.emission;
		if (engineSmoke.isPlaying && Mathf.Abs(verticalAxis) < smokeStartThreshold) {
			engineSmoke.Stop ();
			em.enabled = false;
		}
		if (engineSmoke.isStopped && Mathf.Abs(verticalAxis) > smokeStartThreshold) {
			engineSmoke.Play ();
			em.enabled = true;
		}

		PlaceSmoke ();

		//good handling
//		shipBody.drag = horizontalAxis * 3;
//		shipBody.angularDrag = verticalAxis * 3;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

        //up axis seems to represent facing rotation in 2d
        var flightVector = new Vector2 (transform.up.x, transform.up.y);
        var sideVector = new Vector2 (transform.right.x, transform.right.y);
        shipBody.AddForce (flightVector * verticalAxis * thrustSpeed);
        shipBody.AddForce (sideVector * horizontalAxis * thrustSpeed);
//      transform.Rotate (new Vector3 (0, 0, -horizontalAxis * turnSpeed));
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
