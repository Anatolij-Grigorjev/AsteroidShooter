using UnityEngine;
using System.Collections;

public class DogfightSceneController : MonoBehaviour {

	public GameObject playerShip;
	public GameObject policeShip;
	public Camera mainCamera;
	public GameObject dialogueBorders;
	// Use this for initialization
	void Awake () {
		//begin by disabling everything
		playerShip.GetComponent<ShipController>().enabled = false;
		playerShip.GetComponent<ShipShootingController>().enabled = false;
		policeShip.GetComponent<EnemyAIController>().enabled = false;
		mainCamera.GetComponent<CameraController>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
