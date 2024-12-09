using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [FormerlySerializedAs("SpriteRend")]
    [Header("Animation")]
    [Space]
    [SerializeField] private GameObject fadeInOutPanel;
    [SerializeField] private float fadeInOutDuration;
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
    private bool done1 = false;
    private bool done2 = false;
    
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

        playerInput.DeactivateInput();
        StartCoroutine(FadeInAndOut(fadeInOutPanel, true, fadeInOutDuration));
       
        while(!done1) yield return null;
        player.position = new Vector2(target.position.x, target.position.y + 1.4f);
        StartCoroutine(FadeInAndOut(fadeInOutPanel, false, fadeInOutDuration));
        
        while(!done2) yield return null;
        playerInput.ActivateInput();
        done1 = false;
        done2 = false;
        promptText.gameObject.SetActive(false);
        promptText.text = $"Press '{keyToPress}' key to activate.";
        teleporting = false;
    }
    private IEnumerator FadeInAndOut(GameObject objectToFade, bool fadeIn, float duration)
    {
        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0;
            b = 1;
        }
        else
        {
            a = 1;
            b = 0;
        }

        int mode = 0;
        Color currentColor = Color.clear;

        SpriteRenderer tempSpRenderer = objectToFade.GetComponent<SpriteRenderer>();
        Image tempImage = objectToFade.GetComponent<Image>();
        RawImage tempRawImage = objectToFade.GetComponent<RawImage>();
        MeshRenderer tempRenderer = objectToFade.GetComponent<MeshRenderer>();
        Text tempText = objectToFade.GetComponent<Text>();

        //Check if this is a Sprite
        if (tempSpRenderer != null)
        {
            currentColor = tempSpRenderer.color;
            mode = 0;
        }
        //Check if Image
        else if (tempImage != null)
        {
            currentColor = tempImage.color;
            mode = 1;
        }
        //Check if RawImage
        else if (tempRawImage != null)
        {
            currentColor = tempRawImage.color;
            mode = 2;
        }
        //Check if Text 
        else if (tempText != null)
        {
            currentColor = tempText.color;
            mode = 3;
        }

        //Check if 3D Object
        else if (tempRenderer != null)
        {
            currentColor = tempRenderer.material.color;
            mode = 4;

            //ENABLE FADE Mode on the material if not done already
            tempRenderer.material.SetFloat("_Mode", 2);
            tempRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tempRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tempRenderer.material.SetInt("_ZWrite", 0);
            tempRenderer.material.DisableKeyword("_ALPHATEST_ON");
            tempRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            tempRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            tempRenderer.material.renderQueue = 3000;
        }
        else
        {
            yield break;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            switch (mode)
            {
                case 0:
                    tempSpRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 1:
                    tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 2:
                    tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 3:
                    tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 4:
                    tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
            }
            yield return null;
        }

        if (done1)
        {
            done2 = true;
        }
        done1 = true;
    }
}
