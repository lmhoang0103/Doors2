using TMPro;
using UnityEngine;

public class Room : MonoBehaviour {
    private const string NEW_ROOM_SPAWN_POINT = "NewRoomSpawnPoint";

    [SerializeField] private RoomSO roomSO;
    [SerializeField] private TextMeshProUGUI roomNumberText;
    [SerializeField] private GameObject blockedWall;
    public RoomSO GetRoomSO() { return roomSO; }

    private void Start() {
        HideBlockedWall();
    }
    public static Room SpawnRoomSO(RoomSO roomSO, Transform parentTransform, Vector3 roomSpawnPosition, Vector3 roomSpawnEulerAngle, int roomNumber) {
        //SpawnRoom
        Transform roomTransform = Instantiate(roomSO.roomPrefab);
        Room room = roomTransform.GetComponent<Room>();

        roomTransform.parent = parentTransform;
        roomTransform.localPosition = roomSpawnPosition;
        roomTransform.localEulerAngles = roomSpawnEulerAngle;

        //Set Room Number
        room.SetRoomNumberText(roomNumber);
        return room;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public Vector3 GetNewRoomSpawnPosition() {
        Transform spawnPointTransform = transform.Find(NEW_ROOM_SPAWN_POINT);
        if (spawnPointTransform != null) {
            return spawnPointTransform.position;
        } else {
            Debug.LogError("SpawnPoint not found for " + transform.name);
            UnityEngine.Application.Quit(); // Stop the game and exit the application
            return Vector3.zero;
        }
    }

    public RoomDirection GetRoomDirection() {
        return roomSO.roomDirection;
    }
    private void SetRoomNumberText(int roomNumber) {
        if (roomNumberText == null) return;
        roomNumberText.text = roomNumber.ToString();
    }

    public void ShowBlockedWall() {
        if (blockedWall == null) { return; }
        blockedWall.SetActive(true);
    }
    public void HideBlockedWall() {
        if (blockedWall == null) { return; }
        blockedWall.SetActive(false);
    }
}
