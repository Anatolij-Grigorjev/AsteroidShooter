using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking.NetworkSystem;

public class EnemyIndicatorController : MonoBehaviour {

    public List<GameObject> borderColliders;
    public List<GameObject> indicatorPrefabs;
    //list of indicators employed by collider
    private Dictionary<GameObject, List<GameObject>> currentIndicators;
    //tally of how many indicators currently needed for the collider side
    private Dictionary<GameObject, int> currentColliderTally;

	void Awake () {
        currentIndicators = new Dictionary<GameObject, List<GameObject>> (borderColliders.Count);
        currentColliderTally = new Dictionary<GameObject, int> (borderColliders.Count);
        for (int i = 0; i < borderColliders.Count; i++) {
            currentIndicators.Add (borderColliders [i], new List<GameObject> ());
            currentColliderTally.Add (borderColliders [i], 0);
        }
	}
	

    void Update () {
	    //discover enemy closest to our position
        var enemies = GameController.Instance.currentEnemies;
        //hide all indicators before the frame starts
        foreach (GameObject border in borderColliders) {
            currentColliderTally [border] = 0;
        }
        HideSurplusIndicators ();

        if (enemies.Count != 0) {
            //TODO: hit borders with rays to enemies, group by border type, show needed count of indicators
            var cameraPosition = transform.position;
            foreach (GameObject enemy in enemies) {
                var enemyPosition = enemy.transform.position;
                var cast = Physics2D.Linecast (cameraPosition, enemyPosition);
                Debug.DrawLine (cameraPosition, enemyPosition);
                //fetch indicators for that border

                if (cast.collider != null && cast.collider.gameObject.CompareTag("Borders")) {
                    var indicators = currentIndicators [cast.collider.gameObject];
                    currentColliderTally [cast.collider.gameObject]++;
                    var closestFree = closestFreeIndicator (indicators);
                    //no free indicators available
                    if (closestFree == null) {
                        closestFree = Instantiate (
                            indicatorPrefabs [borderColliders.IndexOf (cast.collider.gameObject)], 
                            Vector3.zero,
                            Quaternion.identity
                        ) as GameObject;
                        indicators.Add (closestFree);
                    }
                    closestFree.GetComponent<SpriteRenderer> ().enabled = true;
                    closestFree.transform.position = new Vector3 (cast.point.x, cast.point.y);
                }
            }
        }
//        HideSurplusIndicators ();
	}

    void HideSurplusIndicators () {
        //disable all previously enabled indicators for this collider
        foreach (GameObject border in borderColliders) {
            for (int i = currentColliderTally [border]; i < currentIndicators [border].Count; i++) {
                var renderer = currentIndicators [border] [i].GetComponent<SpriteRenderer> ();
                renderer.enabled = false;
            }
        }
    }

    /* return closest non visible indicator (means not in active use)
     * 
     * */
    GameObject closestFreeIndicator (List<GameObject> indicators) {

        if (indicators == null || indicators.Count == 0) {
            return null;
        }
        for (int i = 0; i < indicators.Count; i++) {
            var indicatorRenderer = indicators [i].GetComponent<SpriteRenderer>();
            if (!indicatorRenderer.enabled) {
                return indicators [i];
            }
        }

        return null;
    }
}
