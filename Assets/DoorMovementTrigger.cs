using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovementTrigger : MonoBehaviour
{
    public static event Action<MoveDirection> OnScreenTransition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null) 
        {
            if (collision.gameObject.tag == "Player") 
            {
                MoveDirection playerDirection = collision.gameObject.GetComponent<HeroController>().currentMoveDirection;
                
                OnScreenTransition?.Invoke(playerDirection);
            }
        }
    }
}
