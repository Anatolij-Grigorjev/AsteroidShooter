using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {


	public AnimationClip boomClip;
	// Use this for initialization
	void Start () {
		
		StartCoroutine_Auto (LetDie ());
	}

	IEnumerator LetDie() {
		yield return new WaitForSeconds(boomClip.length);
		Destroy (gameObject);
	}
		
}
