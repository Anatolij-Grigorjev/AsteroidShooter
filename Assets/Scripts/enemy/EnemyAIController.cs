﻿using UnityEngine;

public class EnemyAIController : MonoBehaviour {

	
//------------ENEMY STATES-------------------

//Enemy not seeing player, randomly moving
private const int STATE_IDLE = 0;
//Enemy noticed player, move to attack range
private const int STATE_CHASING = 1;
//Enemy in attack range, fuckin up player
private const int STATE_ATTACKING = 2;
//Enemy in special attack range, do special
private const int STATE_ATTACKING_SPECIAL = 3;
//Defending aginst player attacks and trying to get closer
private const int STATE_DEFENSIVE = 4;
//largest continuous state number to ensure smooth random state
private const int STATE_RND = 5;
//------------------------------------------

public float playerChaseDistance;
public float playerAttackDistance;
public float playerSpecialDistance;
public float allowedPlayerHealth;
public float health;
//patroll speed while idle
public float cruiseSpeed;
//speed used while chasing
public float chaseSpeed;
//lowest helath to still act in a stable manner
public float healthStableMin;
public Transform playerTransform;
public GameObject engine;
public float recharge;
public GameObject bulletPrefab;
public AudioSource shotClip;
private int currentState; 
private int prevState;
//2 stages to the state machine here:
//1. Aggressive attack to bring down health
//2. Defensive approach to cage the player
private int stateMachineStage;

//internal state change vars
private float playerDistance;
private bool playerAttacking;
private float playerHealth;
private bool specialReady;
private ShotgunController shotgun;
private Rigidbody2D rigidBody;
private Collider2D collider;
//angle changes recorded to check if enemy still turning 
private Vector3 currFwd;
private Vector3 prevFwd;
//random rotation for idle frames
private Quaternion nextRotation;
private bool isRotating;
private int playerLayersMask;
private SpriteRenderer engineSprite;
private Animator engineAnimator;
private float lastShot;

public void Awake() {

	rigidBody = GetComponent<Rigidbody2D>();
	collider = GetComponent<Collider2D>();
	stateMachineStage = 1;

	prevState = STATE_IDLE;
	currentState = STATE_IDLE;
	currFwd = Vector3.zero;
	prevFwd = Vector3.zero;

	nextRotation = Quaternion.identity;
	isRotating = false;
	playerLayersMask = LayerMask.GetMask(new string[]{"Player"});

	engineSprite = engine.GetComponent<SpriteRenderer>();
	engineAnimator = engine.GetComponent<Animator>();
	engineAnimator.SetTrigger("isEngaged");

	lastShot = Time.time;

}

	public void Update() {

		//a state machine is a system that can use the knowledge of its external
		//inputs and previous state to formulate its next state

		//2 parts to update:
		//1. Resolve what the next state will be based on prevState and inputs
		//2. Resolve actions taken due to new state and old state combo

		//gather external inputs
		playerDistance = Vector3.Distance(transform.position, playerTransform.position);
		playerAttacking = playerTransform.gameObject.GetComponent<ShipShootingController>().isAttacking;
		playerHealth = playerTransform.gameObject.GetComponent<ShipHealthController>().health;
		specialReady = GetComponent<ShotgunController>().isReady;
		//and previous state prevState

		
		//after this call, the new state info should be in currentState, with old state still in prevState
		ResolveNewState();
		//after this call, the new state actions will have been resolved
		TakeAction();
		//remember new state as previous
		prevState = currentState;
	}

	private void ResolveNewState() {

		//initial distance based decisions
		if (playerDistance < playerSpecialDistance) {
			currentState = STATE_ATTACKING_SPECIAL;
		} else if (playerDistance < playerAttackDistance) {
			currentState = STATE_ATTACKING;
		} else if (playerDistance < playerChaseDistance) {
			currentState = STATE_CHASING;
		} else {
			currentState = STATE_IDLE;
		}

		//correct initial decisions based on situational context
		if (playerAttacking && playerDistance < playerChaseDistance
			&& currentState != STATE_ATTACKING_SPECIAL) {
			//defend if not doing special and player attacking
			currentState = STATE_DEFENSIVE;
		} 
		//resort to regular attack until special ready
		if (currentState == STATE_ATTACKING_SPECIAL && !specialReady) {
			currentState = STATE_ATTACKING;
		}

		//randomly decide to use special sometimes
		if (currentState == STATE_ATTACKING && specialReady) {
			if (Random.value < 0.5f) {
				currentState = STATE_ATTACKING_SPECIAL;
			}
		}

		//panic, act haphazardly (random state regardless of context)
		if (health < healthStableMin) {
			if (Random.value < 0.5f) {
				Debug.Log("Going crazy!");
				currentState = Mathf.RoundToInt(Random.value * STATE_RND);
			}
		}

		Debug.Log("Decided next state: " + currentState);
	}

	private void TakeAction() {
		//init current forward
		currFwd = transform.up;

		//have to check looking a player
		var rayHit = Physics2D.Raycast(transform.position, currFwd, playerChaseDistance, playerLayersMask);
		Debug.DrawRay(transform.position, currFwd * playerChaseDistance, Color.cyan, 1.0f);
		bool playerInRay = rayHit.collider != null? rayHit.collider.CompareTag("Ship") : false;
		Debug.Log("Got player in ray: " + playerInRay);

		//bit of rotation is ok, try actions
		isRotating = (currFwd - prevFwd).magnitude > 0.5f;

		//enable engine at large velocity
		engineSprite.enabled = rigidBody.velocity.magnitude > 0.25f;
		
		if (!isRotating || !playerInRay) {	
			isRotating = false;
			//enemy idle, keep faffing around
			nextRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.value * 90.0f));
			if (currentState == STATE_IDLE) {
				int roll = Mathf.RoundToInt(Random.value * 3);
				//3 choices:
				//1. do nothing
				//2. turn randomly
				//3. add juice
				Debug.Log("Decided idle action: " + roll);
				if (roll > 2) {
					rigidBody.AddForce(transform.up * cruiseSpeed);
				} else if (roll > 1) {
					isRotating = true;
					//start smooth rotation around Z for some amount
				} else {
					//do nothing
				}
				return;
			}

			
			if (!playerInRay) {
				//player not in ray, have to turn to face player
				isRotating = true;
				Debug.DrawLine(transform.up, playerTransform.position, Color.green);
				//relative position vector between target and current position
				var offset = playerTransform.position - transform.position;
				//Atan2 returns the angle in radians whose Tan is y/x. 
				//return value is the angle between the x-axis and a 2D vector starting at zero and terminating at (x,y).
				var neededRot = (Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg) - 90;
				nextRotation = Quaternion.Euler(0, 0, neededRot);				
			}

			//decided to chase player
			if (currentState == STATE_CHASING) {
				if (playerInRay) {
					Debug.Log("Player in Ray! chase them!");
					//player was noticed, chase after them
					if (rigidBody.velocity.magnitude < chaseSpeed && playerDistance > playerAttackDistance) {
						rigidBody.AddForce(transform.up * chaseSpeed);
					}
				}
			}

			if (currentState == STATE_ATTACKING) {
				if (playerInRay) {
					Debug.Log("Player in attack ray! Bang bang!");
					TryShoot();
				}
			}

			if (currentState == STATE_ATTACKING_SPECIAL) {
				Debug.Log("Time for the shotgun deluxe!");
				TryShootSpecial();
			}
		}
		//always check new forward and continue random rotation
		prevFwd = currFwd;
		if (isRotating) {
			transform.rotation = Quaternion.Slerp(transform.rotation, 
			nextRotation, 
			Time.deltaTime * 8);
		}
	}

	private void TryShoot() {
		//bugger seems to really shoot up the place
		if (Random.value > 0.9f) {
			if (Time.time > recharge + lastShot) {
				var position = ((transform.up * transform.localScale.magnitude) + transform.localPosition);
				var bullet = Instantiate (bulletPrefab, position, Quaternion.identity) as GameObject;
				bullet.GetComponent<BulletController>().setShooter(gameObject);
				shotClip.Play ();
				lastShot = Time.time;
			}
		}
	}

	private void TryShootSpecial() {
		//internally handles recharge n stuff, this is the visible trigger
		shotgun.ShootSpecial();
	}
}
