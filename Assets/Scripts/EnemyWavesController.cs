using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyWavesController : MonoBehaviour {

    public Text screenText;
    public List<GameObject> enemiesList;
    public WinController winControl;

    [HideInInspector]
    public bool finalWave;


	void Awake () {
        finalWave = false;
        StartWaves ();
	}
	
    void StartWaves() {
        StartCoroutine (WaveMaker (0));
    }

    public IEnumerator DoScreenText(string text, float wait) {
        screenText.text = text;
        screenText.enabled = true;
        Debug.Log ("Doing wave " + text);
        yield return new WaitForSeconds (wait);
        screenText.enabled = false;
    }

    IEnumerator WaveMaker(int waveIndex) {
        yield return new WaitForSeconds (1.5f);
        if (GameController.Instance.currentAsteroids == 0) {
            if (!finalWave) {
                var wave = GameController.Instance.LevelWaves [waveIndex];
                yield return DoScreenText (wave ["name"], 3.5f);
                GameController.Instance.currentAsteroids = wave ["placements"].Count;
                for (int i = 0; i < GameController.Instance.currentAsteroids; i++) {
                    var placement = wave ["placements"] [i];
                    Instantiate (
                        enemiesList [placement ["typeIndex"].AsInt], 
                        new Vector3 (placement ["x"].AsFloat, placement ["y"].AsFloat),
                        Quaternion.identity
                    );
                }
                if (wave ["final"] != null) {
                    finalWave = wave ["final"].AsBool;
                }

                StartCoroutine (WaveMaker (waveIndex + 1));
            } else {
                winControl.DoWin ();
            }
        }

        StartCoroutine (WaveMaker (waveIndex));
    }
}
