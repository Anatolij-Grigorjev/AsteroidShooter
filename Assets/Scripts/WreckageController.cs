using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

public class WreckageController : MonoBehaviour {

	// Use this for initialization

    private Animator wreakAnimator;
    public GameObject LoadingScreen;
    private GameOverSceneController gameOverSceneController;
	void Awake () {
        wreakAnimator = GetComponent<Animator> ();
        gameOverSceneController = GameController.Instance.SceneManager.GetComponent<GameOverSceneController>();
	}
	
	// Update is called once per frame
	void Update () {
        bool pressed = !wreakAnimator.enabled && Input.GetButtonUp ("Main Cannon");
        if (pressed) {
//            GameController.Instance.ShipQuipper.spoutSpecificQuip ("Not giving up!", "father");
            wreakAnimator.enabled = true;
        }
	}

    public void SetupRestartLevel() {
        GameController.Instance.Restart = true;
    }

    public void ToggleAudioSource(int source) {
        
        switch(source) {
            case 0:
                gameOverSceneController.shipAssembleSound.enabled = !gameOverSceneController.shipAssembleSound.enabled;
                break;
            case 1:
                gameOverSceneController.shipEngineStartSound.enabled = !gameOverSceneController.shipEngineStartSound.enabled;
                break;
            case 2:
                gameOverSceneController.shipEngineGoSound.enabled = !gameOverSceneController.shipEngineGoSound.enabled;
                break;
            default:
                gameOverSceneController.backgroundMusicSound.enabled = !gameOverSceneController.backgroundMusicSound.enabled;
                break;
        }
    }

    IEnumerator GoNextPhase () {
        Debug.Log("About to load scene at index: " + GameController.Instance.SceneIndex);
        yield return new WaitForSeconds (0.2f);
        LoadingScreen.SetActive (true);
        SceneManager.LoadScene (GameController.Instance.SceneIndex);
    }
}
