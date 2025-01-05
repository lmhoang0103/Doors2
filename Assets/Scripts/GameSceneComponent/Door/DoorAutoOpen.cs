public class DoorAutoOpen : BaseDoor, ITriggerableByPlayer {
    protected override void Awake() {
        base.Awake();
    }
    public void Trigger(Player player) {
        OpenDoor();
    }
}
