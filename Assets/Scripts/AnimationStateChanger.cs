using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateChanger : MonoBehaviour
{
    public static AnimationStateChanger Instance { get; private set; }
    
    private int currentState;

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    public void ChangeAnimationState(int newState, Animator animator)
    {
        if (currentState == newState) return;
		
        animator.Play(newState);

        currentState = newState;
    }
    public void PlayAnimationState (int state, Animator animator)
    {
        animator.Play(state);
    }

}
