using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnRoomData", order = 1)]

public class RoomDataSO : ScriptableObject
{
    [SerializeField]
    public Vector2 mapDimensions;
    [SerializeField]
    public List<Room> rooms;

}
[Serializable]
public class Room
{
    [SerializeField]
    public Vector2 roomPosition; //relative to map/chunk
    [SerializeField]
    public Vector2 cameraPosition; //transform for moving/verifying camera position
    [SerializeField]
    public List<Door> doors;
    [SerializeField]
    public List <Enemy> enemies;
    [SerializeField]
    public List<Item> items;
    [SerializeField]
    public List<Stairs> stairs;
}
[Serializable]
public class Door
{
    [SerializeField]
    public Vector2 doorLocation; //global space to grab/change door sprite
    [SerializeField]
    public DOOR_TYPE doorType;
}

public enum DOOR_TYPE
{
    OPEN,
    SHUTTER,
    LOCKED
}
[Serializable]
public class Enemy
{
    [SerializeField]
    public GameObject enemyPrefab;
    [SerializeField]
    public Vector2 enemySpawnLocation; //global space
    [SerializeField]
    public GameObject enemyCarriedItem;
}
[Serializable]
public class Item
{
    [SerializeField]
    public GameObject itemPrefab;
    [SerializeField]
    public ITEM_SPAWN_CONDITION itemSpawn;
    [SerializeField]
    public Vector2 itemSpawnLocation; //global space
}

public enum ITEM_SPAWN_CONDITION
{
    SPAWN_ON_ENTER,
    SPAWN_ON_ROOM_CLEAR,
    SPAWN_ON_PUZZLE_SOLVE
}

[Serializable]
public class Stairs
{
    [SerializeField]
    public Vector2 stairEntrance; //global space
    [SerializeField]
    public Vector2 stairExit; //global space
}