using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorTweenAnimator : MonoBehaviour {

    private BaseDoor baseDoor;

    private void Start() {
        baseDoor = GetComponent<BaseDoor>();
        if (baseDoor != null) {

            baseDoor.OnDoorClosing += BaseDoor_OnDoorClosing;
            baseDoor.OnDoorOpening += BaseDoor_OnDoorOpening;
        }
    }


    private void BaseDoor_OnDoorOpening(object sender, System.EventArgs e) {
        DoorOpenAnimPlay();
    }


    private void BaseDoor_OnDoorClosing(object sender, System.EventArgs e) {
        DoorCloseAnimPlay();
    }

    private void DoorOpenAnimPlay() {
        Vector3 openRotateAngle = new Vector3(0, -110, 0);
        float timeToMoveDoor = 0.3f;
        transform.DOLocalRotate(openRotateAngle, timeToMoveDoor).SetEase(Ease.InOutQuad);
    }

    private void DoorCloseAnimPlay() {
        Vector3 closeRotateAngle = new Vector3(0, 0, 0);
        float timeToMoveDoor = 0.3f;
        transform.DOLocalRotate(closeRotateAngle, timeToMoveDoor).SetEase(Ease.OutBounce);
    }
}
