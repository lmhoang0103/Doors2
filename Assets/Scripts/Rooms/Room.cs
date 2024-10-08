using UnityEngine;

public class Room : MonoBehaviour
{
    private const string NEW_ROOM_SPAWN_POINT = "NewRoomSpawnPoint";

    [SerializeField] private RoomSO roomSO;
    private int roomNumber;
    public RoomSO GetRoomSO() { return roomSO; }

    private void Awake()
    {
        roomNumber = RoomsSpawnManager.Instance.GetSpawnedRoomCount();
    }
    public static Room SpawnRoomSO(RoomSO roomSO, Transform parentTransform, Vector3 roomSpawnPosition, Vector3 roomSpawnEulerAngle)
    {
        Transform roomTransform = Instantiate(roomSO.roomPrefab);
        Room room = roomTransform.GetComponent<Room>();

        roomTransform.parent = parentTransform;
        roomTransform.localPosition = roomSpawnPosition;
        roomTransform.localEulerAngles = roomSpawnEulerAngle;
        return room;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public Vector3 GetNewRoomSpawnPosition()
    {
        Transform spawnPointTransform = transform.Find(NEW_ROOM_SPAWN_POINT);
        if (spawnPointTransform != null)
        {

            return spawnPointTransform.position;
        } else
        {
            Debug.LogError("SpawnPoint not found for " + transform.name);
            UnityEngine.Application.Quit(); // Stop the game and exit the application
            return Vector3.zero;
        }
    }
}
