﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class ShipShootingController : MonoBehaviour {

    //PUBLIC BULLET PARAMS
    public float bulletRecharge = 0.25f;        //cooldown length between regular shots
    public float frenzyBulletRecharge = 0.04f;  //cooldown length between frenzy shots
    public float chargeReqTime = 2.5f;          //required amount of charge for frenzy
    public float maxFrenzyCharge = 2.5f;        //Max amount of time frenzy is charged for
    public int bulletsAmmo = 300;               //initial bullets count
    public GameObject bulletPrefab;
    public GameObject frenzyBulletPrefab;
    public GameObject rombusBulletPrefab;
    public AudioSource shotClip;
    public AudioSource chargeClip;
    public AudioSource chargedClip;
    public AudioSource fireEmptyClip;
    public SpriteRenderer chargeGraphics;
    public Animator chargeAnimator;
//    public Image bulletCornerImage;
    public Text bulletCornerCount;
    //------------------------------------


    //PUBLIC ROMBUS PARAMS
    public float rombusRefactoryPeriod = 8.0f;
    //rombus sprite for effect
    public SpriteRenderer rombusRenderer;
    //how many max rombus can the ship fire
    public int currRombusCount = 5;
    public Image rombusCornerImage;
    public Text rombusCornerCount;
    //--------------------------------------

	[HideInInspector]
	public bool isAttacking;


    //PRIVATE VARS INTERNAL STATE
    private float lastShot;
    private SpriteRenderer shipImage;
    private bool rombusRecovered;
    private float frenzyBuildUp, pressTime = 0f;
    private bool charging = false;
    private bool charged = false;
    private bool inFrenzy = false;



	void Awake () {
		lastShot = 0;
		shipImage = GetComponent<SpriteRenderer> ();
		rombusRecovered = true;
	
		rombusCornerCount.text = "X " + currRombusCount;
        bulletCornerCount.text = "X " + bulletsAmmo;
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
		if (Input.GetButton ("Rombus Bomb") && !Input.GetButton("Main Cannon") && currRombusCount > 0) {
            GameController.Instance.ShipQuipper.spoutRandomQuip (QuipTypes.QUIP_FIRED_ROMBUS);
			var c = rombusRenderer.color;
			c.a = 0.0f;
			rombusRenderer.color = c;
			//update corner
			rombusCornerImage.color = c;
			currRombusCount--;
			rombusCornerCount.text = "X " + currRombusCount;

			var rombusGO = Instantiate (rombusBulletPrefab, transform.position, Quaternion.identity) as GameObject;
			rombusGO.GetComponent<ShipRombusController>().setShooter(gameObject);
			rombusRecovered = false;
		}
	}

	void ChargeFrenzy ()
	{
		bool buttonPressed = Input.GetButton ("Main Cannon");
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
            if (bulletsAmmo > 0) {
				var position = transform.up + transform.localPosition;
                var bullet = Instantiate (inFrenzy ? frenzyBulletPrefab : bulletPrefab, position, Quaternion.identity) as GameObject;
				bullet.GetComponent<BulletController>().setShooter(gameObject);
                bulletsAmmo--;
                bulletCornerCount.text = "X " + bulletsAmmo;
                shotClip.Play ();
            } else {
                fireEmptyClip.Play ();
            }
			lastShot = Time.time;
		}
	}
}
