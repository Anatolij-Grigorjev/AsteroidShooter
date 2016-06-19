using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class AnimatedShipController : MonoBehaviour {


    private Animator flyInController;
    private bool skippedIntro = false;
	// Use this for initialization
	void Awake () {
        flyInController = GetComponent<Animator> ();
	}
	
    public void LoadMenu() {
        if (!skippedIntro) {
            skippedIntro = true;
        }
        GetComponent<AudioSource> ().enabled = false;
        SceneManager.LoadScene (1, LoadSceneMode.Additive);
    }

    void Update() {

        if (!skippedIntro) {
            if (Input.anyKey) {
                skippedIntro = true;
                flyInController.SetBool ("Pressed", true);

                LoadMenu ();
            }
        }
    }
}
