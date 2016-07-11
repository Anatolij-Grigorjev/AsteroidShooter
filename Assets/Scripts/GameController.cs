using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public string NextScript {
        get {
            var result = scriptsPaths [currentSceneIndex];
            currentSceneIndex++;
            if (currentSceneIndex >= scriptsPaths.Count) {
                currentSceneIndex = 0;
            }

            return result;
        }
    }

    private List<String> scriptsPaths = new List<String>() {
        "intro_scene",
        "post_game_scene"
    };
    [HideInInspector]
    public List<String> avatarNames = new List<string>() {
        "daughter",
        "father"
    };
    [HideInInspector]
    public Dictionary<String, Sprite> avatarsMap;

    private int currentSceneIndex = 0;
    private GameObject playerShip;

    [HideInInspector]
    public int currentAsteroids;
    [HideInInspector]
    public int nextSceneIndex;

	// Use this for initialization
	void Awake () {
        
	}
}
