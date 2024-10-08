using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KeyStatus : HCMonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<RectTransform> keys;

    private Camera _mainCamera;

    private void OnEnable()
    {
        canvasGroup.alpha = 0;
        _mainCamera = Camera.main;
        EventGlobalManager.Instance.OnCollectKey.AddListener(CollectKey);
    }

    private void OnDisable()
    {
        if (EventGlobalManager.Instance) 
            EventGlobalManager.Instance.OnCollectKey.RemoveListener(CollectKey);
    }

    void CollectKey(Vector3 worldPos)
    {
        Gm.data.user.KeyCount++;
        
        for (var i = 0; i < keys.Count; i++)
        {
            keys[i].gameObject.SetActive(i < Gm.data.user.KeyCount);
            keys[i].anchoredPosition = Vector2.zero;

            if (i == Gm.data.user.KeyCount - 1)
            {
                RectTransform keyTrans = keys[i];
                keyTrans.position = _mainCamera.WorldToScreenPoint(worldPos);
                keyTrans.localScale = Vector3.zero;

                keyTrans.DOScale(1, .2f);
                keyTrans.DOAnchorPosY(100, .3f).SetRelative(true).OnComplete(() =>
                {
                    keyTrans.DOAnchorPos(Vector2.zero, .5f).SetDelay(.5f).OnComplete(() =>
                    {
                        canvasGroup.DOFade(0, .5f).SetDelay(1);
                    });
                });
            }
        }

        canvasGroup.DOFade(1, .5f);
    }
}
