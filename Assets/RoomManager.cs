using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private RoomDataSO roomData;
    private Room currentRoom;
    private List<RoomTracking> exploredRooms = new();
    private int currentActiveRoom;
    private Vector2 currentCoordinates;
    // Start is called before the first frame update
    void Start()
    {
        DoorMovementTrigger.OnScreenTransition += OnScreenTransition;

        currentRoom = roomData.rooms[16];
        List<GameObject> newEnemies = new();
        RoomTracking newRoom = new()
        {
            roomCoordinates = currentRoom.roomPosition,
            isRoomActive = true,
            activeEnemies = newEnemies
        };
        exploredRooms.Add(newRoom);
        currentCoordinates = currentRoom.roomPosition;
    }

    private void OnDisable()
    {
        DoorMovementTrigger.OnScreenTransition -= OnScreenTransition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnScreenTransition(MOVEDIRECTION direction)
    {
        Vector2 newRoomPosition = new Vector2();
        switch (direction) //using map coordinates as a key, so calculating where the next room is
        {
            case MOVEDIRECTION.MOVE_UP:
                newRoomPosition = currentRoom.roomPosition + new Vector2(0, 1);
                break;
            case MOVEDIRECTION.MOVE_DOWN:
                newRoomPosition = currentRoom.roomPosition + new Vector2(0, -1);
                break;
            case MOVEDIRECTION.MOVE_LEFT:
                newRoomPosition = currentRoom.roomPosition + new Vector2(-1, 0);
                break;
            case MOVEDIRECTION.MOVE_RIGHT:
                newRoomPosition = currentRoom.roomPosition + new Vector2(1, 0);
                break;
        }
        
        if (roomData.rooms.Exists(match: x => x.roomPosition == newRoomPosition))
        {
            currentRoom = roomData.rooms.Find(x => x.roomPosition == newRoomPosition);
            currentCoordinates = currentRoom.roomPosition;
            if (!exploredRooms.Exists(match: x => x.roomCoordinates == currentCoordinates))
            {
                CreateNewExploredRoom();
                StartCoroutine(SpawnEnemies());
            }
            
        }
        else
        {
            //where are we going?!?
        }
        
    }

    private void CreateNewExploredRoom()
    {
        RoomTracking newRoom = new RoomTracking()
        {
            roomCoordinates = currentCoordinates,
            isRoomActive = true,
            activeEnemies = new List<GameObject>()
        };
        exploredRooms.Add(newRoom);
    }

    private IEnumerator SpawnEnemies()
    {
        RoomTracking exploredRoom = exploredRooms.Find(match: x => x.roomCoordinates == currentCoordinates);
        foreach (var enemy in currentRoom.enemies)
        {
            float randomInterval = Random.Range(0.0f, 0.5f);
            GameObject spawnedEnemy = Instantiate(enemy.enemyPrefab, enemy.enemySpawnLocation, Quaternion.identity);
            exploredRoom.activeEnemies.Add(spawnedEnemy);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    struct RoomTracking
    {
        public Vector2 roomCoordinates;
        public bool isRoomActive;
        public List<GameObject> activeEnemies;

    }
}


