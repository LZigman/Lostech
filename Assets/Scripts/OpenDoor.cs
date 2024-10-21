using TMPro;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class OpenDoor : MonoBehaviour
{
	
	[SerializeField] private TextMeshProUGUI promptText;
	[SerializeField] private KeyCode keyToPress = KeyCode.E;
	[SerializeField] private GameObject doorCollider;
	[SerializeField] private bool isOpen;
	[SerializeField] private Animator animator;
	private string currentState;

	private const string OPEN_DOOR = "door_Clip";
	
	private void Start()
	{
		promptText.text = $"Press '{keyToPress}' key to activate.";
		promptText.enabled = false;
		animator = GetComponent<Animator>();
	}

	private void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;
		
		animator.Play(newState);

		currentState = newState;
	}
	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (!isOpen)
			{
				promptText.enabled = true;
			}
			if (!isOpen && Input.GetKey(keyToPress))
			{
				doorCollider.SetActive(false);
				ChangeAnimationState(OPEN_DOOR);
				isOpen = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			promptText.enabled = false;
			if (isOpen)
			{
				doorCollider.SetActive(true);
				isOpen = false;
			}
		}
	}
}
