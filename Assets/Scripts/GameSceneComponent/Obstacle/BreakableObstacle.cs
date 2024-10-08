using UnityEngine;

public class BreakableObstacle : MonoBehaviour, IInteractable
{
    public bool IsInteractable(Player player)
    {
        if (player.HasItemObject())
        {
            if (player.GetItemObject().GetItemObjectSO().itemType == ItemType.Hammer)
            {
                return true;
            }
        }
        return false;
    }
    public void Interact(Player player)
    {
        Destroy(gameObject);
        player.GetItemObject().DestroySelf();
    }


    public void Break()
    {
        Destroy(gameObject);
    }
}
