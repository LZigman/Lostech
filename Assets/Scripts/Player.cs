using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
	
	[SerializeField] private float jumpForce, movementSpeed;
	[SerializeField] private float fallTimeBeforeAnimation;
	[SerializeField] private Transform gunTransform, gunBarrelTransform, croshairTransform;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float maxHealth = 100f;
	
	private Animator animator;
	private Rigidbody2D rb;
	private int groundMask;
	private float xAxis, yAxis;
	private Vector2 mousePos;
	public bool isGrounded;
	private bool isAttackPressed;
	private bool isAttacking;
	public bool isFalling;
	
	private float rotationAngle;
	private Vector2 delta, vel;
	private float currentHealth;
	private float fallingTime = 0;
	
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
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		currentHealth = maxHealth;
		groundMask = 1 << LayerMask.NameToLayer("Ground");
		vel = new Vector2(0, 0);
	}

	private void FixedUpdate()
	{
		// check if player is on the ground
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
		
		vel = new Vector2(0, rb.velocity.y);
		// assign the velocity to the rigidbody
		rb.velocity = vel;

		
		// check if falling
		if (rb.velocity.y < 0)
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
		
		Debug.Log(rb.velocity);
	}

	public void OnMoveInput (InputAction.CallbackContext context)
	{
		Debug.Log("Move");
		// getting horizontal input
		xAxis = context.ReadValue<Vector2>().x;
		Debug.Log(xAxis);
		if (xAxis > 0)
		{
			vel.x = movementSpeed;
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerRunForward, animator);
		}
		else if (xAxis < 0)
		{
			vel.x = -movementSpeed;
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerRunBackward, animator);
		}
		else
		{
			vel.x = 0;
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerIdle, animator);
		}
		rb.velocity = vel;
	}
	
	public void OnJumpInput (InputAction.CallbackContext context)
	{
		// getting jump input
		if (context.phase == InputActionPhase.Performed && isGrounded && !isFalling)
		{
			// adding jump force
			rb.AddForce(new Vector2(0, jumpForce));
			AnimationStateChanger.Instance.ChangeAnimationState(PlayerJump, animator);
			
		}
	}

	private IEnumerator Falling()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(PlayerFall, animator);
		yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length);
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
			
			Debug.Log("Bullet rotation: " + bullet.transform.rotation.eulerAngles.z);
			Debug.Log("Shoot!");
		}
	}
	
	public void DamagePlayer (float damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0f)
		{
			Debug.Log("Die!");
		}
	}
}
