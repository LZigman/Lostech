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
	[SerializeField] private Transform groundCheck, gunTransform, gunBarrelTransform;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Animation runRight;

	private int isFlippedId, isMovingId;
	
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	
	private void Update()
	{								
		// moving player according to horizontal input
		rb.position += movementSpeed * horizontal * Time.deltaTime * Vector2.right;
		
		if (horizontal > 0)
		{
			runRight.Play();
		}
	}
	
	public void OnMoveInput (InputAction.CallbackContext context)
	{
		// getting horizontal input
		horizontal = context.ReadValue<Vector2>().x;
		
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
		Vector2 temp = mousePos - (Vector2)gunTransform.position;
		
		// calculating rotation angle between vector above and x axis
		float rotationAngle = -1 * Vector2.SignedAngle(temp, Vector2.right);
		
		// rotating the gun
		gunTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
	}
	
	public void OnMouseShoot (InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
		{
			// instantiating the bullet at barrelPos and rotating it
			GameObject bullet = Instantiate (bulletPrefab, gunBarrelTransform.position, gunTransform.rotation);
			
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
