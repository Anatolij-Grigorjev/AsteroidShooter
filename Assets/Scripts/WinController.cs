using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour {

	private float currentAsteroids;
	// Use this for initialization
	private Text winText;
	public GameObject tryAgainButton;

	void Start () {
        GameController.Instance.nextSceneIndex = 0;
		currentAsteroids = float.MaxValue;
		winText = GetComponent<Text> ();
		winText.enabled = false;
		tryAgainButton.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!winText.enabled) {
			//once every few seconds
			StartCoroutine (CheckAsteroids ());
		}
	}

	public void ReloadStage() {
		Time.timeScale = 1.0f;
        SceneManager.LoadScene (GameController.Instance.nextSceneIndex);
	}

	void DoEndText (string text = null)
	{
		if (text != null) {
			winText.text = text;
		}
		winText.enabled = true;
		Time.timeScale = 0.0f;
		tryAgainButton.SetActive (true);
	}

	public IEnumerator CheckAsteroids() {
		yield return new WaitForSeconds (0.9f);
        //the game may have just started and the game controller did not properly awake yet
        //having this flag should avoid winning before the whole thing took off

        currentAsteroids = GameController.Instance.currentAsteroids;
        if (currentAsteroids <= 0) {
            DoEndText ();
        }
        //check loss condition then
        if (!winText.enabled) {
            var ship = GameController.Instance.PlayerShip;
            if (ship.GetComponent<ShipHealth>().health <= 0.0f) {
                DoEndText ("GAME OVER!");
            }
        }

	}
}
