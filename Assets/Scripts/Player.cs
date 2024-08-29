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

	private int isFlippedId, isMovingId;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	private void Update()
	{
		rb.position += movementSpeed * horizontal * Time.deltaTime * Vector2.right;								// moving player according to horizontal input
	}
	public void OnMoveInput (InputAction.CallbackContext context)
	{
		horizontal = context.ReadValue<Vector2>().x;															// getting horizontal input
	}
	public void OnJumpInput (InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed && isGrounded == true)									// getting jump input
		{
			rb.AddForce(jumpHeight * Vector2.up, ForceMode2D.Impulse);											// adding jump force
		}
	}
	public void OnMousePos (InputAction.CallbackContext context)
	{
		mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());								// calculating world point of mouse
		Vector2 temp = mousePos - (Vector2)gunTransform.position;												// calculating the vector between mousePos and gunPos
		float rotationAngle = -1 * Vector2.SignedAngle(temp, Vector2.right);									// calculating rotation angle between vector above and x axis
		gunTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);                                          // rotating the gun
	}
	public void OnMouseShoot (InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
		{
			GameObject bullet = Instantiate (bulletPrefab, gunBarrelTransform.position, gunTransform.rotation);	// instantiating the bullet at barrelPos and rotating it
			Debug.Log("Shoot!");
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			Debug.Log("Grounded!");
			isGrounded = true;																					// grounding player
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			Debug.Log("UnGrounded!");
			isGrounded = false;																					// ungrounding player
		}
	}
}
