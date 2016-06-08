using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController> {

	protected GameController() {}

    public GameObject playerShip;
    public GameObject background;

    public Transform elementToScatter;
    public int scatterAmount;
    [HideInInspector]
    public int currentAsteroids;

	// Use this for initialization
	void Awake () {
        var backgroundBounds = background.GetComponent<SpriteRenderer> ();

        //create asteroid prefabs around the expanse of space
        for (int i = 0; i < scatterAmount; i++) {
            float multiplierX = Random.value < 0.5 ? -1 : 1;
            float multiplierY = Random.value < 0.5 ? -1 : 1;
            Instantiate (elementToScatter, new Vector3 (
                Random.value * multiplierX * backgroundBounds.bounds.extents.x
                , Random.value * multiplierY * backgroundBounds.bounds.extents.y
            ), Quaternion.identity);
        }

        currentAsteroids = scatterAmount;
	}
}
