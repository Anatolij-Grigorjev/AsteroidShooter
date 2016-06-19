using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class DialogueFlowController : MonoBehaviour {

    private const int DIALOGUE_LINE_PARTS_COUNT = 3;

    public List<DialogueLine> lines;
    private Dictionary<String, Sprite> avatarsMap;

    //name of dialogue script file
    private String scriptName;
    //flag if the file has been processed into data yet (only start dialogue after it has been)
    private bool scriptLoaded;

    //index of dialogue element currently being written to box
    private int currentLine = 0;
    //current symbol to type out in current line
    private int currentSymbol = 0;

    //access to UI text where line is rendered
    public Text lineText;
    //access to UI text where talker name is provided
    public Text avatarName;
    //access to UI Image for avatar face
    public Image avatarFace;

    //is current line fully written
    private bool lineDone = false;

    private bool dialogueOver = false;

    //typing delay between symbols, in seconds
    public double typeDelay = 0.1;

    //directory with sprites that are dialogue faces (this directory must contain pngs)
    public string avatarsDirectory;

    private double delayRecharge;

    public Animator shipAnimatinoController;

	// Use this for initialization
	void Awake () {
        scriptLoaded = false;
        lines = new List<DialogueLine> ();
        avatarsMap = new Dictionary<string, Sprite> ();
        lineText.text = "";
        avatarName.text = "";
        delayRecharge = typeDelay;
        scriptName = GameController.Instance.NextScript;
        StartCoroutine(
            ReadScript (scriptName)
//            ;
        );

	}

    IEnumerator ReadScript (string scriptName) {
        yield return new WaitUntil(() => CookAvatarsMap (avatarsDirectory));
        try {
            string line;
            StreamReader reader = new StreamReader(String.Format("Assets/Dialogue/{0}.txt", scriptName), Encoding.UTF8);

            using(reader) {
                //first line has next stage index

                try {
                    string indexLine = reader.ReadLine();
                    GameController.Instance.nextSceneIndex = int.Parse(indexLine);
                } catch (Exception e) {
                    Debug.LogError(e);
                    GameController.Instance.nextSceneIndex = 0;
                }


                do {
                    line = reader.ReadLine();
//                    Debug.Log("Read line: " + line);
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
                } while (line != null);
            }
        } catch(Exception e) {
            Debug.LogError (String.Format("Couldnt read file because {0}\n!", e.Message));
            yield break;
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
            
            //if user pressed advance button - finish line, prepare for next one
            if (Input.GetButtonUp ("Fire1")) {
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
            if (Input.GetButtonUp ("Fire1")) {
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
                    if (!shipAnimatinoController.GetBool ("dialogueOver")) {
                        shipAnimatinoController.SetBool ("dialogueOver", true);
                    }
                }

            }

        }

	}

    bool CookAvatarsMap (string avatarsDirectory) {

        string[] files = Directory.GetFiles (avatarsDirectory, "*.png");

        foreach (String fileName in files) {
            int dividerIndex = fileName.LastIndexOf (@"\");
            string justFileName = fileName.Substring (dividerIndex + 1);
            string justName = justFileName.Remove(justFileName.LastIndexOf(".png"));
            Sprite resource = (Sprite)Resources.Load ("Images/Dialogue/" + justName, typeof(Sprite));
//            Debug.Log ("Resource was loaded from Images/Dialogue/" + justName + ": " + (resource != null));
            avatarsMap.Add (justName, resource);
        }

//        foreach (KeyValuePair<String, Sprite> kvp in avatarsMap) {
//            Debug.Log (String.Format ("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
//        }
        return true;
    }

    Sprite GetDialogueAvatar (string avatarKey) {
        return avatarsMap[avatarKey];
    }

    void SetupFace (int currentLine) {
        
        if (currentLine >= lines.Count) {
            return;
        }

        avatarName.text = lines [currentLine].name;
        avatarFace.sprite = lines [currentLine].avatar;
    }
}
