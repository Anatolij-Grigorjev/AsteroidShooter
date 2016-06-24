using UnityEngine;
using System.Collections;
using System;

public class BGScrollerController : MonoBehaviour {

    public float scrollSpeed; //speed to scroll background
    public float tileSizeZ; //largest allowed value for position shift before looping

    private Vector3 startPosition;
	
	void Awake () {
        startPosition = transform.position;
	}

    void Update() {

        float newPosition = Mathf.Repeat (Time.time * scrollSpeed, tileSizeZ);
        transform.position = startPosition + Vector3.right * newPosition;


    }
}
