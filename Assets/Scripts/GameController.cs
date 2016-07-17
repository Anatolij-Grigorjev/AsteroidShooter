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
    [HideInInspector]
    public QuipController ShipQuipper {
        get {
            if (shipQuipper == null) {
                var ship = PlayerShip;
                if (ship == null) {
                    return null;
                }
                shipQuipper = ship.GetComponentInChildren<QuipController> ();
            }

            return shipQuipper;
        }
    }


    public Sprite GetAvatar(String key) {
        var sprite = avatarsMap [key];
        if (avatarsMap.Count == 0) {
            CookAvatarsMap ();
            GetAvatar (key);
        }

        return sprite;
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


    private Dictionary<String, Sprite> avatarsMap;
    private int currentSceneIndex = 0;
    private GameObject playerShip;
    private QuipController shipQuipper;

    [HideInInspector]
    public int currentAsteroids;
    [HideInInspector]
    public int nextSceneIndex;

	// Use this for initialization
	void Awake () {
        Debug.Log ("Cooking avatars...");
        CookAvatarsMap ();
        Debug.Log ("Cooked up " + avatarsMap.Count + " avatars!");
	}


    void CookAvatarsMap () {
        if (avatarsMap == null) {
            avatarsMap = new Dictionary<string, Sprite> ();
        }
        foreach (String avatarName in avatarNames) {
            Sprite resource = (Sprite)Resources.Load ("Images/Dialogue/" + avatarName, typeof(Sprite));
            avatarsMap.Add (avatarName, resource);
        }
    }
}
