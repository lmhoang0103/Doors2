public class DoorInteractToOpen : BaseDoor, IInteractable
{

    public void Interact(Player player)
    {
        OpenDoor();
    }

    public bool IsInteractable(Player player)
    {
        return true;
    }
}
