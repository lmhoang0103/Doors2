public class DoorAutoOpenNotSpawnRoom : BaseDoor, ITriggerableByPlayer
{
    public void Trigger(Player player)
    {
        OpenDoorNotSpawnRoom();
    }

}
