using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    [SerializeField]
    private float roomMoveTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        DoorMovementTrigger.OnScreenTransition += OnScreenTransition;
    }

    private void OnDisable()
    {
        DoorMovementTrigger.OnScreenTransition -= OnScreenTransition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnScreenTransition(MoveDirection direction)
    {
        Time.timeScale = 0.0f;
        _ = StartCoroutine(MoveCameraBetweenRooms(direction));
    }

    IEnumerator MoveCameraBetweenRooms(MoveDirection direction)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition;
        switch (direction)
        {
            case MoveDirection.MOVE_UP:
                targetPosition += new Vector3(0.0f, 11.0f, 0.0f);
                break;
            case MoveDirection.MOVE_DOWN:
                targetPosition += new Vector3(0.0f, -11.0f, 0.0f);
                break;
            case MoveDirection.MOVE_LEFT:
                targetPosition += new Vector3(-16.0f, 0.0f, 0.0f);
                break;
            case MoveDirection.MOVE_RIGHT:
                targetPosition += new Vector3(16.0f, 0.0f, 0.0f);
                break;
            default:
                targetPosition += new Vector3(0.0f, 0.0f, 0.0f);
                break;
        }

        while (timeElapsed < roomMoveTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / roomMoveTime);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        Time.timeScale = 1.0f;
    }
}
