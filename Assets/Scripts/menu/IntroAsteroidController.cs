using UnityEngine;
using System.Collections;

public class IntroAsteroidController : MonoBehaviour {


    public float torqueRange;
    [HideInInspector]
    public bool isDead = false;
    public GameObject explosionPrefab;
    private Rigidbody2D rb2d;
    private AudioSource deathExplosionSound;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D> ();
        deathExplosionSound = GetComponent<AudioSource> ();
        //add some random torque
        rb2d.AddTorque(25 + (torqueRange * Random.value));
    }
       

    //die, explode and spawn ministeroids
    public IEnumerator Die() {
        if (deathExplosionSound.isPlaying) {
            yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        }

        if (!isDead) {
            //play sound, spawn boom
            deathExplosionSound.Play ();
            Instantiate (explosionPrefab, transform.position, transform.rotation);
            GetComponent<SpriteRenderer> ().enabled = false;
            isDead = true;
        }
        yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        Destroy (gameObject);
    }
}
