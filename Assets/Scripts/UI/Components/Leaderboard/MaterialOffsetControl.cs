using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
[DisallowMultipleComponent]
[ExecuteAlways]
public class MaterialOffsetControl : MonoBehaviour
{
    [SerializeField]
    private Vector2 offset;

    private void OnEnable()
    {
        
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        Graphic graphic = GetComponent<Graphic>();
        graphic.material.SetTextureOffset("_MainTex", offset);
        graphic.SetMaterialDirty();

        if (TryGetComponent(out Mask mask))
        {
            mask.enabled = false;
            mask.enabled = true;
        }
    }
#endif
}