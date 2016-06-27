using UnityEngine;
using System.Collections;
using System;
using Microsoft.Win32;

public class DebrisController : MonoBehaviour {

    public float speedRange;
    public float torqueRange;
    private float speed;
    public GameObject explosionPrefab;
    private Collider2D theCollider;
    private AudioSource deathExplosionSound;

    private float coefA;
    private float coefB;
    private float coefC;

    Coroutine deathCoroutine;

    // Use this for initialization
    void Start () {
        theCollider = GetComponent<Collider2D> ();
        deathExplosionSound = GetComponent<AudioSource> ();
        //add some random torque
//        rb2d.AddTorque(torqueRange * UnityEngine.Random.value);

        speed = speedRange;
        deathCoroutine = StartCoroutine (Die (1.5f));
        StartCoroutine (becomeCollider ());
    }

    public void SetCoef(float a, float b, float c) {

        coefA = a;
        coefB = b;
        coefC = c;
    }

    IEnumerator becomeCollider() { 
        yield return new WaitForSeconds (0.2f);
        theCollider.isTrigger = false;

        yield return 0;
    }

    // Update is called once per frame
    void Update () {
        var x = transform.position.x;
        transform.position = new Vector3 (x + speed * Time.deltaTime * coefA, coefB * x + coefC);
        speed = Mathf.Clamp (speed - 0.01f, 0.0f, speed);
    }

    //die, explode and spawn ministeroids
    public IEnumerator Die(float waitTime) {
        if (waitTime > 0.0f) {
            yield return new WaitForSeconds (waitTime);
        }
        //play sound, spawn boom
        deathExplosionSound.Play ();
        Instantiate (explosionPrefab, transform.position, transform.rotation);
        GetComponent<SpriteRenderer> ().enabled = false;
        yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        Destroy (gameObject);
    }
}
