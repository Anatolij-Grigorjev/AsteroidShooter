using UnityEngine;
using System.Collections;

public class ShipRombusController : MonoBehaviour {

	public float rombusVelocity;
	public AnimationClip flightClip;
	private GameObject particles;
	public GameObject particlesPrefab;
	public GameObject explosionPrefab;
	// Use this for initialization
	private bool isExploding;
	private Vector3 position;

	[HideInInspector]
	public GameObject shooter;

	private string shooterTag;
	private float shooterRotation;

	void Awake () {
		isExploding = false;
		position = gameObject.transform.position;
		particles = Instantiate (particlesPrefab, transform.position, transform.rotation) as GameObject;
		StartCoroutine (Explode());
	}

	public void setShooter(GameObject shooter) {
		this.shooter = shooter;
		shooterTag = shooter.tag;
		var shooterTransform = this.shooter.transform;
		var shooterRotationEuler = shooterTransform.rotation.eulerAngles;
		shooterRotationEuler.z += 90;
		transform.rotation = Quaternion.Euler(shooterRotationEuler);
		shooterRotation = shooterRotationEuler.z; //nose leads by 90
	}
	
	// Update is called once per frame
	void Update () {
		if (!isExploding) {
			transform.position = position;
            float delta = rombusVelocity * Time.deltaTime;
			position.x += (delta * Mathf.Cos(Mathf.Deg2Rad * shooterRotation));
			position.y += (delta * Mathf.Sin(Mathf.Deg2Rad * shooterRotation));

            particles.gameObject.transform.position = transform.position;
		} else {
		}
	}

	IEnumerator Explode() {
		yield return new WaitForSeconds (flightClip.length);
		isExploding = true;

		//spawn EXPLOSION
		var boom = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
		boom.GetComponent<ShipRombusBoomController>().ShooterTag = shooterTag;
		Destroy (particles, 1.0f);
		Destroy (gameObject);
	}
}
