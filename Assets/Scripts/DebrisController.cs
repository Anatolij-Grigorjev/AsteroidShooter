using UnityEngine;
using System.Collections;
using System;
using Microsoft.Win32;

public class DebrisController : MonoBehaviour {

    public float speedRange;
    public float torqueRange;
    private float speed;
    public GameObject explosionPrefab;
    private Rigidbody2D rb2d;
    private AudioSource deathExplosionSound;

    private float coefA;
    private float coefB;
    private float coefC;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D> ();
        deathExplosionSound = GetComponent<AudioSource> ();
        //add some random torque
        rb2d.AddTorque(torqueRange * UnityEngine.Random.value);

        speed = speedRange * UnityEngine.Random.value;
        StartCoroutine (Die (5.0f));
    }

    public void SetCoef(float a, float b, float c) {

        coefA = a;
        coefB = b;
        coefC = c;
    }

    // Update is called once per frame
    void Update () {
        var x = transform.position.x;
        transform.position = new Vector3 (x + speed * Time.deltaTime, x*x* coefA + coefB * x + coefC);
       
    }

    //die, explode and spawn ministeroids
    public IEnumerator Die(float waitTime) {
        yield return new WaitForSeconds (waitTime);
        //play sound, spawn boom
        deathExplosionSound.Play ();
        Instantiate (explosionPrefab, transform.position, transform.rotation);
        GetComponent<SpriteRenderer> ().enabled = false;
        yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        Destroy (gameObject);
    }
}
