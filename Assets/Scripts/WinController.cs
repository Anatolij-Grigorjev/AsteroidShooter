using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssemblyCSharp;

public class WinController : MonoBehaviour {
    public const int DIALOGUE_SCENE = 2;

	// Use this for initialization
	private Text winText;
    private bool playerWon;

	void Awake () {
        playerWon = false;
        GameController.Instance.nextSceneIndex = 0;
		winText = GetComponent<Text> ();

        StartCoroutine (CheckAsteroids ());
	}

	void DoEndText (string text = null)
	{
		if (text != null) {
			winText.text = text;
		}
		winText.enabled = true;
        if (!playerWon) {
            StartCoroutine (GoNextPhase (GameSceneIndexes.GAME_OVER_SCENE));
        }
	}

    public void DoWin () {
        playerWon = true;
        StartCoroutine (GoNextPhase (GameSceneIndexes.DIALOGUE_SCENE));
        DoEndText ("Area clear");
    }


	public IEnumerator CheckAsteroids() {
		yield return new WaitForSeconds (1.5f);
        //check loss condition then
        if (!winText.enabled) {
            var ship = GameController.Instance.PlayerShip;
            if (ship.GetComponent<ShipHealthController>().health <= 0.0f) {
                yield return new WaitUntil (() => ship.GetComponent<ShipHealthController> ().isDead);
                playerWon = false;
                DoEndText (
                    RandomDissapointments.GAME_OVER_DARNS[Random.Range(0, RandomDissapointments.GAME_OVER_DARNS.Length)]
                );
            }
            //no check hit its mark, we can keep checking asteroids
            if (!winText.enabled) {
                StartCoroutine (CheckAsteroids ());
            }
        }
	}

    IEnumerator GoNextPhase (int nextPhase) {
        yield return new WaitForSeconds (5.5f);
        SceneManager.LoadScene (nextPhase);
    }
}
