using UnityEngine;

public class RoomNumberUI : MonoBehaviour
{
    [SerializeField] private GameObject[] visualRoomNumberArray;

    private int roomNumber;
    private void Awake()
    {
        roomNumber = RoomsSpawnManager.Instance.GetSpawnedRoomCount();
    }
    private void Show()
    {
        foreach (var visualRoomNumber in visualRoomNumberArray)
        {
            visualRoomNumber.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (var visualRoomNumber in visualRoomNumberArray)
        {
            visualRoomNumber.SetActive(false);
        }
    }
}

