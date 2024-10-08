using HighlightPlus;
using UnityEngine;

public class ItemVisualPlacementPreview : MonoBehaviour
{
    [SerializeField] private HighlightEffect highLightEffect;
    [SerializeField] private ItemObject itemObject;

    private void Awake()
    {
        itemObject.OnEnableItemPlacementPreview += ItemObject_OnEnableItemPlacementPreview;
        Hide();
    }

    private void ItemObject_OnEnableItemPlacementPreview(object sender, System.EventArgs e)
    {
        Show();
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
