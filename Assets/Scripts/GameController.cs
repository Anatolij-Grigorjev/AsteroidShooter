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

    public List<JSONNode> LevelWaves {
        get {
            return currentLevelWaves;
        }
    }

    private List<String> scriptsPaths = new List<String>() {
        "intro_scene",
        "post_game_scene"
    };
    private List<String> levelNames = new List<string>() {
        "Level1"
    };
    [HideInInspector]
    public List<String> avatarNames = new List<string>() {
        "daughter",
        "father"
    };

    private List<JSONNode> currentLevelWaves;
    private Dictionary<String, Sprite> avatarsMap;
    private int currentSceneIndex = 0;
    private int currentLevelIndex = 0;
    private GameObject playerShip;
    private QuipController shipQuipper;

    [HideInInspector]
    public List<GameObject> currentEnemies;
    [HideInInspector]
    public int nextSceneIndex;

	// Use this for initialization
	void Awake () {
        CookAvatarsMap ();
        currentEnemies = new List<GameObject> ();
        LoadLevel ();
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

    void LoadLevel () {
        var scriptName = levelNames [currentLevelIndex];
        var textAsset = Resources.Load(String.Format("Text/EnemyPlacement/{0}", scriptName), typeof(TextAsset)) as TextAsset;

        var waves = JSON.Parse (textAsset.text).AsArray;
        Debug.Log ("Got " + waves.Count + " waves for level " + scriptName);
        if (currentLevelWaves == null) {
            currentLevelWaves = new List<JSONNode> ();
        } else {
            currentLevelWaves.Clear ();
        }
        currentLevelWaves.AddRange (waves.Childs);
        Debug.Log ("Ready with " + currentLevelWaves.Count + " valid waves!");
    }
        
        
}
