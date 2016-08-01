using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipIndicatorController : MonoBehaviour {

    public SpriteRenderer indicatorSprite;

    private Transform shipTransform;

	void Awake () {
        indicatorSprite.enabled = false;
        shipTransform = transform.parent;
	}
	

    void Update () {
	    //discover enemy closest to our position
        var enemies = GameController.Instance.currentEnemies;
        if (enemies.Count == 0) {
            if (indicatorSprite.enabled) {
                indicatorSprite.enabled = false;
            }
        } else {
            if (!indicatorSprite.enabled) {
                indicatorSprite.enabled = true;
            }
            var shipPosition = shipTransform.position;
            var closeEnemy = enemies[0];
            var shortestDistance = Utils.DistanceFromTo(shipPosition, enemies[0].transform.position);
            for(int i = 1; i < enemies.Count; i++) {
                var dist = Utils.DistanceFromTo (shipPosition, enemies [i].transform.position);
                if (dist < shortestDistance) {
                    closeEnemy = enemies [i];
                    shortestDistance = dist;
                }
            }

            //rotate indicator to that enemy
            var screenPoint = Camera.main.WorldToScreenPoint(closeEnemy.transform.position);
            var offset = new Vector2(screenPoint.x, screenPoint.y);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            var wantedRotation = Quaternion.Euler (0, 0, angle);

            transform.rotation = wantedRotation;
        }
	}
}
