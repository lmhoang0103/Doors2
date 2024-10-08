
using System.Collections.Generic;
using UnityEngine;

public abstract class DemoTabContent : HCMonoBehaviour
{
    [SerializeField] protected Transform content;
    [SerializeField] protected HCButton itemPrefab;

    protected List<HCButton> items = new List<HCButton>();
    
    public virtual void Show()
    {
        gameObject.SetActive(true);
        
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        
        items.Clear();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}