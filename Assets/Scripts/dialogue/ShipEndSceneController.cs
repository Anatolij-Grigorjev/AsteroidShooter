using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShipEndSceneController : MonoBehaviour {
    
    private AudioSource shipEngineSound;
    public GameObject LoadingScreen;

	// Use this for initialization
	void Awake () {
        shipEngineSound = GetComponent<AudioSource> ();
	}
	
    public void KillEnginesAndFinishScene() {
        shipEngineSound.enabled = false;
        StartCoroutine (GoNextPhase ());
    }

    IEnumerator GoNextPhase () {
        yield return new WaitForSeconds (0.2f);
        LoadingScreen.SetActive (true);
        SceneManager.LoadScene (GameController.Instance.nextSceneIndex);
    }
}
