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
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private KeyCode keyToPress = KeyCode.E;
    [SerializeField] private float activationDistance = 5f;
    [SerializeField] private float chargeTime;
    
    private bool teleporting = false;
    private bool done = false;
    private String prompt;
    private float timer = 0f;
    
    private static readonly int IdleTeleport = Animator.StringToHash("idleTeleport");
    private static readonly int Warp = Animator.StringToHash("warp");

    private void Start()
    {
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
                if (!teleporting && Input.GetKey(keyToPress) && !done)
                {
                    timer = Time.time;
                    teleporting = true;
                }
            }
            
            if (teleporting && !done)
            {
                var timeRemaining = chargeTime + timer - Time.time;
                int minutes = (int)timeRemaining / 60;
                int seconds = (int)timeRemaining % 60;
                promptText.text = $"Charging... Time remaining: {minutes:00}:{seconds:00}";
                if ((int)timeRemaining == 0) done = true;
            }

            if (done)
            {
                teleporting = false;
                promptText.text = $"Press '{keyToPress}' to teleport";
                if (Input.GetKey(keyToPress) && Vector2.Distance(playerPos.position, origin.position) <= activationDistance)
                {
                    StartCoroutine(Teleport(destination, other.transform));
                    done = false;
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
        if (animate)
        {
            AnimationStateChanger.Instance.ChangeAnimationState(Warp, animator);
            yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex:0).Length);
            AnimationStateChanger.Instance.ChangeAnimationState(IdleTeleport, animator);
        }
        player.position = new Vector2(target.position.x, target.position.y + 2);
        promptText.gameObject.SetActive(false);
        promptText.text = $"Press '{keyToPress}' key to activate.";
    }
}
