using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour {

    private float remainingAsteroids;
	// Use this for initialization
	private Text winText;
	public GameObject tryAgainButton;
    private bool playerWon;
    public Text asteroidsCountText;

	void Start () {
        playerWon = false;
        GameController.Instance.nextSceneIndex = 0;
		remainingAsteroids = float.MaxValue;
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
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	void DoEndText (string text = null)
	{
		if (text != null) {
			winText.text = text;
		}
		winText.enabled = true;
        if (!playerWon) {
            Time.timeScale = 0.0f;
            tryAgainButton.SetActive (true);
        }
	}



	public IEnumerator CheckAsteroids() {
		yield return new WaitForSeconds (1.5f);
        //the game may have just started and the game controller did not properly awake yet
        //having this flag should avoid winning before the whole thing took off
        remainingAsteroids = GameController.Instance.currentAsteroids;
        asteroidsCountText.text = "X " + remainingAsteroids;
        if (remainingAsteroids <= 0) {
            playerWon = true;
            StartCoroutine (GoNextPhase ());
            DoEndText ("YAY!");
        }
        //check loss condition then
        if (!winText.enabled) {
            var ship = GameController.Instance.PlayerShip;
            if (ship.GetComponent<ShipHealth>().health <= 0.0f) {
                playerWon = false;
                DoEndText ("GAME OVER!");
            }
        }

	}

    IEnumerator GoNextPhase () {
        yield return new WaitForSeconds (5.5f);
        SceneManager.LoadScene (2);
    }
}
