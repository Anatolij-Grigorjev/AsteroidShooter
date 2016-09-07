using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using AssemblyCSharp;

public class DogfightSceneController : MonoBehaviour {

	public GameObject playerShip;
	public GameObject policeShip;
	public Camera mainCamera;
	public GameObject dialogueBorders;
	public List<string> sceneScriptNames;
	public GameObject loadingScreen;	//loading sceen to engage was capture is done
	public const int INTRO_SCRIPT_INDEX = 0; //scene start script location in script names
	public const int POLICE_WIN_SCRIPT_INDEX = 1; //script for police apprehending Turnip
	public const int SHIP_WIN_SCRIPT_INDEX = 2; //script for Turnip slipping away from police


	private DialogueFlowController dialogueFlowController;
	// Use this for initialization
	private int currentScriptIndex;
	private bool sceneEnding; 		//indicator to start police brutality coroutine
	void Awake () {
		//begin by disabling everything
		ToggleSceneActors(false);
		GameController.Instance.SceneIndex = GameSceneIndexes.DOGFIGHT_SCENE;
		currentScriptIndex = INTRO_SCRIPT_INDEX;
		dialogueFlowController = GetComponent<DialogueFlowController>();
		dialogueFlowController.ResetForScript(sceneScriptNames[currentScriptIndex]);
		sceneEnding = false;
		loadingScreen.SetActive(false);
		dialogueBorders.SetActive(true);
	}

	private void ToggleSceneActors(bool enable) {
		playerShip.GetComponent<ShipController>().enabled = enable;
		playerShip.GetComponent<ShipShootingController>().enabled = enable;
		policeShip.GetComponent<EnemyAIController>().enabled = enable;
		mainCamera.GetComponent<CameraController>().enabled = enable;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (currentScriptIndex == INTRO_SCRIPT_INDEX) {
			if (dialogueFlowController.dialogueOver) {
				if (dialogueBorders.activeInHierarchy) {
					var dialogueAnimator = dialogueBorders.GetComponent<Animator>();
					var currState = dialogueAnimator.GetCurrentAnimatorStateInfo(0);

					//still in fade in state after dialogue over
					if (currState.IsName("FadeInDialogue")) {
						StartCoroutine(FadeOutDialogueToAction(dialogueAnimator));
					}
				}
			}
		}
		if (currentScriptIndex == POLICE_WIN_SCRIPT_INDEX || currentScriptIndex == SHIP_WIN_SCRIPT_INDEX) {
			//chatter stopped, time to wrap up the scene
			if (dialogueFlowController.dialogueOver) {
				//seems its time to end the scene
				StartCoroutine(FadeOutDialogueToSceneEnd(dialogueBorders.GetComponent<Animator>()));
			}
		}
	}

	public void SetScriptIndex(int newIndex) {
		if (currentScriptIndex != newIndex) {
			currentScriptIndex = newIndex;
			dialogueFlowController.ResetForScript(sceneScriptNames[currentScriptIndex]);
			ToggleSceneActors(false);

			//set the game controller finish game script name

			switch(newIndex) {
				case POLICE_WIN_SCRIPT_INDEX:
					GameController.Instance.gameFinishScript = "game_end_prison_script";
					break;
				case SHIP_WIN_SCRIPT_INDEX:
					GameController.Instance.gameFinishScript = "game_end_ship_win_script";
					break;
			}

			dialogueBorders.SetActive(true);
		}
	}
	

	IEnumerator FadeOutDialogueToAction(Animator dialogueAnimator) {
		dialogueAnimator.SetTrigger("FadeOut");
		// var nextState = dialogueAnimator.GetNextAnimatorStateInfo(0);
		yield return new WaitForSeconds(1.0f);//nextState.length);
		ToggleSceneActors(true);
		dialogueBorders.SetActive(false);
	}

	IEnumerator FadeOutDialogueToSceneEnd(Animator dialogueAnimator) {
		dialogueAnimator.SetTrigger("FadeOut");
		// var nextState = dialogueAnimator.GetNextAnimatorStateInfo(0);
		yield return new WaitForSeconds(1.0f);//nextState.length);
		loadingScreen.SetActive(true);
		SceneManager.LoadScene(GameSceneIndexes.GAME_FINISH_SCENE);
	}
	
}
