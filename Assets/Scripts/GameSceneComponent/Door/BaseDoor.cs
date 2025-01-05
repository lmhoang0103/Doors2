using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
public class BaseDoor : MonoBehaviour {
    public event EventHandler OnDoorOpening;
    public event EventHandler OnDoorClosing;

    private BoxCollider boxCollider;
    private NavMeshObstacle navMeshObstacle;

    protected virtual void Awake() {

        boxCollider = GetComponent<BoxCollider>();

        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }


    protected void OpenDoor() {
        boxCollider.enabled = false;
        navMeshObstacle.enabled = false;
        OnDoorOpening?.Invoke(this, EventArgs.Empty);
        RoomsSpawnManager.Instance.SpawnRoom();
        float volume = 1f;
        SoundManager.Instance.PlayDoorknobSound(Camera.main.transform.position, volume);
    }

    protected void CloseDoor() {
        boxCollider.enabled = true;
        navMeshObstacle.enabled = true;
        OnDoorClosing?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OpenDoorNotSpawnRoom() {
        boxCollider.enabled = false;
        navMeshObstacle.enabled = false;
        float volume = 1f;

        SoundManager.Instance.PlayDoorknobSound(Camera.main.transform.position, volume);
        OnDoorOpening?.Invoke(this, EventArgs.Empty);
    }
}
