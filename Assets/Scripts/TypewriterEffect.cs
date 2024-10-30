using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Tooltip("Text mesh that the message will be written at")]
    [SerializeField] private TextMeshProUGUI m_TextMesh;

    [Tooltip("How many characters written per second")]
    [SerializeField] private float m_CharSpeed = 2f;

    private Coroutine mTypeWriterCoroutine; //This will be used if player wants to halt the effect to see the whole message instantly

    private bool mIsEffectRunning = false;
    private bool isNextText;
    public bool IsRunning => mIsEffectRunning;

    public float TimeInterval => (1f / m_CharSpeed); //Calculated each time we need to get this value, in case we change effect speed while this effect is still running

    private string mMessage; //The string we will show to the player with this typewriter effect

    private void Awake()
    {
        m_TextMesh.text = ""; //Make sure the default content of text mesh won't be seen, even for a glimpse of a frame
    }

    public void StartEffect(string message)
    {
        mMessage = message;
        mIsEffectRunning = true;
        StartCoroutine(IncreaseMaxVisibleChar(message));
        
    }

    //textMesh.maxVisibleCharacters will increase gradually in this approach. Advised to use this function.
    private IEnumerator IncreaseMaxVisibleChar(string message)
    {
        m_TextMesh.text = message; //Make the text mesh's content the whole message string right at the beginning. So the characters will stay at the correct positions since the beginning

        m_TextMesh.maxVisibleCharacters = 0;
        var messageCharLength = message.Length;
        WaitForSeconds wait = new(TimeInterval);

        while (m_TextMesh.maxVisibleCharacters < messageCharLength)
        {
            m_TextMesh.maxVisibleCharacters++;
            yield return wait;
        }
    }

    //Let player see the whole message in instant
    public void Halt()
    {
        if (mTypeWriterCoroutine != null)
        {
            StopCoroutine(mTypeWriterCoroutine);
        }

        m_TextMesh.text = mMessage;
        m_TextMesh.maxVisibleCharacters = int.MaxValue;
    }

    private void Update()
    {
        //We assume player needs to press left mouse click to halt the effect, if effect is still running
        if (!IsRunning) return;
        bool isPlayerHaltingTypewriter = Input.GetMouseButtonDown(0);
        if (isPlayerHaltingTypewriter)
        {
            Halt();
            isNextText = Input.GetMouseButtonDown(0);
            if (isNextText)
            {
                mIsEffectRunning = false;
                isNextText = false;
            }
        }
    }
}
