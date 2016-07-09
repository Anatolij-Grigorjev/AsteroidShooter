﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldController : MonoBehaviour {

    public Animator shieldAnimations;
    public SpriteRenderer shieldGraphics;
    public Slider shieldChargeBar;
    public Image shieldChargeKnob;
    public Image shieldChargeBg;
    public CircleCollider2D shieldCollider;
    public float shieldConsumptionPerSec = 0.09f;
    public float shieldRegenPerSec = 0.08f;
    //how much damage does the shield inflict by touching others
    public float shieldDamage = 1.5f;

    private float currentShieldBarValue = 1.0f;
    private bool isUsed = false;
    //accumulator for seconds to slow down shield animations
    private float secAccumulator = 0.0f;

	// Use this for initialization
	void Awake () {
        updateShieldSlider (currentShieldBarValue);
        shieldAnimations.enabled = false;
        shieldGraphics.enabled = false;
        shieldCollider.enabled = isUsed;
	}
	
	// Update is called once per frame
	void Update () {
	    
        bool keyHeld = Input.GetButton ("Shields") && currentShieldBarValue <= 0.90f;

        if (keyHeld) {
            //player is holding down the shield key
            if (isUsed) {
                //shield was already in use
                secAccumulator += Time.deltaTime;
                if (secAccumulator >= 1.0f) {
                    currentShieldBarValue += shieldConsumptionPerSec;
                    secAccumulator = 0.0f;
                }
            } else {
                //shield was not in use
                isUsed = true;
                secAccumulator = 0.0f;
                shieldAnimations.enabled = true;
                shieldGraphics.enabled = true;
                shieldAnimations.SetBool ("isDrained", false);
                shieldAnimations.Play ("Charging");
            }
        } else {
            //player not holding down the shield key
            if (isUsed) {
                //shield was in use
                isUsed = false;
                secAccumulator = 0.0f;
                shieldAnimations.SetBool ("isDrained", true);
//                StartCoroutine (DisableAnimator ());
            } else {
                //button not held and shield not in use - business as usual
                secAccumulator += Time.deltaTime;
                if (secAccumulator >= 1.0f) {
                    secAccumulator = 0.0f;
                    currentShieldBarValue -= shieldRegenPerSec;
                }
            }
        }
        //either way this gets updated
        shieldCollider.enabled = isUsed;
        currentShieldBarValue = Mathf.Clamp (currentShieldBarValue, 0.0f, 1.0f);
        updateShieldSlider (currentShieldBarValue);
	}

    IEnumerator DisableAnimator () {
        var clipLength = shieldAnimations.GetCurrentAnimatorStateInfo (0).length;
        yield return new WaitForSeconds (clipLength);
        shieldGraphics.enabled = false;
        shieldAnimations.SetBool ("isDrained", false);
        shieldAnimations.enabled = false;
    }

//    void OnTriggerEnter2D(Collider2D collider) {
//        if (collider.gameObject.CompareTag ("Ship")) {
//            //dont collide with own ship
//            return;
//        }
//        if (collider.gameObject.CompareTag("Asteroid")) {
//            //if collision is with asteroid, provide chip damage and make it fly away
//            var rigidBody = collider.gameObject.GetComponent<Rigidbody2D> ();
//            rigidBody.AddForce (-1 * rigidBody.velocity * rigidBody.mass);
//            var asteroidHealthScript = collider.gameObject.GetComponent<AsteroidHealth> ();
//            asteroidHealthScript.TakeDamage (transform, shieldDamage);
//        }
//        if (collider.gameObject.CompareTag ("Debris")) {
//            //if the collision is debris, just make it explode then and there
//            var debrisControllerScript = collider.gameObject.GetComponent<DebrisController> ();
//            StopCoroutine (debrisControllerScript.deathCoroutine);
//            StartCoroutine (debrisControllerScript.Die (0.0f));
//        }
//    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag ("Ship")) {
            //dont collide with own ship
            return;
        }
        if (collision.gameObject.CompareTag("Asteroid")) {
            //if collision is with asteroid, provide chip damage and make it fly away
            var rigidBody = collision.gameObject.GetComponent<Rigidbody2D> ();
            rigidBody.AddForce (-1 * rigidBody.velocity * rigidBody.mass);
            var asteroidHealthScript = collision.gameObject.GetComponent<AsteroidHealth> ();
            asteroidHealthScript.TakeDamage (transform, shieldDamage);
        }
        if (collision.gameObject.CompareTag ("Debris")) {
            //if the collision is debris, just make it explode then and there
            var debrisControllerScript = collision.gameObject.GetComponent<DebrisController> ();
            StopCoroutine (debrisControllerScript.deathCoroutine);
            StartCoroutine (debrisControllerScript.Die (0.0f));
        }
    }

    void updateShieldSlider (float currentShieldBarValue) {
        shieldChargeBar.value = currentShieldBarValue;
        shieldChargeKnob.enabled = currentShieldBarValue >= 0.05f;
        shieldChargeBg.enabled = currentShieldBarValue < 0.95f;
    }
}
