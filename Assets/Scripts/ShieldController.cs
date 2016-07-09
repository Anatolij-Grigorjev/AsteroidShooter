using UnityEngine;
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
	    
        bool keyHeld = Input.GetButton ("Shields") && currentShieldBarValue <= 0.95f;

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

    void OnCollisionEnter2D(Collision2D collision) {
        //dont collide with own ship
        if (collision.gameObject.CompareTag ("Ship")) {
            return;
        }
    }

    void updateShieldSlider (float currentShieldBarValue) {
        shieldChargeBar.value = currentShieldBarValue;
        shieldChargeKnob.enabled = currentShieldBarValue >= 0.05f;
        shieldChargeBg.enabled = currentShieldBarValue < 0.95f;
    }
}
