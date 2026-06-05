using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationCallback : MonoBehaviour
{
    SkellyEnemy controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<SkellyEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void AnimationEnded(string state)
    {
        if (state == "Dying")
        {
            controller.DyingAnimationEnded();
        }
    }
}
