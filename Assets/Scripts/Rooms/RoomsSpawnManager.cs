using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsSpawnManager : SingletonDestroy<RoomsSpawnManager>
{
    private const string NEW_ROOM_SPAWN_POINT = "NewRoomSpawnPoint";

    [SerializeField] private Transform currentRoomsTransform;
    [SerializeField] private Room startRoom;
    [SerializeField] private RoomListSO straightRoomListSO;
    [SerializeField] private RoomListSO leftRoomListSO;
    [SerializeField] private RoomListSO rightRoomListSO;
    [SerializeField] private int maxRoomsExist = 3;

    private Queue<Room> currentRoomsQueue = new Queue<Room>();
    private Vector3 nextRoomSpawnPoint;
    private int spawnedRoomCount;
    private List<RoomSO> allRoomList = new List<RoomSO>();
    private List<RoomSO> leftRoomList = new List<RoomSO>();
    private List<RoomSO> rightRoomList = new List<RoomSO>();
    private RoomDirection currentRoomDirection;
    private void Start()
    {
        spawnedRoomCount = 1;
        nextRoomSpawnPoint = startRoom.GetNewRoomSpawnPosition();
        currentRoomDirection = RoomDirection.Straight;
        currentRoomsQueue.Enqueue(startRoom);

        allRoomList = straightRoomListSO.roomSOList.Concat(leftRoomListSO.roomSOList).Concat(rightRoomListSO.roomSOList).ToList();
        leftRoomList = straightRoomListSO.roomSOList.Concat(leftRoomListSO.roomSOList).ToList();
        rightRoomList = straightRoomListSO.roomSOList.Concat(rightRoomListSO.roomSOList).ToList();

        SpawnRoom();
    }

    [Button]
    public void SpawnRoom()
    {
        int randomIndex;
        RoomDirection roomDir;
        Room spawnedRoom;
        List<RoomSO> choosenRoomSOList = new List<RoomSO>();
        Vector3 spawnRoomEulerAngles = new Vector3();

        switch (currentRoomDirection)
        {
            case RoomDirection.Straight:

                choosenRoomSOList = allRoomList;
                spawnRoomEulerAngles = Vector3.zero;
                break;
            case RoomDirection.Left:

                choosenRoomSOList = rightRoomList;
                spawnRoomEulerAngles = new Vector3(0, -90, 0);
                break;
            case RoomDirection.Right:

                choosenRoomSOList = leftRoomList;
                spawnRoomEulerAngles = new Vector3(0, 90, 0);

                break;

        }

        //Choose a random room to spawn
        randomIndex = Random.Range(0, choosenRoomSOList.Count);
        spawnedRoom = Room.SpawnRoomSO(choosenRoomSOList[randomIndex], currentRoomsTransform, nextRoomSpawnPoint, spawnRoomEulerAngles);
        currentRoomsQueue.Enqueue(spawnedRoom);
        spawnedRoomCount++;
        nextRoomSpawnPoint = spawnedRoom.GetNewRoomSpawnPosition();
        roomDir = GetRoomDirection(choosenRoomSOList[randomIndex]);
        currentRoomDirection = (RoomDirection)(((int)currentRoomDirection + (int)roomDir) % 4);
        if (currentRoomsQueue.Count > maxRoomsExist)
        {
            DestroyRoom();
        }


    }

    private void DestroyRoom()
    {
        Room oldestRoom = currentRoomsQueue.Dequeue();
        oldestRoom.DestroySelf();
    }

    private RoomDirection GetRoomDirection(RoomSO roomSO)
    {
        return roomSO.roomDirection;
    }

    public int GetSpawnedRoomCount()
    {
        return spawnedRoomCount;
    }
}
