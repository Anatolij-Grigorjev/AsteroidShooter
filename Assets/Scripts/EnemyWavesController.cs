using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyWavesController : MonoBehaviour {

    public Text screenText;
    public Text asteroidsCountText;
    public List<GameObject> enemiesList;
    public WinController winControl;

    [HideInInspector]
    public bool finalWave;

    private int waveIndex;

	void Awake () {
        finalWave = false;
        waveIndex = 0;
        StartWaves ();
	}
	
    void StartWaves() {
        StartCoroutine (WaveMaker ());
    }

    public IEnumerator DoScreenText(string text, float wait) {
        screenText.text = text;
        screenText.enabled = true;
        Debug.Log ("Doing wave " + text);
        yield return new WaitForSeconds (wait);
        screenText.enabled = false;
    }

    IEnumerator WaveMaker() {
        yield return new WaitForSeconds (1.4f);
        asteroidsCountText.text = "X " + GameController.Instance.currentAsteroids;
        if (GameController.Instance.currentAsteroids <= 0) {
            if (!finalWave) {
                Debug.Log ("Not final wave ended, time for wave " + waveIndex);
                var wave = GameController.Instance.LevelWaves [waveIndex];
                yield return DoScreenText (wave ["name"], 3.5f);
                waveIndex++;
                GameController.Instance.currentAsteroids = wave ["placements"].Count;
                asteroidsCountText.text = "X " + GameController.Instance.currentAsteroids;
                if (wave["quip"] != null) {
                    var quip = wave ["quip"];
                    GameController.Instance.ShipQuipper.spoutSpecificQuip (quip ["text"], quip ["avatar"]);
                }
                for (int i = 0; i < GameController.Instance.currentAsteroids; i++) {
                    var placement = wave ["placements"] [i];
                    var origX = placement ["x"].AsFloat;
                    var origY = placement ["y"].AsFloat;
                    Instantiate (
                        enemiesList [placement ["typeIndex"].AsInt], 
                        //somewhat random position
                        new Vector3 (
                            origX + Random.Range (-0.1f * origX, 0.1f * origX),
                            origY + Random.Range (-0.1f * origY, 0.1f * origY)
                        ),
                        Quaternion.identity
                    );
                }
                if (wave ["final"] != null) {
                    finalWave = wave ["final"].AsBool;
                }
            } else {
                Debug.Log ("Player won, doing win");
                winControl.DoWin ();
            }
        }

        StartCoroutine (WaveMaker ());
    }
}
