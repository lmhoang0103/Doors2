using UnityEngine;

public class BananaPeelObstacle : MonoBehaviour, ITriggerableByPlayer
{
    public void Trigger(Player player)
    {
        player.OnSlippingMovement();
        Destroy(gameObject);
    }
}
