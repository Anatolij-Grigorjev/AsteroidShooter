using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldController : MonoBehaviour {

    public Animator shieldAnimations;
    public SpriteRenderer shieldGraphics;
    public Slider shieldChargeBar;
    public Image shieldChargeKnob;
    public float shieldConsumptionPerSec = 0.1f;
    public float shieldRegenPerSec = 0.05f;

    private float currentShieldBarValue = 0.0f;
    private bool isUsed = false;

	// Use this for initialization
	void Awake () {
        updateShieldSlider (currentShieldBarValue);
        shieldAnimations.enabled = false;
        shieldGraphics.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	    
        bool keyHeld = Input.GetKey (KeyCode.E);

        if (keyHeld) {
            //player is holding down the shield key
            if (isUsed) {
                //shield was already in use
                //TODO: drain shield bar, update animations
            } else {
                //shield was not in use
                //TODO: setup shield use states, start animation
            }
        } else {
            //player not holding down the shield key
            if (isUsed) {
                //shield was in use
                //TODO: stop draining shild, give it quit animation
            } else {
                //button not held and shield not in use - business as usual
                //TODO: regenerate shield, if required
            }
        }
	}

    void updateShieldSlider (float currentShieldBarValue) {
        currentShieldBarValue = Mathf.Clamp (currentShieldBarValue, 0.0f, 1.0f);
        shieldChargeBar.value = currentShieldBarValue;
        shieldChargeKnob.enabled = currentShieldBarValue > 0.15f;
    }
}
