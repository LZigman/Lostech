using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
	[SerializeField] private Transform gunTransform, gunBarrelTransform, croshairTransform;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float jumpForce, movementSpeed;
	[SerializeField] private float fallTimeBeforeAnimation;
	[SerializeField] private float maxHealth = 100f;
	
	private Animator animator;
	private Rigidbody2D rb;
	private int groundMask;
	private float xAxis, yAxis;
	private Vector2 mousePos;
	private float rotationAngle;
	private Vector2 delta, vel;
	private float currentHealth;
	private float fallingTime;
	
	public bool isJumping;
	public bool isDropDown;
	public bool isDead;
	
	private bool isGrounded;
	private bool isAttackPressed;
	private bool isAttacking;
	private bool isFalling;
	private bool isColliding;
	
	private static readonly int PlayerRunForward = Animator.StringToHash("playerRunForward");
	private static readonly int PlayerRunBackward = Animator.StringToHash("playerRunBackward");
	private static readonly int LookingDir = Animator.StringToHash("lookingDir");
	private static readonly int PlayerIdle = Animator.StringToHash("playerIdle");
	private static readonly int PlayerJump = Animator.StringToHash("playerJump");
	private static readonly int PlayerFall = Animator.StringToHash("playerFall");
	private static readonly int PlayerCrouchLand = Animator.StringToHash("playerCrouchLand");
	private static readonly int PlayerRoll = Animator.StringToHash("playerRoll");

	
	private void Start()
	{
		isDead = false;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		currentHealth = maxHealth;
		groundMask = 1 << LayerMask.NameToLayer("Ground");
		vel = new Vector2(rb.velocity.x, rb.velocity.y);
	}

	private void Update()
	{
		if (currentHealth <= 0f || isDead)
		{
			Debug.Log("Die!");
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
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
		}
		else
		{
			isGrounded = false;
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
				AudioManager.Instance.PlaySFX("player walk");
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
			if (!isFalling && isGrounded && !isJumping) AnimationStateChanger.Instance.ChangeAnimationState(PlayerIdle, animator);
		}
	}

	private void FallCheck()
	{
		if (rb.velocity.y < -0.1)
		{
			isFalling = true;
			fallingTime += Time.fixedDeltaTime;
			if (fallingTime >= fallTimeBeforeAnimation)
			{
				// play falling animation
				StartCoroutine(Falling());
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

	public void OnMoveInput (InputAction.CallbackContext context)
	{
		// getting horizontal input
		xAxis = context.ReadValue<Vector2>().x;
	}
	
	public void OnJumpInput (InputAction.CallbackContext context)
	{
		// getting jump input
		if (context.phase == InputActionPhase.Performed && isGrounded && !isFalling)
		{
			isJumping = true;
			// adding jump force
			rb.AddForce(new Vector2(0, jumpForce * rb.mass));
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerJump, animator);
		}
		
		else
		{
			isJumping = false;
		}
	}

	public void OnDropDownInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed && isGrounded && !isFalling && !isJumping)
		{
			isDropDown = true;
		}
		else
		{
			isDropDown = false;
		}
	}
	
	public void OnMousePos (InputAction.CallbackContext context)
	{
		// calculating world point of mouse
		if (Camera.main != null) mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());

		// calculating the vector between mousePos and gunPos
		delta = mousePos - (Vector2)gunTransform.position;
		// mousePos left of gunTransform.pos
		if (delta.x < 0)
		{
			transform.localScale = new Vector3(-1, 1, 1);
			rotationAngle = Vector2.SignedAngle(-1 * transform.right, delta);
			animator.SetInteger(LookingDir, -1);
			//Debug.Log("lookingDir == -1");
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
			rotationAngle = Vector2.SignedAngle(transform.right, delta);
			animator.SetInteger(LookingDir, 1);
			//Debug.Log("lookingDir == 1");
		}

		// calculating rotation angle between vector above and x axis
		//float rotationAngle = -1 * Vector2.SignedAngle(temp, transform.right);
		// rotating the gun
		gunTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
	}
	
	public void OnMouseShoot (InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
		{
			// instantiating the bullet at barrelPos and rotating it
			delta = mousePos - (Vector2)gunTransform.position;
			rotationAngle = Vector2.SignedAngle(Vector2.right, delta);
			GameObject bullet = Instantiate (bulletPrefab, gunBarrelTransform.position, Quaternion.Euler(0, 0, rotationAngle));
			AudioManager.Instance.PlaySFX("player shoot");
			Debug.Log("Bullet rotation: " + bullet.transform.rotation.eulerAngles.z);
			Debug.Log("Shoot!");
		}
	}
	
	public void DamagePlayer (float damage)
	{
		currentHealth -= damage;
	}
}
