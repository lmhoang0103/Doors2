using UnityEngine;

public class InteractButtonUI : MonoBehaviour
{
    private void Awake()
    {
        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
        Hide();
    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e)
    {
        if (e.interactable != null)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
