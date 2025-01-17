using System.Collections;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Door : MonoBehaviour
{
	
	[SerializeField] private TextMeshProUGUI promptText;
	[SerializeField] private KeyCode keyToPress = KeyCode.E;
	[SerializeField] private GameObject doorCollider;
	[SerializeField] private bool isOpen;
	[SerializeField] private Animator animator;
	
	private static readonly int OpenDoor = Animator.StringToHash("openDoor");
	private static readonly int CloseDoor = Animator.StringToHash("closeDoor");
	private static readonly int IdleOpen = Animator.StringToHash("idleOpen");
	private static readonly int IdleClosed = Animator.StringToHash("idleClosed");

	private void Start()
	{
		promptText.text = $"Press '{keyToPress}' key to activate.";
		promptText.enabled = false;
		animator = GetComponent<Animator>();
	}
	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (!isOpen)
			{
				promptText.enabled = true;
				if (other.gameObject.GetComponent<Player>().isInteracting == true)
				{
					Debug.Log("Opening door!");
					StartCoroutine(Open());
					other.gameObject.GetComponent<Player>().isInteracting = false;
                }
			}
			/*if (!isOpen && Input.GetKey(keyToPress))
			{
				Debug.Log("open triggered!");
				StartCoroutine(Open());
			}*/
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			promptText.enabled = false;
			if (isOpen)
			{
				StartCoroutine(Close());
			}
		}
	}

	private IEnumerator Open()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(OpenDoor, animator);
		AudioManager.Instance.PlaySFX("door opening");
		yield return null;
		yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length - 0.2f);
		AnimationStateChanger.Instance.ChangeAnimationState(IdleOpen, animator);
		doorCollider.SetActive(false);
		isOpen = true;
	}

	private IEnumerator Close()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(CloseDoor, animator);
        AudioManager.Instance.PlaySFX("door closing");
        yield return null;
		yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length - 0.2f);
		AnimationStateChanger.Instance.ChangeAnimationState(IdleClosed, animator);
		doorCollider.SetActive(true);
		isOpen = false;
	}
}
