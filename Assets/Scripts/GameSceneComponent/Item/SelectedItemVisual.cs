using HighlightPlus;
using UnityEngine;

public class SelectedItemVisual : MonoBehaviour {
    private HighlightEffect highLightEffect;
    private ItemObject itemObject;

    private void Awake() {
        highLightEffect = GetComponent<HighlightEffect>();
        itemObject = GetComponent<ItemObject>();
    }

    private void Start() {
        Hide();
        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e) {
        if (e.interactable == itemObject.GetItemObjectParent()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        if (highLightEffect != null) {
            highLightEffect.SetHighlighted(true);
        }
    }

    private void Hide() {
        if (highLightEffect != null) {
            highLightEffect.SetHighlighted(false);
        }
    }
}
