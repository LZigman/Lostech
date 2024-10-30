//Sample script that calls our typewriter effect

using UnityEngine;

public class TypewriterEffectCallerSample : MonoBehaviour
{
    [Tooltip("What we want to be written with a typewriter effect")]
    [SerializeField]
    string m_Message;

    [SerializeField]
    TypewriterEffect m_TypewriterEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_TypewriterEffect.StartEffect(m_Message);
    }
}
