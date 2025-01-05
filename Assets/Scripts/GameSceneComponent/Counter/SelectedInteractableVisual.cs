using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

public class SelectedInteractableVisual : MonoBehaviour {
    [SerializeField] private HighlightEffect highLightEffect;
    [SerializeField] private GameObject hasInteractableGameObject;

    private IInteractable interactable;

    private void Awake() {
        highLightEffect = GetComponent<HighlightEffect>();
        if (transform.parent != null) {
            hasInteractableGameObject = transform.parent.gameObject;
            if (!hasInteractableGameObject.TryGetComponent(out interactable)) {
                Debug.LogError($"GameObject {hasInteractableGameObject.name} does not have a component that implements the IInteractable interface.");
            }
        }
    }
    private void Start() {
        Hide();
        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e) {
        if (e.interactable == interactable) {
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
