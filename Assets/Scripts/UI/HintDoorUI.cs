using UnityEngine;

public class HintDoorUI : MonoBehaviour
{
    [SerializeField] private GameObject[] visualHintUIArray;
    [SerializeField] private GameObject hintableGameObject;
    private IHintable hintableObject;

    private void Start()
    {
        if (!hintableGameObject.TryGetComponent(out hintableObject))
        {
            Debug.LogError($"GameObject {hintableGameObject.name} does not have a component that implements the IHintable interface.");
        } else
        {
            hintableObject.OnHintHidden += DoorUnlockedByKey_OnTriggerHintHidden;
            hintableObject.OnHintShow += DoorUnlockedByKey_OnTriggerHintShow;
        }
        Hide();
    }

    private void DoorUnlockedByKey_OnTriggerHintShow(object sender, System.EventArgs e)
    {
        Show();
    }

    private void DoorUnlockedByKey_OnTriggerHintHidden(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        foreach (GameObject visualHint in visualHintUIArray)
        {
            visualHint.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualHint in visualHintUIArray)
        {
            visualHint.SetActive(false);
        }
    }

}
