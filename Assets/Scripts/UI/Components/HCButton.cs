
using UnityEngine;
using UnityEngine.UI;

public class HCButton : Button
{
    public ButtonAnim buttonAnim;
    
    public void Show(bool instant = false)
    {
        gameObject.SetActive(true);

        if (!instant && buttonAnim)
            buttonAnim.ShowAnim();
    }

    public void Hide(bool instant = false)
    {
        if (instant || !buttonAnim)
            gameObject.SetActive(false);
        else
            buttonAnim.HideAnim(() => gameObject.SetActive(false));
    }

    [ContextMenu("Setup")]
    void Setup()
    {
        buttonAnim = GetComponent<ButtonAnim>();
    }
}