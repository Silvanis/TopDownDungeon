using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndedEvent : MonoBehaviour
{
    HeroController controller;
    private void Start()
    {
        controller = GetComponentInParent<HeroController>();    
    }

    void AnimationEnded(string state)
    {
        if (state == "Attack")
        {
            controller.AttackEnded();
        }
    }
}