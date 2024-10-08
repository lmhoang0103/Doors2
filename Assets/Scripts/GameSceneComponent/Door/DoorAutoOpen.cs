public class DoorAutoOpen : BaseDoor, ITriggerableByPlayer
{
    public void Trigger(Player player)
    {
        OpenDoor();
    }
}
