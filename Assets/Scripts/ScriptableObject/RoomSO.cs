using UnityEngine;

[CreateAssetMenu(menuName = "Room/RoomSO")]
public class RoomSO : ScriptableObject
{
    public Transform roomPrefab;
    public string roomName;
    public RoomDirection roomDirection;
}
