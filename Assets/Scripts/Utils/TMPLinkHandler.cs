
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text _text;
    private Canvas _canvas;
    private Camera _camera;
    
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _canvas = GetComponentInParent<Canvas>();

        _camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePos = new Vector3(eventData.position.x, eventData.position.y, 0);

        var linkTextIndex = TMP_TextUtilities.FindIntersectingLink(_text, mousePos, _camera);
        
        if (linkTextIndex == -1)
            return;

        TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkTextIndex];

        string linkID = linkInfo.GetLinkID();

        if (linkID.Contains("www"))
            Application.OpenURL(linkID);
    }
}