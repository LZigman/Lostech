using System;
using TMPro;
using UnityEngine;
public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private Transform origin, destination;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private KeyCode keyToPress = KeyCode.E;
    [SerializeField] private float activationDistance = 5f;
    [SerializeField] private float chargeTime;
    
    private bool teleporting = false;
    private bool done = false;
    private String prompt;
    private float timer = 0f;

    private void Start()
    {
        origin = this.gameObject.transform;
        promptText.text = $"Press '{keyToPress}' key to activate.";
        promptText.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().isTrigger = true;
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
                    timer = 0;
                    teleporting = true;
                }
            }
            
            if (teleporting && !done)
            {
                timer += Time.deltaTime;
                var timeRemaining = chargeTime - timer;
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
                    Teleport(destination, other.transform);
                    done = false;
                }
            }
        }

        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Teleport(Transform target, Transform player)
    {
        player.position = new Vector2(target.position.x, target.position.y + 2);
        promptText.gameObject.SetActive(false);
        promptText.text = $"Press '{keyToPress}' key to activate.";
    }
}
