using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room/RoomListSO")]
public class RoomListSO : ScriptableObject
{
    public List<RoomSO> roomSOList;

    public RoomDirection roomDirection;
}
