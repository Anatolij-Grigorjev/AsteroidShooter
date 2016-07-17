using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object
	public SpriteRenderer background; //reference to background to get area bounds
    public AudioSource transitionSound; //sound made during zooming transition

	private float mapX; //stuff to make the camera bound to the bounds of the background
	private float mapY;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;

    private Camera theCamera;
    private Boolean isViewingMode;
	private Vector3 offset;         //Private variable to store the offset distance between the player and camera

	// Use this for initialization
	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = transform.position - player.transform.position;

		mapX = background.bounds.extents.x * 2;
		mapY = background.bounds.extents.y * 2;

		var vertExtent = Camera.main.orthographicSize;    
		var horzExtent = vertExtent * Screen.width / Screen.height;

		// Calculations assume map is position at the origin
		minX = (float)(horzExtent - mapX / 2.0);
		maxX = (float)(mapX / 2.0 - horzExtent);
		minY = (float)(vertExtent - mapY / 2.0);
		maxY = (float)(mapY / 2.0 - vertExtent);
        isViewingMode = false;
        theCamera = GetComponent<Camera> ();
	}

    void Update() {
        bool pressed = Input.GetButtonUp ("Map View");
        if (pressed) {
            transitionSound.Play ();
            isViewingMode = !isViewingMode;
            Time.timeScale = isViewingMode ? 0.0f : 1.0f;
            theCamera.orthographicSize = isViewingMode? 15 : 5;
            transform.position = new Vector3 ();
        }
    }

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		transform.position = player.transform.position + offset;

		var v3 = transform.position;
		v3.x = Mathf.Clamp(v3.x, minX, maxX);
		v3.y = Mathf.Clamp(v3.y, minY, maxY);
		transform.position = v3;
	}
}