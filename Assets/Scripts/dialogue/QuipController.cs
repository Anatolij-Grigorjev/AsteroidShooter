using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class QuipController : MonoBehaviour {

    public GameObject quipPrefab;
    public float quipsCooldown; // how many seconds minimum between quips
	
    private Dictionary<QuipTypes, List<Tuple<String, String>>> quipsMap;


	void Awake () {
        quipsMap = new Dictionary<QuipTypes, List<Tuple<String, String>>> ();
        quipsMap.Add (QuipTypes.QUIP_FIRED_ROMBUS,
            new List<Tuple<String,String>> {
                new Tuple<String, String>("father", "Testing...")
            });


	}
	
    public void spoutRandomQuip(QuipTypes type) {
        var selection = quipsMap [type];
        //first tuple item is key to avatar, second is the quip text itself
        var chosenQuip = selection [UnityEngine.Random.Range (0, selection.Count)];

        var quipObject = Instantiate (quipPrefab, transform.position, Quaternion.identity) as GameObject;
        var textMesh = quipObject.GetComponentInChildren<TextMesh> ();
        textMesh.text = chosenQuip.second;

        var avatarSprite = Utils.GetComponentInChild<SpriteRenderer> (quipObject.transform);
        avatarSprite.sprite = GameController.Instance.avatarsMap [chosenQuip.first];

        StartCoroutine (QuipDissapear (quipObject));
    }

    IEnumerator QuipDissapear (GameObject quipObject) {
        var length = quipObject.GetComponent<TextMesh> ().text.Length;
        yield return new WaitForSeconds (0.5f * length);
        Destroy (quipObject);
    }
}
