﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShootBullet : MonoBehaviour {

	public float bulletRecharge;
	public float frenzyBulletRecharge;
	private float lastShot;
	public GameObject bulletPrefab;
	public GameObject rombusBulletPrefab;


	private SpriteRenderer shipImage;

	private AudioSource shotClip;
	// Use this for initialization
	public AudioSource chargeClip;
	public AudioSource chargedClip;

	public float rombusRefactoryPeriod = 3.0f;
	private bool rombusRecovered;
	private float frenzyBuildUp, pressTime = 0f;
	public float chargeReqTime = 2.0f;
	private bool charging = false;
	private bool charged = false;
	private bool inFrenzy = false;
	//contain excitement to seconds
	public float maxFrenzyCharge = 0.0f;

	public SpriteRenderer chargeGraphics;
	public Animator chargeAnimator;

	//rombus sprite for effect
	public SpriteRenderer rombusRenderer;
	//how many max rombus can the ship fire
	public int currRombusCount;

	public Image rombusCornerImage;
	public Text rombusCornerCount;


	void Awake () {
		lastShot = 0;
		shipImage = GetComponent<SpriteRenderer> ();
		shotClip = GetComponentInChildren<AudioSource> ();
		rombusRecovered = true;
	
		rombusCornerCount.text = "X " + currRombusCount;
		chargeAnimator.enabled = false;
		chargeGraphics.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (inFrenzy) {
			//ignotre shooting controls, discharge color and bullets
			PerformShot(frenzyBulletRecharge);
			var color = shipImage.color;
			color.g	= Mathf.Clamp (color.g + Time.deltaTime / 2.0f, 0, 1.0f);
			shipImage.color = color;
			frenzyBuildUp -= Time.deltaTime;
			frenzyBuildUp = Mathf.Clamp (frenzyBuildUp, 0, float.MaxValue);

			if (frenzyBuildUp == 0) {
				inFrenzy = false;
			}
		} else {
			//or perform regular shot
			ChargeFrenzy ();
		}
		//recover rombus attack
		if (!rombusRecovered) {
			RombusRecovery ();
		} else {
			//or try shooting it if recovered
			ShootRombus ();
		}
	}

	void RombusRecovery() {
		var color = rombusRenderer.color;
		color.a += ((1.0f / rombusRefactoryPeriod) * Time.deltaTime);
		color.a = Mathf.Clamp (color.a, 0.0f, 1.0f);
		rombusRenderer.color = color;
		//update corner
		rombusCornerImage.color = color;

		rombusRecovered = color.a >= 1.0f || currRombusCount == 0;
	}

	void ShootRombus() {
		//only the rombus was pressed
		if (Input.GetButton ("Fire2") && !Input.GetButton("Fire1") && currRombusCount > 0) {
			var c = rombusRenderer.color;
			c.a = 0.0f;
			rombusRenderer.color = c;
			//update corner
			rombusCornerImage.color = c;
			currRombusCount--;
			rombusCornerCount.text = "X " + currRombusCount;

			Instantiate (rombusBulletPrefab, transform.position, Quaternion.identity);
			rombusRecovered = false;
		}
	}

	void ChargeFrenzy ()
	{
		bool buttonPressed = Input.GetButton ("Fire1");
		//stopped hoding button
		if (!buttonPressed) {
			//held long enough for frenzy
			if (charged) {
				frenzyBuildUp = pressTime;
				inFrenzy = true;
				ResetWeaponChargeState ();

			}
			//button was held down, just not long enough for frenzy, single shot
			else
				if (pressTime > 0) {
					ResetWeaponChargeState ();
					var color = shipImage.color;
					color.g = 1.0f;
					shipImage.color = color;
					PerformShot (bulletRecharge);
				}
		}
		else {
			pressTime += Time.deltaTime;
			pressTime = Mathf.Clamp (pressTime, 0.0f, maxFrenzyCharge);
			//pressed max charge
			if (pressTime >= maxFrenzyCharge && !charged) {
				charged = true;
				chargedClip.Play ();
				chargeAnimator.SetBool ("isCharged", true);
				var pos = chargeGraphics.transform.position;
				pos.y = 0.92f;
				chargeGraphics.transform.position = pos;
			}
			//pressed long enough
			if (pressTime >= chargeReqTime && !charging) {
				charging = true;
				chargeClip.Play ();
				chargeAnimator.enabled = true;
			}
			if (charging) {
				//color the ship (initial green is 255)
				var color = shipImage.color;
				color.g = Mathf.Clamp (color.g - Time.deltaTime / 2.0f, 0, 1.0f);
				shipImage.color = color;
			}
		}
	}

	void ResetWeaponChargeState ()
	{
		charging = false;
		charged = false;
		pressTime = 0f;
		chargeAnimator.enabled = false;
		chargeGraphics.enabled = false;
		chargeAnimator.SetBool ("isCharged", false);
		var pos = chargeGraphics.transform.position;
		pos.y = 0.0f;
		chargeGraphics.transform.position = pos;
	}

	void PerformShot (float recharge) {
		if (Time.time > recharge + lastShot) {
			Vector3 position = new Vector3 (//position is set by rotating nozzle of aircraft
			//+90 becuase nozzle 90 degrees misplaced from rotation origin
			transform.position.x + (shipImage.bounds.extents.x * Mathf.Cos (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + 90)))
				, transform.position.y + (shipImage.bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + 90)))
				, transform.position.z
			);
			Instantiate (bulletPrefab, position, Quaternion.identity);
			shotClip.Play ();
			lastShot = Time.time;
		}
	}
}