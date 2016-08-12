using UnityEngine;
using System.Collections;

public class QuitController : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton ("Cancel")) {
            DoQuit();
        }
	}

    void DoQuit () {
        Application.Quit ();
    }
}
