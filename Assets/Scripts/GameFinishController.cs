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
	private List<Tuple<String, Boolean>> lines; //lines some of which are picture names (true if they are)
	private bool scriptLoaded;
    private int currentLine = 0; //index of text element currently being written to box
    private int currentSymbol = 0; //current symbol to type out in current line
    private bool lineDone = false; //is current line fully written
    [HideInInspector]
    public bool textOver = false;

    private double delayRecharge;
	// Use this for initialization
	void Awake () {
		scriptName = GameController.Instance.gameFinishScript;
		lines = new List<Tuple<String, Boolean>>();
		scriptLoaded = false;
		finishSceneText.text = "";
		//make sure the game happens from start to finish properly the second time around
		GameController.Instance.Restart = true;
		StartCoroutine(ReadGameFinishScript(scriptName));
	}

	private IEnumerator ReadGameFinishScript(string scriptName) {
		var textAsset = Resources.Load(String.Format("Text/GameEnd/{0}", scriptName), typeof(TextAsset)) as TextAsset;
        string[] textLines = textAsset.text.Split(new char[] {'\n'});

		for (int i = 0; i < textLines.Length; i++) {
			var split = textLines[i].Split(new char[] {';'});
			var useIndex = split.Length > 1? 1 : 0;
			lines.Add(new Tuple<String, Boolean>(split[useIndex].Trim(), useIndex < 1));	
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
			//dealing with image
			if (lines[currentLine].second) {
				//just insert image and move on

				var resource = Resources.Load (
					String.Format("Images/GameEnd/{0}", lines[currentLine].first)
					, typeof(Sprite)) as Sprite;

				finishSceneImages.sprite = resource;
				lineDone = true;
				// currentLine++;
			} else {
				//dealing with text line
				//if user pressed advance button - finish line, prepare for next one
				if (Input.GetButtonUp ("Main Cannon")) {
					lineDone = true;
					finishSceneText.text = lines[currentLine].first;
				} else {
					delayRecharge -= Time.deltaTime;

					if (delayRecharge < 0) {
						finishSceneText.text += lines [currentLine].first[currentSymbol];
						currentSymbol++;
						delayRecharge = typeDelay;
						lineDone = currentSymbol >= lines [currentLine].first.Length;
					}
				}
			}

        } else {
            //wait for user to press key and move on to next line (or move on anyway if its an image)
            if (Input.GetButtonUp ("Main Cannon") || (currentLine < lines.Count && lines[currentLine].second)) {
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
