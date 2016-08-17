using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

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

    public String NextLevel {
        get {
            var result = levelNames [currentLevelIndex];
            currentLevelIndex++;
            if (currentLevelIndex >= levelNames.Count) {
                currentLevelIndex = 0;
            }

            return result;
        }
    }

    public List<JSONNode> LevelWaves {
        get {
            return currentLevelWaves;
        }
    }

    public Dictionary<int, Animator> produceAnimationsForScript(String script) {
        var result = new Dictionary<int, Animator> ();

        switch (script) {
            case "intro_scene":
                result.Add (2, GameObject.Find ("ScreenImagePast").GetComponent<Animator> ());
                //result.Add (5, GameObject.Find ("ScreenImagePresent").GetComponent<Animator> ());
                break;
            case "post_level_1": 
                break;
            default: 
                break;
        }

        return result;
    }

    private List<String> scriptsPaths = new List<String>() {
        "intro_scene",
        "post_level_1",
        "post_level_2"
    };
    private List<String> levelNames = new List<string>() {
        "Level1",
        "Level2"
    };
    [HideInInspector]
    public List<String> avatarNames = new List<string>() {
        "daughter",
        "father"
    };

   
    private Dictionary<String, Sprite> avatarsMap;
    private int currentSceneIndex = 0;
    private int currentLevelIndex = 0;
    private GameObject playerShip;
    private QuipController shipQuipper;

    [HideInInspector]
    public List<GameObject> currentEnemies;
    [HideInInspector]
    public int nextSceneIndex;
    [HideInInspector]
    public List<JSONNode> currentLevelWaves;

	// Use this for initialization
	void Awake () {
        CookAvatarsMap ();
        currentEnemies = new List<GameObject> ();
	}


    void CookAvatarsMap () {
        if (avatarsMap == null) {
            avatarsMap = new Dictionary<string, Sprite> ();
        }
        foreach (String avatarName in avatarNames) {
            Sprite resource = (Sprite)Resources.Load ("Images/Dialogue/" + avatarName, typeof(Sprite));
            avatarsMap.Add (avatarName, resource);
        }
        Debug.Log ("Cooked up " + avatarsMap.Count + " avatars!");
    }
        
        
        
}
