using DG.Tweening;
using UnityEngine;

public class SwitchTweenAnimator : MonoBehaviour
{
    [SerializeField] private Switch parentSwitch;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Transform switchTrigger;
    private void Awake()
    {
        //if (parentSwitch != null)
        {
            parentSwitch.OnSwitchTurnOn += ParentSwitch_OnSwitchTurnOn;
            parentSwitch.OnswitchTurnOff += ParentSwitch_OnswitchTurnOff;
        }
    }

    private void ParentSwitch_OnswitchTurnOff(object sender, System.EventArgs e)
    {
        PlaySwitchTurnOffAnim();
    }

    private void ParentSwitch_OnSwitchTurnOn(object sender, System.EventArgs e)
    {
        PlaySwitchTurnOnAnim();
    }

    private void PlaySwitchTurnOnAnim()
    {
        Vector3 switchTurnOnRotationValue = new Vector3(0, 0, 0);
        switchTrigger.DOLocalRotate(switchTurnOnRotationValue, animationDuration).SetEase(Ease.OutBack);
    }


    private void PlaySwitchTurnOffAnim()
    {
        Vector3 switchTurnOffRotationValue = new Vector3(-90, 0, 0);
        switchTrigger.DOLocalRotate(switchTurnOffRotationValue, animationDuration).SetEase(Ease.OutBack);
    }
}
