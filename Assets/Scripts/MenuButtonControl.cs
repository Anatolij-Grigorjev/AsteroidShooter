using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuButtonControl : MonoBehaviour {

    public GameObject loadingImage;

    private List<GameObject> finalizers;

	// Use this for initialization
	void Awake () {
        finalizers = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Asteroid"));
	}
	
    public void LoadLevel() {

        foreach (GameObject asteroid in finalizers) {
            if (asteroid != null) {
                var astCont = asteroid.GetComponent<AsteroidController> ();
                StartCoroutine_Auto (astCont.Die ());
            }
        }

        StartCoroutine_Auto (MoveOnToLoad());

    }

    private IEnumerator MoveOnToLoad() {
        yield return new WaitUntil(
            () => finalizers.TrueForAll(obj => obj == null || obj.GetComponent<AsteroidController>().isDead)
        );
        yield return new WaitForSeconds (0.8f);

        loadingImage.SetActive (true);
        SceneManager.LoadScene (2);
    }
}
