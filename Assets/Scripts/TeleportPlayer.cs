using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class TeleportPlayer : MonoBehaviour
{
    [Header("Animation")]
    [Space]
    [SerializeField] private Animator animator;
    [SerializeField] private bool animate;

    [Header("Parameters")] 
    [Space] 
    [SerializeField] private Transform origin;
    [SerializeField] private Transform destination;
    [SerializeField] private BoxCollider2D activationArea;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private KeyCode keyToPress = KeyCode.E;
    [SerializeField] private float chargeTime;
    
    
    private float activationDistance;
    [SerializeField]private bool charging = false;
    [SerializeField]private bool teleporting = false;
    private String prompt;
    private float timer = 0f;
    
    private static readonly int IdleTeleport = Animator.StringToHash("idleTeleport");
    private static readonly int Warp = Animator.StringToHash("warp");

    private void Start()
    {
        activationDistance = activationArea.size.x;
        origin = this.gameObject.transform;
        promptText.text = $"Press '{keyToPress}' key to activate.";
        promptText.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().isTrigger = true;
        animator = GetComponent<Animator>();
        if (animator != null) animate = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Transform playerPos = other.transform;
            if (Vector2.Distance(playerPos.position, origin.position) <= activationDistance)
            {
                promptText.gameObject.SetActive(true);
                if (!charging && !teleporting)
                {
                    if (other.gameObject.GetComponent<Player>().isInteracting == true)
                    {
                        timer = Time.time;
                        charging = true;
                        other.gameObject.GetComponent<Player>().isInteracting = false;
                    }
                }
            }
            
            if (charging && !teleporting)
            {
                var timeRemaining = chargeTime + timer - Time.time;
                int minutes = (int)timeRemaining / 60;
                int seconds = (int)timeRemaining % 60;
                promptText.text = $"Charging... Time remaining: {minutes:00}:{seconds:00}";
                if ((int)timeRemaining == 0) teleporting = true;
            }

            if (teleporting)
            {
                charging = false;
                promptText.text = $"Press '{keyToPress}' to teleport";
                if ((Input.GetKey(keyToPress) || !animate) && Vector2.Distance(playerPos.position, origin.position) <= activationDistance)
                {
                    StartCoroutine(Teleport(destination, other.transform));
                }
            }
        }

        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private IEnumerator Teleport(Transform target, Transform player)
    {
        AudioManager.Instance.PlaySFX("teleporting");
        if (animate)
        {
            AnimationStateChanger.Instance.ChangeAnimationState(Warp, animator);
            yield return null;
            yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0)[0].clip.length);
            AnimationStateChanger.Instance.ChangeAnimationState(IdleTeleport, animator);
        }
        player.position = new Vector2(target.position.x, target.position.y + 1.25f);
        promptText.gameObject.SetActive(false);
        promptText.text = $"Press '{keyToPress}' key to activate.";
        teleporting = false;
    }
}
