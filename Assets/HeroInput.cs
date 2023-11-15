using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroInput : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10.0f;
    private Rigidbody2D rigidbody2d;
    private Vector2 currentMoveDirection = new Vector2(0.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + (currentMoveDirection * moveSpeed * Time.fixedDeltaTime));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMoveDirection = context.ReadValue<Vector2>().normalized;
    }
}
