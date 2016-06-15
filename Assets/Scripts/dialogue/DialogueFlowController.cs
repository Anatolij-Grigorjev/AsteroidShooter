using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class DialogueFlowController : MonoBehaviour {

    private const int DIALOGUE_LINE_PARTS_COUNT = 3;

    public List<DialogueLine> lines;

    public String scriptName;
    private bool scriptLoaded;

    private int currentLine = 0;

	// Use this for initialization
	void Awake () {
        scriptLoaded = false;
        lines = new List<DialogueLine> ();
        StartCoroutine_Auto( ReadScript(scriptName));
	}

    IEnumerator ReadScript (string scriptName) {
        try {
            string line;
            StreamReader reader = new StreamReader(String.Format("Dialogue/{0}.txt", scriptName), Encoding.Unicode);

            using(reader) {
                do {
                    line = reader.ReadLine();

                    if (line != null) {

                        var parts = line.Split(new char[]{';'}, DIALOGUE_LINE_PARTS_COUNT);
                        if (parts.Length == DIALOGUE_LINE_PARTS_COUNT) {
                            //can make a dialogue line out of this!
                            var newLine = new DialogueLine();
                            newLine.lineText = parts[2];
                            newLine.name = parts[1];
                            newLine.avatar = Utils.GetDialogueAvatar(parts[0]);

                            lines.Add(newLine);
                        }

                    }
                } while (line != null);
            }
        } catch(Exception e) {
            Debug.LogError (String.Format("Couldnt read file because {0}\n!", e.Message));
            return null;
        }
        scriptLoaded = true;
        return null;
    }
	
	// Update is called once per frame
	void Update () {
        //ignore input while script isnt loaded
        if (!scriptLoaded) {
            return;
        }

	}
}
