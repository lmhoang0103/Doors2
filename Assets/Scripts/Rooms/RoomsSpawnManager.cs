using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsSpawnManager : SingletonDestroy<RoomsSpawnManager> {
    private const string NEW_ROOM_SPAWN_POINT = "NewRoomSpawnPoint";

    public event EventHandler OnRoomSpawn;

    [SerializeField] private Transform currentRoomsTransform;
    [SerializeField] private Room startRoom;
    [SerializeField] private RoomListSO straightRoomListSO;
    [SerializeField] private RoomListSO leftRoomListSO;
    [SerializeField] private RoomListSO rightRoomListSO;
    [SerializeField] private int maxRoomsExist = 3;

    private Queue<Room> activeRoomsQueue = new Queue<Room>();
    private Vector3 nextRoomSpawnPoint;

    private int roomCount;
    private List<RoomSO> allRoomList = new List<RoomSO>();
    private List<RoomSO> leftRoomList = new List<RoomSO>();
    private List<RoomSO> rightRoomList = new List<RoomSO>();
    private RoomDirection currentRoomDirection;


    private void Start() {
        roomCount = 1;
        nextRoomSpawnPoint = startRoom.GetNewRoomSpawnPosition();
        currentRoomDirection = RoomDirection.Straight;
        activeRoomsQueue.Enqueue(startRoom);

        allRoomList = straightRoomListSO.roomSOList.Concat(leftRoomListSO.roomSOList).Concat(rightRoomListSO.roomSOList).ToList();
        leftRoomList = straightRoomListSO.roomSOList.Concat(leftRoomListSO.roomSOList).ToList();
        rightRoomList = straightRoomListSO.roomSOList.Concat(rightRoomListSO.roomSOList).ToList();

        SpawnRoom();
    }

    [Button]
    public void SpawnRoom() {
        List<RoomSO> choosenRoomSOList = new List<RoomSO>();
        Vector3 spawnRoomEulerAngles = new Vector3();

        //Choose What type of rooms to spawn
        switch (currentRoomDirection) {
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
        int randomIndex = UnityEngine.Random.Range(0, choosenRoomSOList.Count);
        roomCount++;
        Room spawnedRoom = Room.SpawnRoomSO(choosenRoomSOList[randomIndex], currentRoomsTransform, nextRoomSpawnPoint, spawnRoomEulerAngles, roomCount);
        activeRoomsQueue.Enqueue(spawnedRoom);
        //Cache neccessary info
        nextRoomSpawnPoint = spawnedRoom.GetNewRoomSpawnPosition();
        RoomDirection roomDir = spawnedRoom.GetRoomDirection();
        currentRoomDirection = (RoomDirection)(((int)currentRoomDirection + (int)roomDir) % 4);
        if (activeRoomsQueue.Count > maxRoomsExist) {
            DestroyOldestRoom();
        }

        OnRoomSpawn?.Invoke(this, EventArgs.Empty);


    }

    private void DestroyOldestRoom() {
        Room oldestRoom = activeRoomsQueue.Dequeue();
        oldestRoom.DestroySelf();

        if (activeRoomsQueue.Count > 0) {
            Room nextRoom = activeRoomsQueue.Peek(); // Peek at the next room
            nextRoom.ShowBlockedWall(); // Block off the exit
        }
    }



    public int GetSpawnedRoomCount() {
        return roomCount;
    }
}
