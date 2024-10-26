using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SavePoint : MonoBehaviour
{
    private Animator animator;
    private bool isOn;
    
    private static readonly int IdleOff = Animator.StringToHash("idleOff");
    private static readonly int IdleOn = Animator.StringToHash("idleOn");
    private static readonly int PowerOff = Animator.StringToHash("powerOff");
    private static readonly int PowerOn = Animator.StringToHash("powerOn");

    private void Start()
    {
        animator = GetComponent<Animator>();
        AnimationStateChanger.Instance.ChangeAnimationState(IdleOff, animator);    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isOn)
            {
                StartCoroutine(TurnOn());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isOn)
            {
                // StartCoroutine(TurnOff());
            }
        }
    }

    private IEnumerator TurnOn()
    {
        AnimationStateChanger.Instance.ChangeAnimationState(PowerOn, animator);
        yield return null;
        yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length);
        AnimationStateChanger.Instance.ChangeAnimationState(IdleOn, animator);
        isOn = true;
    }

    private IEnumerator TurnOff()
    {
        AnimationStateChanger.Instance.ChangeAnimationState(PowerOff, animator);
        yield return null;
        yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length);
        AnimationStateChanger.Instance.ChangeAnimationState(IdleOff, animator);
        isOn = false;
    }
}
