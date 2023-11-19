using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroInput : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10.0f;
    private Rigidbody2D rigidbody2d;
    private Vector2 currentMoveVector = new Vector2(0.0f, 0.0f);
    private MoveDirection currentMoveDirection;
    private MoveDirection previousMoveDirection;
    private Animator heroAnimationController;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        heroAnimationController = gameObject.GetComponentInChildren<Animator>();
        
    }

    private void Awake()
    {
        previousMoveDirection = MoveDirection.MOVE_IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + (currentMoveVector * moveSpeed * Time.fixedDeltaTime));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMoveVector = context.ReadValue<Vector2>();
        
        if (Mathf.Abs(currentMoveVector.x) < 0.1f && Mathf.Abs(currentMoveVector.y) < 0.1f) //not moving
        {
            currentMoveDirection = MoveDirection.MOVE_IDLE;
        }
        else if (Mathf.Abs(currentMoveVector.x) > Mathf.Abs(currentMoveVector.y)) //moving left or right
        {
            currentMoveVector.y = 0.0f;
            currentMoveVector = currentMoveVector.normalized;
            if (currentMoveVector.x > 0.0f)
            {
                currentMoveDirection = MoveDirection.MOVE_RIGHT;
            }
            else
            {
                currentMoveDirection = MoveDirection.MOVE_LEFT;
            }
        }
        else //moving up or down
        {
            currentMoveVector.x = 0.0f;
            currentMoveVector = currentMoveVector.normalized;
            if (currentMoveVector.y > 0.0f)
            {
                currentMoveDirection = MoveDirection.MOVE_UP;
            }
            else
            {
                currentMoveDirection = MoveDirection.MOVE_DOWN;
            }
        }

        if (currentMoveDirection != previousMoveDirection)
        {
            switch (currentMoveDirection)
            {
                case MoveDirection.MOVE_UP:
                    heroAnimationController.SetTrigger("MoveUp");
                    break;
                case MoveDirection.MOVE_DOWN:
                    heroAnimationController.SetTrigger("MoveDown");
                    break;
                case MoveDirection.MOVE_LEFT:
                    heroAnimationController.SetTrigger("MoveLeft");
                    break;
                case MoveDirection.MOVE_RIGHT:
                    heroAnimationController.SetTrigger("MoveRight");
                    break;
                case MoveDirection.MOVE_IDLE:
                    heroAnimationController.SetTrigger("Idle");
                    break;
                default:
                    heroAnimationController.SetTrigger("Idle");
                    break;
            }
        }

        previousMoveDirection = currentMoveDirection;

    }
}

enum MoveDirection
{
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    MOVE_IDLE
}
