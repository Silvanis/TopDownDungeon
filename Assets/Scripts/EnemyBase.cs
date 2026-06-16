using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected int health = 1;
    [SerializeField]
    protected int knockbackStrength = 2;
    [SerializeField]
    protected int damageDealt = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Weapons")) 
        {
            //take damage

            if (health <= 0)
            {
                //play death animation

            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //deal damage to player
        }
    }
}
