using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
	[SerializeField] private HealthBar healthBar;
	[SerializeField] private GameObject pauseMenu, deathScreen;
	[SerializeField] private Transform gunTransform, gunBarrelTransform, croshairTransform;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private BulletCounter bulletCounter;
	[SerializeField] private float jumpForce, movementSpeed, downwardForce;
	[SerializeField] private float fallTimeBeforeAnimation;
	[SerializeField] private int maxHealth = 5;
	
	private Animator animator;
	private Rigidbody2D rb;
	private int groundMask;
	private float xAxis, yAxis;
	private Vector2 mousePos;
	private float rotationAngle;
	private Vector2 delta, vel;
	private float currentHealth;
	private float fallingTime;
	
	[HideInInspector] public bool isJumping;
	[HideInInspector] public bool isDropDown;
	[HideInInspector] public bool isDead;
	[HideInInspector] public bool isInteracting;
	[HideInInspector] public Vector2 lastSavePoint;
	
	private bool isGrounded;
	private bool isAttackPressed;
	private bool isAttacking;
	private bool isFalling;
	private bool isColliding;
	private bool isHurt;
	private bool isGroundMetallic;
	private bool isDying;
	
	private static readonly int PlayerRunForward = Animator.StringToHash("playerRunForward");
	private static readonly int PlayerRunBackward = Animator.StringToHash("playerRunBackward");
	private static readonly int LookingDir = Animator.StringToHash("lookingDir");
	private static readonly int PlayerIdle = Animator.StringToHash("playerIdle");
	private static readonly int PlayerJump = Animator.StringToHash("playerJump");
	private static readonly int PlayerFall = Animator.StringToHash("playerFall");
	private static readonly int PlayerCrouchLand = Animator.StringToHash("playerCrouchLand");
	private static readonly int PlayerRoll = Animator.StringToHash("playerRoll");
	private static readonly int PlayerHurt = Animator.StringToHash("PlayerHurt");
	private static readonly int PlayerHeal = Animator.StringToHash("PlayerHeal");
	private static readonly int PlayerDie = Animator.StringToHash("PlayerDie");

	
	private void Start()
	{
		isDead = false;
		isInteracting = false;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		currentHealth = maxHealth;
		groundMask = 1 << LayerMask.NameToLayer("Ground");
		vel = new Vector2(rb.velocity.x, rb.velocity.y);
	}

	private void Update()
	{
		if (currentHealth <= 0f)
		{
			isDead = true;
		}  
		if (isDead && !isDying)
		{
			// start death
			isDying = true;
			StartCoroutine(Death());
		}
	}
	private IEnumerator Death()
	{
		//currentHealth = maxHealth;
        Debug.Log("Die!");
        // Player death animation
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerDie, animator);
        yield return new WaitForSeconds(1f);
        // Load Death screen
        deathScreen.SetActive(true);
        // freeze time
        Time.timeScale = 0;
    }
	private void FixedUpdate()
	{
		// check if player is on the ground
		GroundCheck();
		
		// check if falling
		FallCheck();
		
		// check where the player is moving
		MovementCheck();
		
		// assign the velocity to the rigidbody
		var vector2 = rb.velocity;
		vector2.x = vel.x;
		rb.velocity = vector2;
		
	}

	private void GroundCheck()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f + 0.1f, groundMask);
		Debug.DrawLine(transform.position, hit.point, Color.red);

		if (hit.collider != null)
		{
			isGrounded = true;
			if (hit.collider.gameObject.CompareTag("Platform"))
			{
				isGroundMetallic = true;
			}
			else
			{
				isGroundMetallic = false;
			}
		}
		else
		{
			isGrounded = false;
			isGroundMetallic = false;
		}
	}

	private void MovementCheck()
	{
		if (xAxis > 0)
		{
			vel.x = movementSpeed;
			if (!isFalling && isGrounded && !isJumping)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(PlayerRunForward, animator);
				if (isGroundMetallic == true)
				{
					AudioManager.Instance.PlaySFX("player walk metallic");
				}
				else
				{
					AudioManager.Instance.PlaySFX("player walk");
				}
			}
		}
		else if (xAxis < 0)
		{
			vel.x = -movementSpeed;
			if (!isFalling && isGrounded && !isJumping)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(PlayerRunBackward, animator);
				AudioManager.Instance.PlaySFX("player walk");
			}
		}
		else
		{
			vel.x = 0;
			if (!isFalling && isGrounded && !isJumping && !isHurt)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(PlayerIdle, animator);
			}
		}
	}

	private void FallCheck()
	{
		if (rb.velocity.y < -0.1)
		{
			if (isGrounded == true)
			{
				AudioManager.Instance.PlaySFX("land");
			}
			isFalling = true;
			fallingTime += Time.fixedDeltaTime;
			if (fallingTime >= fallTimeBeforeAnimation)
			{
				// play falling animation
				StartCoroutine(Falling());
				// add downward force
				rb.AddForce(new Vector2(0, -downwardForce));
			}
		}
		
		else
		{
			isFalling = false;
			fallingTime = 0;
		}
	}

	private IEnumerator Falling()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerFall, animator);
		yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length);
	}

	public void OnInteractInput(InputAction.CallbackContext context)
	{
		if (Time.timeScale != 1f)
		{
			return;
		}
		if (context.phase == InputActionPhase.Performed)
		{
			isInteracting = true;
		}
	}
	public void OnMoveInput(InputAction.CallbackContext context)
	{
        if (Time.timeScale != 1f)
        {
            return;
        }
        // getting horizontal input
        xAxis = context.ReadValue<Vector2>().x;
	}
	
	public void OnJumpInput(InputAction.CallbackContext context)
	{
        if (Time.timeScale != 1f)
        {
            return;
        }

        if (isJumping && !Input.GetKey(KeyCode.Space))
        {
	        rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        
        // getting jump input
        if (context.phase == InputActionPhase.Performed && isGrounded && !isFalling)
		{
			isJumping = true;
			// adding jump force
			rb.AddForce(new Vector2(0, jumpForce * rb.mass));
			AudioManager.Instance.PlaySFX("jump");
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerJump, animator);
		}
		
		else
		{
			isJumping = false;
		}
	}
	public void OnDropDownInput(InputAction.CallbackContext context)
	{
        if (Time.timeScale != 1f)
        {
            return;
        }
        if (context.phase == InputActionPhase.Performed && isGrounded && !isFalling && !isJumping)
		{
			isDropDown = true;
		}
		else
		{
			isDropDown = false;
		}
	}
	
	public void OnMousePos(InputAction.CallbackContext context)
	{
        if (Time.timeScale != 1f)
        {
            return;
        }
        // calculating world point of mouse
        if (Camera.main != null) mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());

		// calculating the vector between mousePos and gunPos
		delta = mousePos - (Vector2)gunTransform.position;
		// mousePos left of gunTransform.pos
		if (delta.x < 0)
		{
			FlipLeft(true);
			rotationAngle = Vector2.SignedAngle(-1 * transform.right, delta);
			animator.SetInteger(LookingDir, -1);
			//Debug.Log("lookingDir == -1");
		}
		else
		{
			FlipLeft(false);
			rotationAngle = Vector2.SignedAngle(transform.right, delta);
			animator.SetInteger(LookingDir, 1);
			//Debug.Log("lookingDir == 1");
		}

		// calculating rotation angle between vector above and x axis
		//float rotationAngle = -1 * Vector2.SignedAngle(temp, transform.right);
		// rotating the gun
		gunTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
	}
	
	public void OnMouseShoot(InputAction.CallbackContext context)
	{
        if (Time.timeScale != 1f)
        {
            return;
        }
        if (context.phase == InputActionPhase.Performed)
		{
			if (!bulletCounter.canFire) return;
			bulletCounter.ReduceAmmo();
			// instantiating the bullet at barrelPos and rotating it
            delta = (Vector2)croshairTransform.position - (Vector2)gunTransform.position;
			rotationAngle = Vector2.SignedAngle(Vector2.right, delta);
			GameObject bullet = Instantiate (bulletPrefab, gunBarrelTransform.position, Quaternion.Euler(0, 0, rotationAngle));
			AudioManager.Instance.PlaySFX("player shoot");
		}
	}
	private void FlipLeft(bool toggle)
	{
		if (toggle == true)
		{
			transform.localScale = new Vector3(-1f, 1f, 1f);
			transform.GetChild(0).localScale = new Vector3(-1f, 0.35f, 1f);
		}
		else
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
            transform.GetChild(0).localScale = new Vector3(1f, 0.35f, 1f);
        }
	}
	public void DamagePlayer(float damage)
	{
		currentHealth -= damage;
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerHurt, animator);
		StartCoroutine(DamageAnimation());
		AudioManager.Instance.PlaySFX("player hurt");
		Debug.Log("Current health: " + currentHealth);
		healthBar.ReduceHealthBar((int)currentHealth);
	}
	
	public void HealPlayer (float healing)
	{
		currentHealth += healing;
		StartCoroutine(HealAnimation());
		healthBar.IncreaseHealthBar((int)currentHealth);
	}
	
	public void HealPlayerToFull ()
	{
		currentHealth = maxHealth;
		Debug.Log("healing!");
		StartCoroutine(HealAnimation());
		healthBar.IncreaseHealthBarToFull();
	}
	
	private IEnumerator DamageAnimation()
	{
		isHurt = true;
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerHurt, animator);
		yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
		isHurt = false;
    }
	
	private IEnumerator HealAnimation()
	{
		isHurt = true;
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerHeal, animator);
		yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
		isHurt = false;
	}

	public void OnMenuButtonClick(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed && Time.timeScale != 0)
		{
			pauseMenu.SetActive(true);
		}
	}

	public void OnRespawnClick()
	{
		currentHealth = maxHealth;
		rb.position = lastSavePoint;
		Debug.Log("Respawned!");
		healthBar.IncreaseHealthBarToFull();
		isDead = false;
		isDying = false;
		Time.timeScale = 1;
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerIdle, animator);
	}
}
