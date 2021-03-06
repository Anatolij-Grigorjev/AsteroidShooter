﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssemblyCSharp;
using SimpleJSON;

public class GameController : Singleton<GameController> {

	protected GameController() {}
    public GameObject PlayerShip {
        get {
            if (playerShip == null) {
                playerShip = GameObject.FindGameObjectWithTag ("Ship");
            }
            return playerShip;
        }
    }
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
    public int SceneIndex {
        get {
            return currentSceneIndex;
        }
        set {
            currentSceneIndex = value;
        }
    }
    public GameObject SceneManager {
        get {
            if (currentSceneManager == null) {
                currentSceneManager = GameObject.FindGameObjectWithTag("SceneManager");
            }

            return currentSceneManager;
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
            var result = scriptsPaths [currentDialogueScriptIndex];
            currentDialogueScriptIndex++;
            if (currentDialogueScriptIndex >= scriptsPaths.Count) {
                currentDialogueScriptIndex = 0;
            }

            return result;
        }
    }

    public String NextLevel {
        get {
            currentLevelIndex++;
            if (currentLevelIndex >= levelNames.Count) {
                currentLevelIndex = 0;
            }

            return levelNames [currentLevelIndex];
        }
    }

    public String CurrentLevel {
        get {
            return levelNames[currentLevelIndex];
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

    /**
        Check if the level was loaded as part of a restart. Resets the restart flag after getting it once
    **/
    public bool Restart {
        get {
            var result = restartState;
            if (result) {
                restartState = false;
            }
            return result;
        }
        set {
            restartState = value;
        }
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
    public List<String> avatarNames = new List<string>() {
        "daughter",
        "father",
        "police"
    };

   
    private Dictionary<String, Sprite> avatarsMap;
    private int currentDialogueScriptIndex = 0;
    private int currentLevelIndex = 0;
    private int currentSceneIndex = GameSceneIndexes.MENU_INTRO_SCENE;
    private GameObject playerShip;
    private GameObject currentSceneManager;
    private QuipController shipQuipper;

    [HideInInspector]
    public List<GameObject> currentEnemies;
    [HideInInspector]
    public int nextSceneIndex;
    [HideInInspector]
    public List<JSONNode> currentLevelWaves;

    public string gameFinishScript; //correct script file for game finsh
    private bool restartState = true;

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

    void Update() {
        if (Input.GetButton("Cancel")) {
            Application.Quit();
        }
    }
        
        
        
}
