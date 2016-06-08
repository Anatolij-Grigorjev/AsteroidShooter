using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtonControl : MonoBehaviour {

    public GameObject loadingImage;

	// Use this for initialization
	void Awake () {
	
	}
	
    public void LoadLevel() {
        loadingImage.SetActive (true);
        SceneManager.LoadSceneAsync (1);
    }
}
