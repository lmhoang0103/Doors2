using DG.Tweening;
using UnityEngine;

public class LeverTweenAnimator : MonoBehaviour {
    [SerializeField] private Lever parentLever;
    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private Transform leverHandle;

    private void Awake() {
        if (parentLever != null) {
            parentLever.OnLeverTurnOn += ParentLever_OnLeverTurnOn;
        }
    }

    private void ParentLever_OnLeverTurnOn(object sender, System.EventArgs e) {
        PlayLeverTurnOnAnim();
    }

    private void PlayLeverTurnOnAnim() {
        Vector3 leverTurnOnRotationValue = new Vector3(47.98f, 0, 0);
        leverHandle.DOLocalRotate(leverTurnOnRotationValue, animationDuration).SetEase(Ease.OutQuad);
    }

    private void PlayLeverTurnOffAnim() {

    }
}
