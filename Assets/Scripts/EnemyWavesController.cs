using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyWavesController : MonoBehaviour {

    public Text screenText;
    public List<GameObject> enemiesList;

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
        yield return new WaitForSeconds (wait);
        screenText.enabled = false;
    }

    IEnumerator WaveMaker(int waveIndex) {
        if (GameController.Instance.currentAsteroids == 0) {
            if (!finalWave) {
                var wave = GameController.Instance.LevelWaves [waveIndex];
                waveIndex++;
                DoScreenText (wave ["name"], 5.0f);
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
                //TODO: observe waves
                yield return 0;
            } else {

                //TODO: create WIN
                yield return 0;
            }
        }

        yield return 0;
    }
}
