using System;
using UnityEngine;
using UnityEngine.AI;
public class BaseDoor : MonoBehaviour
{
    public event EventHandler OnDoorOpening;
    public event EventHandler OnDoorClosing;

    [SerializeField] public BoxCollider boxCollider;
    [SerializeField] public NavMeshObstacle navMeshObstacle;



    protected void OpenDoor()
    {
        boxCollider.enabled = false;
        navMeshObstacle.enabled = false;
        OnDoorOpening?.Invoke(this, EventArgs.Empty);
        RoomsSpawnManager.Instance.SpawnRoom();
    }

    protected void CloseDoor()
    {
        boxCollider.enabled = true;
        navMeshObstacle.enabled = true;
        OnDoorClosing?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OpenDoorNotSpawnRoom()
    {
        boxCollider.enabled = false;
        navMeshObstacle.enabled = false;
        OnDoorOpening?.Invoke(this, EventArgs.Empty);
    }
}
