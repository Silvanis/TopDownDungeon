using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private RoomDataSO roomData;
    private Room currentRoom;
    private List<RoomTracking> exploredRooms = new();
    private int currentActiveRoom;
    private Vector2 currentCoordinates;
    public static RoomManager _instance;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        DoorMovementTrigger.OnScreenTransition += OnScreenTransition;

        currentRoom = roomData.rooms[16];
        List<Enemy> newEnemies = new();
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
            currentActiveRoom = exploredRooms.FindIndex(match: x => x.roomCoordinates == currentCoordinates);
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
            activeEnemies = new List<Enemy>()
        };
        exploredRooms.Add(newRoom);
    }

    private IEnumerator SpawnEnemies()
    {
        RoomTracking exploredRoom = exploredRooms.Find(match: x => x.roomCoordinates == currentCoordinates);
        foreach (var enemy in currentRoom.enemies)
        {
            
            float randomInterval = Random.Range(0.0f, 0.5f);
            GameObject enemyObject = Instantiate(enemy.enemyPrefab, enemy.enemySpawnLocation, Quaternion.identity);
            Enemy spawnedEnemy = new Enemy() { enemyPrefab = enemyObject, enemySpawnLocation = enemy.enemySpawnLocation, enemyCarriedItem = enemy.enemyCarriedItem };
            exploredRoom.activeEnemies.Add(spawnedEnemy);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        Transform location = enemy.transform;
        int enemyIndex = exploredRooms[currentActiveRoom].activeEnemies.FindIndex(match: x => x.enemyPrefab == enemy);
        GameObject item = exploredRooms[currentActiveRoom].activeEnemies[enemyIndex].enemyCarriedItem;
        if ( item != null)
        {
            Instantiate(item, location.position, Quaternion.identity);
        }
        else
        {
            //roll for random item drop
        }
        exploredRooms[currentActiveRoom].activeEnemies.RemoveAt(enemyIndex);

    }

    struct RoomTracking
    {
        public Vector2 roomCoordinates;
        public bool isRoomActive;
        public List<Enemy> activeEnemies;

    }
}


