using UnityEngine;

public class BananaPeelObstacle : MonoBehaviour, ITriggerableByPlayer {
    private void Awake() {
        // 50% chance to destroy the object on initialization
        if (Random.value > 0.5f) {
            Destroy(gameObject);
        }
    }
    public void Trigger(Player player) {
        player.OnSlippingMovement();
        Destroy(gameObject);
    }
}
