using UnityEngine;
using System.Collections;

public class ShipEngineController : MonoBehaviour {

    private SpriteRenderer engineSprite;
    private Animator engineAnimator;
    private AudioSource engineSound;

	// Use this for initialization
	void Awake () {
        engineSprite = GetComponent<SpriteRenderer> ();
        engineSound = GetComponent<AudioSource> ();
        engineAnimator = GetComponent<Animator> ();
        engineSprite.enabled = false;
	}
	
	/**
     * Perform ship engine related animation-showing sound-playing 
     * based on current thrust amplitude
     *
     **/
    public void ProcessThrust (float amount) {

        if (Mathf.Abs (amount) > 0 && !engineSound.isPlaying) {
            engineSound.Play ();
        }
        if (Mathf.Abs (amount) == 0 && engineSound.isPlaying) {
            engineSound.Stop ();
        }

        engineAnimator.SetBool ("isEngaged", Mathf.Abs (amount) > 0);
        engineSprite.enabled = engineAnimator.GetBool ("isEngaged");

	    
	}
}
