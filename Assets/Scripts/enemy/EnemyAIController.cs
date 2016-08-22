using UnityEngine;

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
private ShootingController shooting;
private ShotgunController shotgun;
private Rigidbody2D rigidBody;
private Collider2D collider;

public void Awake() {

	rigidBody = GetComponent<Rigidbody2D>();
	collider = GetComponent<Collider2D>();
	shooting = GetComponent<ShootingController>();
	stateMachineStage = 1;

	prevState = STATE_IDLE;
	currentState = STATE_IDLE;
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
		//enemy idle, keep faffing around
		if (currentState == STATE_IDLE) {
			int roll = Mathf.RoundToInt(Random.value * 3);
			//3 choices:
			//1. do nothing
			//2. turn randomly
			//3. add juice

			if (roll > 2) {
				rigidBody.AddForce(Vector3.up * Time.deltaTime * cruiseSpeed);
			} else if (roll > 1) {
				//start smooth rotation around Z for osme amount
				transform.rotation = Quaternion.Slerp(
					transform.rotation,
					Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.value * 90.0f)), 
					Time.deltaTime
				);
			} else {
				//do nothing
			}
			return;
		}

		//have to turn to player
		// bool playerInRay = Physics2D.Raycast(tranform.position, {direction is quaternion Z rotation imagined as Vector3})
		var rayHit = Physics2D.Raycast(transform.position, Vector2.up);
		bool playerInRay = rayHit.collider.CompareTag("Ship");
		if (!playerInRay) {
			//player not in ray, have to turn to face player
			transform.rotation = Quaternion.Slerp(transform.rotation,
			 Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(transform.position, playerTransform.position))),
			 Time.deltaTime
			);
		}

		//decided to chase player
		if (currentState == STATE_CHASING) {
			if (playerInRay) {
				//player was noticed, chase after them
				if (rigidBody.velocity.magnitude < chaseSpeed && playerDistance > playerAttackDistance) {
					rigidBody.AddForce(Vector2.up * Time.deltaTime * chaseSpeed);
				}
			}
		}

		if (currentState == STATE_ATTACKING) {
			if (playerInRay) {
				TryShoot();
			}
		}

		if (currentState == STATE_ATTACKING_SPECIAL) {
			TryShootSpecial();
		}
	}

	private void TryShoot() {
		//internally handles recharge timers n shit, visible trigger to it all
		shooting.Shoot();
	}

	private void TryShootSpecial() {
		//internally handles recharge n stuff, this is the visible trigger
		shotgun.ShootSpecial();
	}
}
