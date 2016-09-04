using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using AssemblyCSharp;

public class GameCreditsController : MonoBehaviour {

	public Text titleText;		//text explaining title of those involved
	public Text peopleText;		//text listing people involved
	public string creditsFile;		//Name of credits file
	private bool creditsLoaded;
	private List<Tuple<String, String>> titleAndFolksList;
	private float latestCreditStart;		//when was the latest credit started to be shown
	private int currentCreditIndex;
	// Use this for initialization
	void Awake () {
		creditsLoaded = false;
		titleAndFolksList = new List<Tuple<String, String>>();
		StartCoroutine(ReadCreditsFile(creditsFile));
		currentCreditIndex = 0;
	}

	private IEnumerator ReadCreditsFile(string fileName) {
		var textAsset = Resources.Load(String.Format("Text/GameEnd/{0}", fileName), typeof(TextAsset)) as TextAsset;
        string[] creditsPieces = textAsset.text.Split(new char[] {','});

		foreach(String aPiece in creditsPieces) {
			string[] pieceSplit = aPiece.Split(new char[] {';'});
			titleAndFolksList.Add(new Tuple<String, String>(pieceSplit[0], pieceSplit[1]));
		}
		currentCreditIndex = 0;
		ApplyCredit(titleAndFolksList[currentCreditIndex]);
		latestCreditStart = Time.time;
		creditsLoaded = true;
		yield return 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (!creditsLoaded) {
			return;
		}
		var currentCredit = peopleText.text;
		var btnPress = Input.anyKey;
		if (Time.time > latestCreditStart + (currentCredit.Length / 5)) {
			currentCreditIndex++;
			if (currentCreditIndex >= titleAndFolksList.Count) {
				btnPress = true;
			}
			if (!btnPress) {
				latestCreditStart = Time.time;
				ApplyCredit(titleAndFolksList[currentCreditIndex]);
			}
		}

		if (btnPress) {
			SceneManager.LoadScene(GameSceneIndexes.MENU_INTRO_SCENE);
		}
	}

	private void ApplyCredit(Tuple<String, String> credit) {
		titleText.text = credit.first;
		peopleText.text = credit.second;
	}
}
