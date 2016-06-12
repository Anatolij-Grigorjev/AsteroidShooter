using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController> {

	protected GameController() {}

    [HideInInspector]
    public GameObject PlayerShip {
        get {
            if (playerShip == null) {
                playerShip = GameObject.FindGameObjectWithTag ("Ship");
            }
            return playerShip;
        }
    }

    private GameObject playerShip;

    [HideInInspector]
    public int currentAsteroids;

	// Use this for initialization
	void Awake () {
        playerShip = GameObject.FindGameObjectWithTag ("Ship");
	}
}
