using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinController : MonoBehaviour {

	private float currentAsteroids;
	// Use this for initialization
	private Text winText;
	void Start () {
		currentAsteroids = float.MaxValue;
		winText = GetComponent<Text> ();
		winText.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!winText.enabled) {
			//once every few seconds
			StartCoroutine (CheckAsteroids ());
		}
	}

	public IEnumerator CheckAsteroids() {
		yield return new WaitForSeconds (0.5f);
		currentAsteroids = GameObject.FindGameObjectsWithTag ("Asteroid").Length;
		if (currentAsteroids <= 0) {
			winText.enabled = true;
			Time.timeScale = 0.0f;
		}
	}
}
