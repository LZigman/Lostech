using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
	private Rigidbody2D rb;
	private float horizontal;
	private Vector2 mousePos;
	private bool isGrounded;
	[SerializeField] private float jumpHeight, movementSpeed;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCheck, gunTransform, gunBarrelTransform, crosshairTransform;
	[SerializeField] private GameObject bulletPrefab;
	//[SerializeField] private Animator animator;

	private int movingDirId, lookingDirId;
	private float rotationAngle;
	private Vector2 delta;
	
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		movingDirId = Animator.StringToHash("movingDir");
		lookingDirId = Animator.StringToHash("lookingDir");
	}
	
	private void Update()
	{								
		// moving player according to horizontal input
		rb.position += movementSpeed * horizontal * Time.deltaTime * Vector2.right;
	}
	public void DamagePlayer (float damage)
	{
		// damage
		Debug.Log("Damage: " + damage);
	}
	public void OnMoveInput (InputAction.CallbackContext context)
	{
		// getting horizontal input
		horizontal = context.ReadValue<Vector2>().x;
		if (horizontal > 0)
		{
			//animator.SetInteger(movingDirId, 1);
			Debug.Log("movingDir == 1");
		}
		else if (horizontal < 0)
		{
			//animator.SetInteger(movingDirId, -1);
			Debug.Log("movingDir == -1");
		}
		else
		{
			//animator.SetInteger(movingDirId, 0);
			Debug.Log("movingDir == 0");
		}
		Debug.Log("Horizontal movement!");
	}
	
	public void OnJumpInput (InputAction.CallbackContext context)
	{
		// getting jump input
		if (context.phase == InputActionPhase.Performed && isGrounded == true)
		{
			// adding jump force
			rb.AddForce(jumpHeight * Vector2.up, ForceMode2D.Impulse);
			
			Debug.Log("Jump!");
		}
	}
	
	public void OnMousePos (InputAction.CallbackContext context)
	{
		// calculating world point of mouse
		mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
		
		// calculating the vector between mousePos and gunPos
		delta = mousePos - (Vector2)gunTransform.position;
		// mousePos left of gunTransform.pos
		if (delta.x < 0)
		{
			transform.localScale = new Vector3(-1, 1, 1);
			rotationAngle = Vector2.SignedAngle(-1 * transform.right, delta);
			//animator.SetInteger(lookingDirId, -1);
			Debug.Log("lookingDir == -1");
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
			rotationAngle = Vector2.SignedAngle(transform.right, delta);
			//animator.SetInteger(lookingDirId, 1);
			Debug.Log("lookingDir == 1");
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
			Vector2 delta = mousePos - (Vector2)gunTransform.position;
			float rotationAngle = Vector2.SignedAngle(Vector2.right, delta);
			GameObject bullet = Instantiate (bulletPrefab, gunBarrelTransform.position, Quaternion.Euler(0, 0, rotationAngle));
			
			Debug.Log("Bullet rotation: " + bullet.transform.rotation.eulerAngles.z);
			Debug.Log("Shoot!");
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			// grounding player
			isGrounded = true;
			
			Debug.Log("Grounded!");
		}
	}
	
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			// un-grounding player
			isGrounded = false;
			
			Debug.Log("UnGrounded!");
		}
	}
}
