using HighlightPlus;
using UnityEngine;

public class SelectedItemVisual : MonoBehaviour
{
    [SerializeField] private HighlightEffect highLightEffect;
    [SerializeField] private ItemObject itemObject;

    private void Awake()
    {
        Hide();
        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
    }
    private void Start()
    {
        Hide();
    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e)
    {
        if (e.interactable == itemObject.GetItemObjectParent())
        {
            //Debug.Log(e.interactable);
            Show();

        } else
        {
            Hide();
        }
    }

    private void Show()
    {
        highLightEffect.SetHighlighted(true);
    }

    private void Hide()
    {
        highLightEffect.SetHighlighted(false);
    }
}
