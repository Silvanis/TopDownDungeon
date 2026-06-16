using System.Collections;
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
    [SerializeField]
    private GameObject spawnCloud;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private int initialHP;
    private int currentHP;
    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private Vector2 currentMoveVector = Vector2.zero;
    private bool reachedPosition = false;
    private MOVEDIRECTION currentDirection = MOVEDIRECTION.MOVE_IDLE;
    private float distanceTravelled = 0.0f;
    private ENEMY_STATE currentState;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private AudioClip enemyHitSound;
    [SerializeField]
    private AudioClip enemyDyingSound;
    private AudioSource sounds;
    private float soundVolume = 0.45f;

    //---Knockback---
    [SerializeField]
    private int knockbackFrames = 3;
    [SerializeField]
    private int remainingKnockbackFrames = 0;
    private Vector2 knockbackDirection;
    private KnockbackStrength knockbackStrength = KnockbackStrength.STRONG;
    
    // Start is called before the first frame update
    void Start()
    {
        
        animator = gameObject.GetComponentInChildren<Animator>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        sounds = gameObject.GetComponent <AudioSource>();
        currentState = ENEMY_STATE.ENEMY_STATE_SPAWNING;
        spriteRenderer.enabled = false;
        currentHP = initialHP;
        StartCoroutine(SpawnEffects());
    }

    private void Awake()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        //ChangePosition();
    }
    // Update is called once per frame
    void Update()
    {
        rigidbody2d.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (currentState == ENEMY_STATE.ENEMY_STATE_MOVING)
        {
            float distanceIncrement = movementSpeed * Time.fixedDeltaTime;
            distanceTravelled += distanceIncrement;
            CheckForReachedPosition();
            if (reachedPosition)
            {
                ChangePosition();
            }
            rigidbody2d.MovePosition(rigidbody2d.position + (currentMoveVector * distanceIncrement));
            return;
        }

        else if (currentState == ENEMY_STATE.ENEMY_STATE_SPAWNING)
        {
            return;
        }

        else if (currentState == ENEMY_STATE.ENEMY_STATE_KNOCKBACK)
        {
            if (remainingKnockbackFrames > 0)
            {
                float movementDistance = (float)knockbackStrength / (float)knockbackFrames;
                rigidbody2d.MovePosition(rigidbody2d.position + (knockbackDirection * movementDistance));
                remainingKnockbackFrames--;
            }
            else
            {
                currentState = ENEMY_STATE.ENEMY_STATE_IDLE;
                ChangePosition();
            }
            return;
        }

        else if (currentState == ENEMY_STATE.ENEMY_STATE_IDLE) 
        { 
            return; 
        }

        else if (currentState == ENEMY_STATE.ENEMY_STATE_DYING)
        {
            return;
        }

        else if (currentState == ENEMY_STATE.ENEMY_STATE_DEAD)
        {
            Destroy(gameObject);
        }



        
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
        MOVEDIRECTION newDirection = RollDirection();
        LayerMask mask = LayerMask.GetMask("Default", "Walls", "BlocksEnemy"); 
        while(!validDirection) 
        {

            switch (newDirection)
            {
                case MOVEDIRECTION.MOVE_IDLE:
                    currentMoveVector = Vector2.zero;
                    break;
                case MOVEDIRECTION.MOVE_UP:
                    currentMoveVector = Vector2.up;
                    break;
                case MOVEDIRECTION.MOVE_DOWN:
                    currentMoveVector = Vector2.down;
                    break;
                case MOVEDIRECTION.MOVE_LEFT:
                    currentMoveVector = Vector2.left;
                    break;
                case MOVEDIRECTION.MOVE_RIGHT:
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
        currentState = ENEMY_STATE.ENEMY_STATE_MOVING;
        ChangeFacing();
    }

    private void ChangeFacing()
    {
        switch (currentDirection)
        {
            case MOVEDIRECTION.MOVE_IDLE:
                animator.SetTrigger("Idle");
                break;
            case MOVEDIRECTION.MOVE_UP:
                animator.SetTrigger("MoveUp");
                break;
            case MOVEDIRECTION.MOVE_DOWN:
                animator.SetTrigger("MoveDown");
                break;
            case MOVEDIRECTION.MOVE_LEFT:
                animator.SetTrigger("MoveLeft");
                break;
            case MOVEDIRECTION.MOVE_RIGHT:
                animator.SetTrigger("MoveRight");
                break;
        }
    }

    private MOVEDIRECTION RollDirection()
    {
        MOVEDIRECTION newDirection;
        //don't roll idle more than once
        if (currentDirection == MOVEDIRECTION.MOVE_IDLE) 
        {
            newDirection = (MOVEDIRECTION)UnityEngine.Random.Range(0, 4);
        }
        else
        {
            newDirection = (MOVEDIRECTION)UnityEngine.Random.Range(0, 5);
        }
        return newDirection;
    }

    private IEnumerator Idle(float idleTime)
    {
        currentState = ENEMY_STATE.ENEMY_STATE_IDLE;
        yield return new WaitForSeconds(idleTime);
        if (currentState == ENEMY_STATE.ENEMY_STATE_IDLE) //make sure we're still idle
        {
            distanceTravelled = 0;
            ChangePosition();
        }
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

    public void GetHit(HitPacket hitPacket)
    {
        currentHP -= hitPacket.damageAmount;
        if (currentHP <= 0)
        {
            currentState = ENEMY_STATE.ENEMY_STATE_DYING;
            animator.SetTrigger("Dying");
            rigidbody2d.simulated = false;
            sounds.PlayOneShot(enemyDyingSound, soundVolume);
            RoomManager._instance.OnEnemyDeath(gameObject);
            return;
        }
        currentState = ENEMY_STATE.ENEMY_STATE_KNOCKBACK;
        remainingKnockbackFrames = knockbackFrames;
        switch (hitPacket.knockbackDirection)
        {
            case MOVEDIRECTION.MOVE_IDLE:
                knockbackDirection = Vector2.zero;
                break;
            case MOVEDIRECTION.MOVE_UP:
                knockbackDirection = Vector2.up;
                break;
            case MOVEDIRECTION.MOVE_DOWN:
                knockbackDirection = Vector2.down;
                break;
            case MOVEDIRECTION.MOVE_LEFT:
                knockbackDirection = Vector2.left;
                break;
            case MOVEDIRECTION.MOVE_RIGHT:
                knockbackDirection = Vector2.right;
                break;
        }
        sounds.PlayOneShot(enemyHitSound, soundVolume);
    }

    private IEnumerator SpawnEffects()
    {
        GameObject spawnCloudInstance = null;
        spawnCloudInstance = Instantiate(spawnCloud, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(spawnTime);
        spriteRenderer.enabled = true;
        ChangePosition();
    }

    public void DyingAnimationEnded()
    {
        currentState = ENEMY_STATE.ENEMY_STATE_DEAD;
    }
}

public enum ENEMY_STATE
{
    ENEMY_STATE_SPAWNING,
    ENEMY_STATE_IDLE,
    ENEMY_STATE_MOVING,
    ENEMY_STATE_ATTACKING,
    ENEMY_STATE_KNOCKBACK,
    ENEMY_STATE_DYING,
    ENEMY_STATE_DEAD
}
