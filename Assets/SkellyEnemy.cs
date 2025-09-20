using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class SkellyEnemy : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 10.0f;
    [SerializeField]
    private float howManyTilesToTravel = 1.0f;
    [SerializeField]
    private float idleTime = 2.0f;
    [SerializeField]
    private KnockbackStrength knockback;
    [SerializeField]
    private int damageDealt = 1;
    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private Vector2 currentMoveVector = Vector2.zero;
    private bool reachedPosition = false;
    private MoveDirection currentDirection = MoveDirection.MOVE_IDLE;
    private float distanceTravelled = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
        animator = gameObject.GetComponentInChildren<Animator>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        ChangePosition();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (currentDirection == MoveDirection.MOVE_IDLE) { return; }

        float distanceIncrement = movementSpeed * Time.fixedDeltaTime;
        distanceTravelled += distanceIncrement;
        CheckForReachedPosition();
        if (reachedPosition) 
        {
            ChangePosition();
        }
        rigidbody2d.MovePosition(rigidbody2d.position + (currentMoveVector * distanceIncrement));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {

            Vector2 contactDirection = new Vector2(); 
            contactDirection = collision.GetContact(0).normal;
            contactDirection = -contactDirection; //need to reverse the direction of the normal for pushback
            Debug.Log("<color=orange>Die pesky hero! Push in: </color>" + contactDirection);
            collision.gameObject.GetComponent<HeroController>().GetHit(damageDealt, knockback, ref contactDirection, gameObject);
        }
    }

    private void ChangePosition()
    {
        bool validDirection = false;
        MoveDirection newDirection = RollDirection();
        LayerMask mask = LayerMask.GetMask("Default", "Walls", "BlocksEnemy"); 
        while(!validDirection) 
        {

            switch (newDirection)
            {
                case MoveDirection.MOVE_IDLE:
                    currentMoveVector = Vector2.zero;
                    break;
                case MoveDirection.MOVE_UP:
                    currentMoveVector = Vector2.up;
                    break;
                case MoveDirection.MOVE_DOWN:
                    currentMoveVector = Vector2.down;
                    break;
                case MoveDirection.MOVE_LEFT:
                    currentMoveVector = Vector2.left;
                    break;
                case MoveDirection.MOVE_RIGHT:
                    currentMoveVector = Vector2.right;
                    break;
            }

            if (currentMoveVector == Vector2.zero)
            {
                StartCoroutine(Idle(idleTime));
                validDirection = true;
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, currentMoveVector, 2.5f, mask);

                if (hit.collider == null)
                {
                    validDirection = true; 
                    break;
                }
                else
                {
                    newDirection = RollDirection();
                }
            }

        }

        currentDirection = newDirection;
        ChangeFacing();
    }

    private void ChangeFacing()
    {
        switch (currentDirection)
        {
            case MoveDirection.MOVE_IDLE:
                animator.SetTrigger("Idle");
                break;
            case MoveDirection.MOVE_UP:
                animator.SetTrigger("MoveUp");
                break;
            case MoveDirection.MOVE_DOWN:
                animator.SetTrigger("MoveDown");
                break;
            case MoveDirection.MOVE_LEFT:
                animator.SetTrigger("MoveLeft");
                break;
            case MoveDirection.MOVE_RIGHT:
                animator.SetTrigger("MoveRight");
                break;
        }
    }

    private MoveDirection RollDirection()
    {
        MoveDirection newDirection;
        //don't roll idle more than once
        if (currentDirection == MoveDirection.MOVE_IDLE) 
        {
            newDirection = (MoveDirection)UnityEngine.Random.Range(0, 4);
        }
        else
        {
            newDirection = (MoveDirection)UnityEngine.Random.Range(0, 5);
        }
        return newDirection;
    }

    private IEnumerator Idle(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);
        distanceTravelled = 0;
        ChangePosition();

    }

    private void CheckForReachedPosition()
    {
        if (distanceTravelled >= howManyTilesToTravel - 0.05f) 
        {
            Vector2 position = rigidbody2d.position;
            //center of tile is .5, so need to round to .5
            position.x = Mathf.Round(position.x * 2) / 2;
            position.y = Mathf.Round(position.y * 2) / 2;
            rigidbody2d.position = position;
            distanceTravelled = 0.0f;
            reachedPosition = true;
        }
        else
        {
            reachedPosition = false;
        }
    }
}
