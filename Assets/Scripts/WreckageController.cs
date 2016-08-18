using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

public class WreckageController : MonoBehaviour {

	// Use this for initialization

    private Animator wreakAnimator;
    public GameObject LoadingScreen;

	void Awake () {
        wreakAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
        bool pressed = !wreakAnimator.enabled && Input.GetButtonUp ("Main Cannon");
        if (pressed) {
//            GameController.Instance.ShipQuipper.spoutSpecificQuip ("Not giving up!", "father");
            wreakAnimator.enabled = true;
        }
	}

    IEnumerator GoNextPhase () {
        yield return new WaitForSeconds (0.2f);
        LoadingScreen.SetActive (true);
        SceneManager.LoadScene (GameSceneIndexes.GAME_SCENE);
    }
}
