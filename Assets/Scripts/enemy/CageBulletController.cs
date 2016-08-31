using UnityEngine;
using System.Collections;

public class CageBulletController : MonoBehaviour {

	public float cageTTL;			//amount of cage flight before dissapearance
	public float targetCheckRecharge; //how frequently to update target position
	public float cageSpeed;				//homing speed of cage
	private Animator cageAnimator;
	private GameObject target; 		//the target to capture in cage
	private string targetTag;		//tag of target to capture
	private Vector3 currentTargetPosition; // current known position of target 
	private Coroutine dissapear;	//handle to dissapearance of cage
	[HideInInspector]
	public bool caughtPrey;		//cage caught target
	private float lastPosCheck; 	//time of last target check
	void Awake () {
		cageAnimator = GetComponent<Animator>();
		caughtPrey = false;
		dissapear = StartCoroutine(FadeAway());
		Target = GameController.Instance.PlayerShip;
	}

	public GameObject Target {
		get {
			return target;
		} 
		set {
			target = value;
			targetTag = value.tag;
			currentTargetPosition = value.transform.position;
			lastPosCheck = Time.time;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > lastPosCheck + targetCheckRecharge) {
			currentTargetPosition = target.transform.position;
		}
		transform.position = Vector3.Slerp(transform.position, currentTargetPosition, Time.deltaTime * cageSpeed);

		if (caughtPrey) {
			
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(targetTag)) {
			caughtPrey = true;
			StopCoroutine(dissapear);
			cageAnimator.SetTrigger("Captured");
			other.gameObject.GetComponent<ShipController>().KillEngines();
			other.gameObject.GetComponent<ShipController>().enabled = false;
			other.gameObject.GetComponent<ShipShootingController>().enabled = false;
		}
	}

	public void AlertPlayerCatch(float seconds) {
		StartCoroutine(WaitAndAlert(seconds));
	}

	private IEnumerator WaitAndAlert(float seconds) {
		yield return new WaitForSeconds(seconds);
		var sceneManager = GameController.Instance.SceneManager; 
		if (sceneManager != null) {
			var sceneController = sceneManager.GetComponent<DogfightSceneController>();
			sceneController.SetScriptIndex(DogfightSceneController.POLICE_WIN_SCRIPT_INDEX);
		}
	}

	private IEnumerator FadeAway() {
		yield return new WaitForSeconds(cageTTL);
		if (!caughtPrey) {
			Destroy(gameObject);
		}
	}
}
