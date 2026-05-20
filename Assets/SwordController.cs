using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField]
    private Sprite woodenSword;
    [SerializeField]
    private Sprite whiteSword;
    [SerializeField]
    private Sprite magicSword;
    private Sprite currentSword;
    // Start is called before the first frame update
    void Start()
    {
        currentSword = woodenSword;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            MOVEDIRECTION direction;
            Vector2 contactDirection = new Vector2();
            contactDirection = transform.parent.position - transform.position; //get direction by comparing the swod's position to the hero
            if (Mathf.Abs(contactDirection.x) > Mathf.Abs(contactDirection.y)) //left or right
            {
                if (contactDirection.x > 0)
                {
                    direction = MOVEDIRECTION.MOVE_LEFT;
                }
                else
                {
                    direction = MOVEDIRECTION.MOVE_RIGHT;
                }
            }
            else //up or down
            {
                if (contactDirection.y > 0) 
                {
                    direction = MOVEDIRECTION.MOVE_DOWN;
                }
                else
                {
                    direction = MOVEDIRECTION.MOVE_UP;
                }
                    
            }
            HitPacket hit = new HitPacket();
            hit.type = WEAPON_TYPE.TYPE_SWORD;
            hit.knockbackDirection = direction;
            hit.damageAmount = DamageTableManager._instance.damageTable.woodenSword;
            Debug.Log("Hit " + collision.gameObject.name + " in this direction: " + direction);
            collision.gameObject.GetComponent<SkellyEnemy>().GetHit(hit);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

            Vector2 contactDirection = new Vector2();
            contactDirection = collision.transform.position - transform.position;
            contactDirection = collision.GetContact(0).normal;
            Debug.Log("Hit " + collision.gameObject.name + " in this direction: " + contactDirection);
        }
    }
}
