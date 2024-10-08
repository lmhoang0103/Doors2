using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sigtrap.Relays;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DemoTabButton : HCMonoBehaviour
{
    [SerializeField] private Transform graphic, selectedTrans, notSelectedTrans;
    [SerializeField] private DemoTabContent content;
    
    private Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void SetSelected(bool selected)
    {
        _btn.interactable = !selected;

        graphic.DOKill();
        Vector3 targetPos = selected ? selectedTrans.position : notSelectedTrans.position;
        graphic.DOMove(targetPos, .2f);

        if (selected)
        {
            transform.SetAsLastSibling();
            content.Show();
        }
        else
            content.Hide();
    }
}
