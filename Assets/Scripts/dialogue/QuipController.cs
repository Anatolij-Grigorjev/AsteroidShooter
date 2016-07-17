using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class QuipController : MonoBehaviour {

    public GameObject quipPrefab;
    public float quipsCooldown; // how many seconds minimum between quips
    private float currTimer;
    private bool quipReady;
    private Dictionary<QuipTypes, List<Tuple<String, String>>> quipsMap;
    private Dictionary<QuipTypes, Boolean> quipUsedMap;

	void Awake () {
        quipsMap = new Dictionary<QuipTypes, List<Tuple<String, String>>> ();
        quipsMap.Add (QuipTypes.QUIP_FIRED_ROMBUS,
            new List<Tuple<String,String>> {
                new Tuple<String, String>("daughter", "What's this button?"),
                new Tuple<String, String>("father", "Eat rombus, rocks!"),
                new Tuple<String, String>("father", "Try this hot potato!"),
            });
        quipsMap.Add (QuipTypes.QUIP_SHIELD_DEPLETED,
            new List<Tuple<String, String>> {
                new Tuple<String, String>("father", "There goes the shield...")
            });
        quipUsedMap = new Dictionary<QuipTypes, bool> ();
        foreach (QuipTypes type in Enum.GetValues(typeof(QuipTypes))) {
            quipUsedMap.Add (type, false);
        }
        currTimer = 0.0f;
        quipReady = true;
	}

    public void FixedUpdate() {
        if (!quipReady) {
            currTimer += Time.fixedDeltaTime;
            if (currTimer >= quipsCooldown) {
                currTimer = 0.0f;
                quipReady = true;
            }
        }
    }
	
    public void spoutRandomQuip(QuipTypes type) {
        if (quipReady && !quipUsedMap [type]) {
            var selection = quipsMap [type];
            //first tuple item is key to avatar, second is the quip text itself
            var chosenQuip = selection [UnityEngine.Random.Range (0, selection.Count)];
            var offset = new Vector3 (-1.0f, -2.5f, 0.0f);
            var quipObject = Instantiate (quipPrefab, offset + transform.position, Quaternion.identity) as GameObject;
            var textMesh = quipObject.GetComponentInChildren<TextMesh> ();
            textMesh.text = chosenQuip.second;

            var avatarSprite = Utils.GetComponentInChild<SpriteRenderer> (quipObject.transform);
            avatarSprite.sprite = GameController.Instance.GetAvatar (chosenQuip.first);

            var followScript = quipObject.AddComponent<HPFollow> ();
            followScript.offset = offset;
            followScript.keepChecking = false;
            followScript.thing = gameObject.transform;

            StartCoroutine (QuipDissapear (quipObject));
            //taking damage is always quippable
            if (type != QuipTypes.QUIP_TAKEN_DAMAGE) {
                quipUsedMap [type] = true;
            }
            quipReady = false;
        } 
    }

    IEnumerator QuipDissapear (GameObject quipObject) {
        var length = quipObject.GetComponentInChildren<TextMesh> ().text.Length;
        yield return new WaitForSeconds (0.2f * length);
        Destroy (quipObject);
    }
}
