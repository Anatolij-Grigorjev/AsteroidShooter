using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController> {

	protected GameController() {}

    [HideInInspector]
    public GameObject playerShip;
    [HideInInspector]
    public int currentAsteroids;

	// Use this for initialization
	void Awake () {
        playerShip = GameObject.FindGameObjectWithTag ("Ship");
	}
}
