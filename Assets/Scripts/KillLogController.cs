using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KillLogController : MonoBehaviour {

	private Text logText;
	private Queue<string> logData;
	public int logSize;

	// Use this for initialization
	void Start () {
		logText = GetComponent<Text> ();
		logData = new Queue<string> (logSize);
		RefreshText ();
	}

	public void AddDeath(string victimName) {
		logData.Enqueue (victimName);
		while (logData.Count > logSize) {
			logData.Dequeue ();
		}
		RefreshText ();
	}

	private void RefreshText() {
		logText.text = "";
		foreach (string name in logData) {
			logText.text += ("You killed " + name + "!\n");
		}
	}
	
	// Update is called once per frame
	void Update () {



	}
}
