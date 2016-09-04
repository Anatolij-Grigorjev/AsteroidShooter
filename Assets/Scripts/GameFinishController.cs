using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class GameFinishController : MonoBehaviour {


	public Image finishSceneImages;
	public Text finishSceneText; 
	public String scriptName;
    public double typeDelay = 0.1; //typing delay between symbols, in seconds
	private List<String> lines;
	private bool scriptLoaded;
    private int currentLine = 0; //index of text element currently being written to box
    private int currentSymbol = 0; //current symbol to type out in current line
    private bool lineDone = false; //is current line fully written
    [HideInInspector]
    public bool textOver = false;

    private double delayRecharge;
	// Use this for initialization
	void Awake () {
		// scriptName = GameController.Instance.gameFinishScript;
		lines = new List<String>();
		scriptLoaded = false;
		finishSceneText.text = "";
		StartCoroutine(ReadGameFinishScript(scriptName));
	}

	private IEnumerator ReadGameFinishScript(string scriptName) {
		var textAsset = Resources.Load(String.Format("Text/GameEnd/{0}", scriptName), typeof(TextAsset)) as TextAsset;
        string[] textLines = textAsset.text.Split(new char[] {'\n'});

		string imageName = textLines[0].Trim();
		Debug.Log("Loading image: " + imageName);
		var resource = Resources.Load (String.Format("Images/GameEnd/{0}", imageName), typeof(Sprite)) as Sprite;
		Debug.Log("Sprite: " + resource);
		finishSceneImages.sprite = resource;

		for (int i = 1; i < textLines.Length; i++) {
			lines.Add(textLines[i]);	
		}
		scriptLoaded = true;

		yield return 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (!scriptLoaded) {
			return;
		}

		//some amount of line left to write
        if (!lineDone) {
            //if user pressed advance button - finish line, prepare for next one
            if (Input.GetButtonUp ("Main Cannon")) {
                lineDone = true;
				finishSceneText.text = lines[currentLine];
            } else {
                delayRecharge -= Time.deltaTime;

                if (delayRecharge < 0) {
                    finishSceneText.text += lines [currentLine][currentSymbol];
                    currentSymbol++;
                    delayRecharge = typeDelay;
                    lineDone = currentSymbol >= lines [currentLine].Length;
                }
            }

        } else {
            //wait for user to press key and move on to next line
            if (Input.GetButtonUp ("Main Cannon")) {
                if (!textOver) {
                    currentLine++;
                    textOver = currentLine >= lines.Count;
                    if (!textOver) {
                        currentSymbol = 0;
                        lineDone = false;
                        finishSceneText.text = "";
                    }
                } else {
                    SceneManager.LoadScene(GameSceneIndexes.CREDITS_SCENE);
                }
            }
        }
	}



}
