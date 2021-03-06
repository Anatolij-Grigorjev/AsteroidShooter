﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using AssemblyCSharp;

public class DialogueFlowController : MonoBehaviour {

    private const int DIALOGUE_LINE_PARTS_COUNT = 3;

    public List<DialogueLine> lines;
    //access to UI text where line is rendered
    public Text lineText;
    //access to UI text where talker name is provided
    public Text avatarName;
    //access to UI Image for avatar face
    public Image avatarFace;

    public Dictionary<int, Animator> lineAnimations;

    //name of dialogue script file
    public String scriptName;
    //flag if the file has been processed into data yet (only start dialogue after it has been)
    private bool scriptLoaded;
    
    //index of dialogue element currently being written to box
    private int currentLine = 0;
    //current symbol to type out in current line
    private int currentSymbol = 0;
    //is current line fully written
    private bool lineDone = false;
    [HideInInspector]
    public bool dialogueOver = false;

    //typing delay between symbols, in seconds
    public double typeDelay = 0.1;

    private double delayRecharge;

    public Animator shipAnimatinoController;

	// Use this for initialization
	void Awake () {
        ResetForScript(String.IsNullOrEmpty(scriptName)? GameController.Instance.NextScript : scriptName);
	}

    public void ResetForScript(String scriptName) {
        this.scriptName = scriptName;
        scriptLoaded = false;
        lines = new List<DialogueLine> ();
        lineText.text = "";
        avatarName.text = "";
        delayRecharge = typeDelay;
        lineAnimations = GameController.Instance.produceAnimationsForScript (scriptName);
        StartCoroutine(ReadScript (scriptName));
        lineDone = false;
        dialogueOver = false;
        currentLine = 0;
        currentSymbol = 0;
    }

    IEnumerator ReadScript (string scriptName) {
        string line;
        var textAsset = Resources.Load(String.Format("Text/Dialogue/{0}", scriptName), typeof(TextAsset)) as TextAsset;

        string[] textLines = textAsset.text.Split(new char[] {'\n'});

        try {
            string indexLine = textLines[0];
            GameController.Instance.nextSceneIndex = int.Parse(indexLine);
        } catch (Exception e) {
            Debug.LogError(e);
            GameController.Instance.nextSceneIndex = GameSceneIndexes.MENU_INTRO_SCENE;
        }

        for (int i = 1; i < textLines.Length; i++) {
            line = textLines[i];
            //Debug.Log("Read line: " + line);
            if (line != null) {
                var parts = line.Split(new char[]{';'}, DIALOGUE_LINE_PARTS_COUNT);
                if (parts.Length == DIALOGUE_LINE_PARTS_COUNT) {
                    //can make a dialogue line out of this!
                    var newLine = new DialogueLine();
                    newLine.lineText = parts[2];
                    newLine.name = parts[1];
                    newLine.avatar = GetDialogueAvatar(parts[0]);

                    lines.Add(newLine);
                }
            }
        }
//        Debug.LogAssertion (String.Format("Salvaged {0} lines from file {1}", lines.Count, scriptName));
        scriptLoaded = true;
        SetupFace (0);
        yield return 0;
    }

	
	// Update is called once per frame
	void Update () {
        //ignore input while script isnt loaded
        if (!scriptLoaded) {
            return;
        }

        //some amount of line left to write
        if (!lineDone) {
            CheckLineAnimation ();
            //if user pressed advance button - finish line, prepare for next one
            if (Input.GetButtonUp ("Main Cannon")) {
                lineDone = true;
                lineText.text = lines [currentLine].lineText;
            } else {

                delayRecharge -= Time.deltaTime;

                if (delayRecharge < 0) {
                    lineText.text += lines [currentLine].lineText [currentSymbol];
                    currentSymbol++;
                    delayRecharge = typeDelay;
                    lineDone = currentSymbol >= lines [currentLine].lineText.Length;

                }
            }

        } else {
            //wait for user to press key and move on to next line
            if (Input.GetButtonUp ("Main Cannon")) {
                if (!dialogueOver) {
                    currentLine++;
                    dialogueOver = currentLine >= lines.Count;
                    if (!dialogueOver) {
                        currentSymbol = 0;
                        SetupFace (currentLine);
                        lineDone = false;
                        lineText.text = "";
                    }
                } else {
                    
                    if (shipAnimatinoController != null) {
                        if (!shipAnimatinoController.GetBool ("dialogueOver")) {
                            shipAnimatinoController.SetBool ("dialogueOver", true);
                        }
                    }
                }

            }

        }

	}

    void CheckLineAnimation () {
        Animator animator = null;
        try {
            animator = lineAnimations [currentLine];
            if (!animator.enabled) {
                animator.enabled = true;
            }
        }
        catch (KeyNotFoundException e) {
            //keep this quiet
        }
    }        

    Sprite GetDialogueAvatar (string avatarKey) {
        return GameController.Instance.GetAvatar (avatarKey);
    }

    void SetupFace (int currentLine) {
        
        if (currentLine >= lines.Count) {
            return;
        }

        avatarName.text = lines [currentLine].name;
        avatarFace.sprite = lines [currentLine].avatar;
    }
}
