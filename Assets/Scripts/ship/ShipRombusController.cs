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

	private Transform ship;
	private float shipRotation;



	void Start () {
		isExploding = false;
		ship = GameObject.FindGameObjectWithTag ("Ship").transform;
		transform.rotation = Quaternion.Euler(new Vector3(ship.rotation.eulerAngles.x, ship.rotation.eulerAngles.y, ship.rotation.eulerAngles.z + 90));
		shipRotation = ship.rotation.eulerAngles.z + 90; //nose leads by 90
		position = gameObject.transform.position;

		particles = Instantiate (particlesPrefab, transform.position, transform.rotation) as GameObject;

		StartCoroutine (Explode());
	}
	
	// Update is called once per frame
	void Update () {
		if (!isExploding) {
			transform.position = position;
            float delta = rombusVelocity * Time.deltaTime;
			position.x += (delta * Mathf.Cos(Mathf.Deg2Rad * shipRotation));
			position.y += (delta * Mathf.Sin(Mathf.Deg2Rad * shipRotation));

            particles.gameObject.transform.position = transform.position;
		} else {
		}
	}

	IEnumerator Explode() {
		yield return new WaitForSeconds (flightClip.length);
		isExploding = true;

		//spawn EXPLOSION
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		Destroy (particles, 1.0f);
		Destroy (gameObject);
	}
}
