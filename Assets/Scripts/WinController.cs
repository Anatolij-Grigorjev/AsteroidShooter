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

	void DoEndText (string text = null)
	{
		if (text != null) {
			winText.text = text;
		}
		winText.enabled = true;
		Time.timeScale = 0.0f;
	}

	public IEnumerator CheckAsteroids() {
		yield return new WaitForSeconds (0.8f);
		currentAsteroids = GameObject.FindGameObjectsWithTag ("Asteroid").Length;
		if (currentAsteroids <= 0) {
			DoEndText ();
		}
		//check loss condition then
		if (!winText.enabled) {
			var ship = GameObject.FindGameObjectWithTag ("Ship");
			if (!ship.GetComponent<SpriteRenderer> ().enabled) {
				DoEndText ("GAME OVER!");
			}
		}
	}
}
