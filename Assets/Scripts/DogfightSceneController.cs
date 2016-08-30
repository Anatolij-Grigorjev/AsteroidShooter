using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DogfightSceneController : MonoBehaviour {

	public GameObject playerShip;
	public GameObject policeShip;
	public Camera mainCamera;
	public GameObject dialogueBorders;
	public List<string> sceneScriptNames;
	private int INTRO_SCRIPT_INDEX = 0; //scene start script location in script names
	private int POLICE_WIN_SCRIPT_INDEX = 1; //script for police apprehending Turnip
	private int SHIP_WIN_SCRIPT_INDEX = 2; //script for Turnip slipping away from police
	private DialogueFlowController dialogueFlowController;
	// Use this for initialization
	private int currentScriptIndex;
	void Awake () {
		//begin by disabling everything
		ToggleSceneActors(false);
		currentScriptIndex = INTRO_SCRIPT_INDEX;
		dialogueFlowController = GetComponent<DialogueFlowController>();
		dialogueFlowController.ResetForScript(sceneScriptNames[currentScriptIndex]);

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
		if (dialogueFlowController.dialogueOver) {
			if (dialogueBorders.activeInHierarchy) {
				var dialogueAnimator = dialogueBorders.GetComponent<Animator>();
				var currState = dialogueAnimator.GetCurrentAnimatorStateInfo(0);

				//still in fade in state after dialogue over
				if (currState.IsName("FadeInDialogue")) {
					StartCoroutine(FadeOutDialogue(dialogueAnimator));
				}
			}
		}
	}

	IEnumerator FadeOutDialogue(Animator dialogueAnimator) {
		dialogueAnimator.SetTrigger("FadeOut");
		// var nextState = dialogueAnimator.GetNextAnimatorStateInfo(0);
		yield return new WaitForSeconds(1.0f);//nextState.length);
		ToggleSceneActors(true);
		dialogueBorders.SetActive(false);
	}
}
