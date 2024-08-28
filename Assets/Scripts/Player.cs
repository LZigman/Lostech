using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
	private Rigidbody2D rb;
	private float horizontal;
	private bool isGrounded;
	[SerializeField] private float jumpHeight, movementSpeed;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Animator animator;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	private void Update()
	{
		rb.position += movementSpeed * horizontal * Time.deltaTime * Vector2.right;
		
		animator.SetFloat("moveDir", horizontal);
		animator.SetFloat("jumpDir", rb.velocity.y);
	}
	public void OnMoveInput (InputAction.CallbackContext context)
	{
		Debug.Log("Horizontal movement!");
		horizontal = context.ReadValue<Vector2>().x;
	}
	public void OnJumpInput (InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed && isGrounded == true)
		{
			Debug.Log("Jump!");
			rb.AddForce(jumpHeight * Vector2.up, ForceMode2D.Impulse);
		}
	}/*
	private bool IsGrounded ()
	{
		return Physics2D.OverlapCollider(groundCheckCollider, 0.2f, groundLayer);
	}*/
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			Debug.Log("Grounded!");
			isGrounded = true;
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			Debug.Log("UnGrounded!");
			isGrounded = false;
		}
	}
}
