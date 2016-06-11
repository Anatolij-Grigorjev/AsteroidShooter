using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AnimatedShipController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    public void LoadMenu() {
        SceneManager.LoadScene (1, LoadSceneMode.Additive);
    }
}
