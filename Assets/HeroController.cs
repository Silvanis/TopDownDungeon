using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class HeroController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private float roomMoveTime = 1.0f;
    [SerializeField]
    private int knockbackFrames = 3;
    [SerializeField]
    private float invincibilityDuration = 3.0f;
    private float invincibilityAccumulator = 0.0f;
    private int remainingKnockbackFrames = 0;
    private Rigidbody2D rigidbody2d;
    private Vector2 currentMoveVector = new Vector2(0.0f, 0.0f);
    public MoveDirection currentMoveDirection { get; private set; }
    private MoveDirection previousMoveDirection;
    private Vector2 knockbackDirection;
    private KnockbackStrength knockbackStrength;
    private Animator heroAnimationController;
    private bool isPaused = false;
    private bool isInvincible = false;
    private PlayerInput m_playerInput;
    private CHARACTER_STATE currentState = CHARACTER_STATE.NORMAL;
    private bool isStillTouching = false;
    private int lastEnemyDamage;
    private GameObject lastEnemy;
    private Vector2 lastEnemyKnockbackDirection;
    private KnockbackStrength lastEnemyKnockbackStrength;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        heroAnimationController = gameObject.GetComponentInChildren<Animator>();
        m_playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        previousMoveDirection = MoveDirection.MOVE_IDLE;
        currentMoveDirection = MoveDirection.MOVE_IDLE;
        DoorMovementTrigger.OnScreenTransition += OnScreenTransisiton;
    }

    private void OnDisable()
    {
        DoorMovementTrigger.OnScreenTransition -= OnScreenTransisiton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (currentState == CHARACTER_STATE.NORMAL)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + (currentMoveVector * moveSpeed * Time.fixedDeltaTime));
        }
        else if (currentState == CHARACTER_STATE.KNOCKBACK)
        {
            if (remainingKnockbackFrames > 0)
            {
                float movementDistance = (float)knockbackStrength / (float)knockbackFrames;
                rigidbody2d.MovePosition(rigidbody2d.position + (knockbackDirection * movementDistance));
                remainingKnockbackFrames--;
            }
            else
            {
                currentState = CHARACTER_STATE.NORMAL;
            }

        }

        if (isInvincible)
        {
            if (invincibilityAccumulator >= invincibilityDuration)
            {
                isInvincible = false;
                invincibilityAccumulator = 0.0f;
                if (isStillTouching)
                {
                    GetHit(lastEnemyDamage, lastEnemyKnockbackStrength, ref lastEnemyKnockbackDirection, lastEnemy);
                }
            }
            else
            {
                invincibilityAccumulator += Time.fixedDeltaTime;
            }
            
        }
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0.01f ) //no character input for you!
        {
            return;
        }

        currentMoveVector = context.ReadValue<Vector2>();

        AnalyzeMovementVector();

        //when changing from vertical to horizontal direction (or vice versa), we should adjust the position to the closest half tile for QOL
        if (currentMoveDirection != previousMoveDirection) 
        {
            Vector2 adjustedPosition = rigidbody2d.position;
            ResetAnimationTrigger();
            switch (currentMoveDirection)
            {
                case MoveDirection.MOVE_UP:
                    heroAnimationController.SetTrigger("MoveUp");
                    adjustedPosition.x = Mathf.Round(rigidbody2d.position.x * 2) / 2;
                    rigidbody2d.position = adjustedPosition;
                    break;
                case MoveDirection.MOVE_DOWN:
                    heroAnimationController.SetTrigger("MoveDown");
                    adjustedPosition.x = Mathf.Round(rigidbody2d.position.x * 2) / 2;
                    rigidbody2d.position = adjustedPosition;

                    break;
                case MoveDirection.MOVE_LEFT:
                    heroAnimationController.SetTrigger("MoveLeft");
                    adjustedPosition.y = Mathf.Round(rigidbody2d.position.y * 2) / 2;
                    rigidbody2d.position = adjustedPosition;
                    break;
                case MoveDirection.MOVE_RIGHT:
                    heroAnimationController.SetTrigger("MoveRight");
                    adjustedPosition.y = Mathf.Round(rigidbody2d.position.y * 2) / 2;
                    rigidbody2d.position = adjustedPosition;
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

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    void OnScreenTransisiton(MoveDirection direction) 
    {
        ResetAnimationTrigger();
        previousMoveDirection = currentMoveDirection;
        currentMoveDirection = MoveDirection.MOVE_IDLE;
        currentMoveVector = Vector2.zero;
        heroAnimationController.updateMode = AnimatorUpdateMode.UnscaledTime;
        gameObject.GetComponent<Collider2D>().enabled = false;
        switch (direction)
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
            default:
                heroAnimationController.SetTrigger("Idle");
                break;
        }
        _ = StartCoroutine(MoveBetweenRooms(direction));

    }
    IEnumerator MoveBetweenRooms(MoveDirection direction)
    {
        float timeElapsed = 0f;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition;
        
        switch (direction)
        {
            case MoveDirection.MOVE_UP:
                targetPosition.x = Mathf.Round(targetPosition.x); //smooth out the position in the doorway
                targetPosition.y += 3.1f;
                break;
            case MoveDirection.MOVE_DOWN:
                targetPosition.x = Mathf.Round(targetPosition.x);
                targetPosition.y -= 3.1f;
                break;
            case MoveDirection.MOVE_LEFT:
                targetPosition.x -= 3.1f;
                targetPosition.y = Mathf.Round(targetPosition.y); //left and right doorways are at x.5 so need to round, determine if that's even, then add or subtract .5
                if ((int)targetPosition.y % 2 == 0)
                {
                    targetPosition.y += 0.5f;
                }
                else
                {
                    targetPosition.y -= 0.5f;
                }
                break;
            case MoveDirection.MOVE_RIGHT:
                targetPosition.x += 3.1f;
                targetPosition.y = Mathf.Round(targetPosition.y);
                if ((int)targetPosition.y % 2 == 0)
                {
                    targetPosition.y += 0.5f;
                }
                else
                {
                    targetPosition.y -= 0.5f;
                }
                break;
            default:
                targetPosition = new Vector2(0.0f, 0.0f);
                break;
        }


        while (timeElapsed < roomMoveTime) 
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, timeElapsed / roomMoveTime);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        gameObject.GetComponent<Collider2D>().enabled = true;


        currentMoveVector = m_playerInput.actions["Move"].ReadValue<Vector2>();
        AnalyzeMovementVector();


        if (currentMoveDirection != previousMoveDirection)
        {
            ResetAnimationTrigger();
        }
        
        //heroAnimationController.SetTrigger("Idle");
        heroAnimationController.updateMode = AnimatorUpdateMode.Normal;

        
    }

    void ResetAnimationTrigger()
    {
        switch (previousMoveDirection)
        {
            case MoveDirection.MOVE_UP:
                heroAnimationController.ResetTrigger("MoveUp");
                break;
            case MoveDirection.MOVE_DOWN:
                heroAnimationController.ResetTrigger("MoveDown");
                break;
            case MoveDirection.MOVE_LEFT:
                heroAnimationController.ResetTrigger("MoveLeft");
                break;
            case MoveDirection.MOVE_RIGHT:
                heroAnimationController.ResetTrigger("MoveRight");
                break;
            case MoveDirection.MOVE_IDLE:
                heroAnimationController.ResetTrigger("Idle");
                break;
            default:
                heroAnimationController.ResetTrigger("Idle");
                break;
        }
    }

    void AnalyzeMovementVector()
    {
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
    }

    public void GetHit(int incomingDamage, KnockbackStrength knockbackDistance, ref Vector2 directionOfKnockback, GameObject enemyAttacking)
    {
        if (isInvincible)
        {
            lastEnemy = enemyAttacking;
            lastEnemyDamage = incomingDamage;
            lastEnemyKnockbackDirection = directionOfKnockback;
            lastEnemyKnockbackStrength = knockbackDistance;
            return;
        }
        else
        {
            currentState = CHARACTER_STATE.KNOCKBACK;
            knockbackStrength = knockbackDistance;
            remainingKnockbackFrames = knockbackFrames;
            knockbackDirection = directionOfKnockback;
            isInvincible = true;
            gameObject.GetComponentInChildren<HeroInvincibilityEffect>().StartRunning();
        }
            
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == lastEnemy)
        {
            isStillTouching = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == lastEnemy)
        {
            isStillTouching = false;
        }
    }

    private enum CHARACTER_STATE
    {
        NORMAL,
        KNOCKBACK,
        DYING,
        DEAD
    }
}

public enum MoveDirection
{
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    MOVE_IDLE
}

public enum KnockbackStrength
{
    WEAK = 1,
    STRONG = 3
}
