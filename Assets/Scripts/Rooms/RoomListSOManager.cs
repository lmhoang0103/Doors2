using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomListSOManager : MonoBehaviour
{
    public List<RoomSO> roomSOList; // Reference to the RoomSO
    public RoomListSO straightRoomListSO;
    public RoomListSO leftRoomListSO;
    public RoomListSO rightRoomListSO;

    [Button]
    private void AddRoomSOToRoomListSO()
    {
        // Find the corresponding RoomListSO based on the room direction
        foreach (RoomSO roomSO in roomSOList)
        {
            switch (roomSO.roomDirection)
            {
                case RoomDirection.Straight:
                    if (!straightRoomListSO.roomSOList.Contains(roomSO))
                        straightRoomListSO.roomSOList.Add(roomSO);
                    break;
                case RoomDirection.Left:
                    if (!leftRoomListSO.roomSOList.Contains(roomSO))
                        leftRoomListSO.roomSOList.Add(roomSO);

                    break;
                case RoomDirection.Right:
                    if (!rightRoomListSO.roomSOList.Contains(roomSO))
                        rightRoomListSO.roomSOList.Add(roomSO);
                    break;
            }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(straightRoomListSO);
        EditorUtility.SetDirty(leftRoomListSO);
        EditorUtility.SetDirty(rightRoomListSO);
        AssetDatabase.SaveAssets();
#endif
    }

    [Button]
    private void ClearRoomListSO()
    {
        straightRoomListSO.roomSOList.Clear();
        leftRoomListSO.roomSOList.Clear();
        rightRoomListSO.roomSOList.Clear();

#if UNITY_EDITOR
        EditorUtility.SetDirty(straightRoomListSO);
        EditorUtility.SetDirty(leftRoomListSO);
        EditorUtility.SetDirty(rightRoomListSO);
        AssetDatabase.SaveAssets();
#endif
    }
}
