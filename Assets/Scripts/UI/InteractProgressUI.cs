using UnityEngine;
using UnityEngine.UI;

public class InteractProgressUI : MonoBehaviour
{
    [SerializeField] private Image interactProgressImage;

    private void Start()
    {
        Player.Instance.OnInteractProgressChanged += Player_OnInteractProgressChanged;
        interactProgressImage.fillAmount = 0f;

    }

    private void Player_OnInteractProgressChanged(object sender, Player.OnInteractProgressChangedEventArgs e)
    {
        interactProgressImage.fillAmount = e.progressNormalized;
    }

}
