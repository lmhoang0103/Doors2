#region

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#endregion

public class PanelAnim : HCMonoBehaviour
{
    public enum AnimInType
    {
        None,
        FromTop,
        FromLeft,
        FromRight,
        FromBot,
        FadeAlpha,
        FromScale
    }

    public enum AnimOutType
    {
        None,
        ToTop,
        ToLeft,
        ToRight,
        ToBot,
        FadeAlpha,
        ToScale
    }
    
    private bool UsePopupAnim => animIn != AnimInType.None && animIn != AnimInType.FadeAlpha ||
                                 animOut != AnimOutType.None && animOut != AnimOutType.FadeAlpha;
    
    #region Anim In Fields
    
    [Header("Anim In")]
    
    public AnimInType animIn = AnimInType.None;

    [HideIf(nameof(animIn), AnimInType.None)]
    public UnityEvent animInCompletedEvent;

    [HideIf(nameof(animIn), AnimInType.None)]
    public float animInTime = .35f;

    [HideIf(nameof(animIn), AnimInType.None)]
    public float delayAnimIn;
    
    [ShowIf(nameof(animIn), AnimInType.FromScale)]
    public float initScale = 0;

    [ShowIf(nameof(animIn), AnimInType.FromScale)]
    public bool separateAxisAnimIn;
    
    [ShowIf(nameof(ShowAnimInEasingType))]
    public EasingType animInEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseAnimInCurve))]
    public AnimationCurve animInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [ShowIf(nameof(ShowSeparateAnimInEasingType))]
    public EasingType animInXAxisEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseXAxisAnimInCurve))]
    public AnimationCurve animInXAxisCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [ShowIf(nameof(ShowSeparateAnimInEasingType))]
    public EasingType animInYAxisEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseYAxisAnimInCurve))]
    public AnimationCurve animInYAxisCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    #region Anim In Properties
    
    private bool ShowAnimInEasingType
    {
        get
        {
            if (animIn == AnimInType.FromScale && separateAxisAnimIn)
                return false;

            return animIn != AnimInType.None;
        }
    }
    
    private bool ShowSeparateAnimInEasingType => animIn == AnimInType.FromScale && separateAxisAnimIn;

    private bool UseAnimInCurve
    {
        get
        {
            if (ShowSeparateAnimInEasingType)
                return false;
            
            return animIn != AnimInType.None && animInEasingType == EasingType.Custom;
        }
    }
    
    private bool UseXAxisAnimInCurve => ShowSeparateAnimInEasingType && animInXAxisEasingType == EasingType.Custom;
    
    private bool UseYAxisAnimInCurve => ShowSeparateAnimInEasingType && animInYAxisEasingType == EasingType.Custom;
    
    #endregion
    
    #endregion
    
    #region Anim Out Fields
    
    [Space]
    [Header("Anim Out")]
    public AnimOutType animOut = AnimOutType.None;

    [HideIf(nameof(animOut), AnimOutType.None)]
    public UnityEvent animOutCompletedEvent;

    [HideIf(nameof(animOut), AnimOutType.None)]
    public float animOutTime = .35f;
    
    [ShowIf(nameof(animOut), AnimOutType.ToScale)]
    public float targetScale = 0;

    [ShowIf(nameof(animOut), AnimOutType.ToScale)]
    public bool separateAxisAnimOut;
    
    [ShowIf(nameof(ShowAnimOutEasingType))]
    public EasingType animOutEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseAnimOutCurve))]
    public AnimationCurve animOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [ShowIf(nameof(ShowSeparateAnimOutEasingType))]
    public EasingType animOutXAxisEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseXAxisAnimOutCurve))]
    public AnimationCurve animOutXAxisCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [ShowIf(nameof(ShowSeparateAnimOutEasingType))]
    public EasingType animOutYAxisEasingType = EasingType.OutQuad;
    
    [ShowIf(nameof(UseYAxisAnimOutCurve))]
    public AnimationCurve animOutYAxisCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    #region Anim Out Properties
    
    private bool ShowAnimOutEasingType
    {
        get
        {
            if (animOut == AnimOutType.ToScale && separateAxisAnimOut)
                return false;

            return animOut != AnimOutType.None;
        }
    }
    
    private bool ShowSeparateAnimOutEasingType => animOut == AnimOutType.ToScale && separateAxisAnimOut;

    private bool UseAnimOutCurve
    {
        get
        {
            if (ShowSeparateAnimOutEasingType)
                return false;
            
            return animOut != AnimOutType.None && animOutEasingType == EasingType.Custom;
        }
    }
    
    private bool UseXAxisAnimOutCurve => ShowSeparateAnimOutEasingType && animOutXAxisEasingType == EasingType.Custom;
    
    private bool UseYAxisAnimOutCurve => ShowSeparateAnimOutEasingType && animOutYAxisEasingType == EasingType.Custom;
    
    #endregion
    
    #endregion
    
    [Space]
    [Header("Others")]

    public bool unscaleTime;
    [ShowIf("UsePopupAnim")] public RectTransform popup;
    public CanvasGroup rootGraphic;
    private UIPanel _target;
    private Vector3 _initLocalPosition;

    public void Setup(UIPanel target)
    {
        _target = target;
        _initLocalPosition = UsePopupAnim && popup ? popup.localPosition : transform.localPosition;
    }

    public void StartAnimIn()
    {
        if (rootGraphic)
        {
            rootGraphic.alpha = 1;
            rootGraphic.DOKill();
        }

        if (popup)
        {
            popup.DOKill();
            popup.localPosition = _initLocalPosition;
            popup.localScale = Vector3.one;
        }
        
        _target.root.transform.localScale = Vector3.one;
        _target.root.transform.DOKill();

        _target.root.DOComplete();
        
        if (animIn == AnimInType.None)
            return;

        Ease dotweenEase = (Ease) animInEasingType;
         
        switch (animIn)
        {
            case AnimInType.FromTop:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    rootGraphic.DOFade(1f, animInTime * 0.75f).SetUpdate(unscaleTime).SetDelay(delayAnimIn);
                }
                
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.height / 2 + (rootRect.center.y - popupRect.center.y) + popupRect.height / 2;
    
                    popup.localPosition += Vector3.up * offset;
    
                    var tween = popup.DOLocalMoveY(-offset, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .SetUpdate(unscaleTime).OnComplete(OnAnimInComplete);

                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    _target.root.transform.localPosition = Vector3.up * Screen.height;
                    var tween = _target.root.transform.DOLocalMoveY(0f, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .SetEase(Ease.InOutBack)
                        .OnComplete(OnAnimInComplete);
                    
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                
                break;
            case AnimInType.FromBot:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    rootGraphic.DOFade(1f, animInTime * 0.75f).SetDelay(delayAnimIn).SetUpdate(unscaleTime);
                }
                
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.height / 2 - (rootRect.center.y - popupRect.center.y) + popupRect.height / 2;
    
                    popup.localPosition += Vector3.down * offset;
    
                    var tween = popup.DOLocalMoveY(offset, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .SetUpdate(unscaleTime).OnComplete(OnAnimInComplete);

                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    _target.root.transform.localPosition = Vector3.down * Screen.height;
                    var tween = _target.root.transform.DOLocalMoveY(0f, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .OnComplete(OnAnimInComplete);
                    
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                
                break;
            case AnimInType.FromLeft:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    rootGraphic.DOFade(1f, animInTime * 0.75f).SetDelay(delayAnimIn).SetUpdate(unscaleTime);
                }
                
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.width / 2 - (rootRect.center.x - popupRect.center.x) + popupRect.width / 2;
    
                    popup.localPosition += Vector3.left * offset;
    
                    var tween = popup.DOLocalMoveX(offset, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .SetUpdate(unscaleTime).OnComplete(OnAnimInComplete);
                    
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                   _target.root.transform.localPosition = Vector3.left * Screen.width;
                   var tween = _target.root.transform.DOLocalMoveX(0f, animInTime)
                       .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                       .OnComplete(OnAnimInComplete);
                   
                   if (UseAnimInCurve)
                       tween.SetEase(animInCurve);
                   else
                       tween.SetEase(dotweenEase);
                }
                
                break;
            case AnimInType.FromRight:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    rootGraphic.DOFade(1f, animInTime * 0.75f).SetDelay(delayAnimIn).SetUpdate(unscaleTime);
                }

                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.width / 2 + (rootRect.center.x - popupRect.center.x) + popupRect.width / 2;
    
                    popup.localPosition += Vector3.right * offset;
    
                    var tween = popup.DOLocalMoveX(-offset, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .SetUpdate(unscaleTime).OnComplete(OnAnimInComplete);
                    
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    _target.root.transform.localPosition = Vector3.right * Screen.width;
                    var tween = _target.root.transform.DOLocalMoveX(0f, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .OnComplete(OnAnimInComplete);

                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }

                break;
            case AnimInType.FadeAlpha:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    var tween = rootGraphic.DOFade(1f, animInTime)
                        .SetDelay(delayAnimIn).SetUpdate(unscaleTime)
                        .OnComplete(OnAnimInComplete);
                    
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }

                break;
            case AnimInType.FromScale:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 0f;
                    rootGraphic.DOFade(1f, animInTime * 0.75f).SetDelay(delayAnimIn).SetUpdate(unscaleTime);
                }

                Transform tweenTarget = UsePopupAnim && popup ? popup : _target.root.transform;
                
                var initLocalScale = Vector3.one * initScale;
                initLocalScale.z = 1;
                tweenTarget.localScale = initLocalScale;
                if (separateAxisAnimIn)
                {
                    Ease dotweenXEase = (Ease) animInXAxisEasingType;
                    Ease dotweenYEase = (Ease) animInYAxisEasingType;
                    
                    var xTween = tweenTarget.DOScaleX(1, animInTime)
                        .SetUpdate(unscaleTime).SetDelay(delayAnimIn)
                        .OnComplete(OnAnimInComplete);
                    
                    if (UseXAxisAnimInCurve)
                        xTween.SetEase(animInXAxisCurve);
                    else
                        xTween.SetEase(dotweenXEase);

                    var yTween = tweenTarget.DOScaleY(1, animInTime)
                        .SetUpdate(unscaleTime).SetDelay(delayAnimIn);

                    if (UseYAxisAnimInCurve)
                        yTween.SetEase(animInYAxisCurve);
                    else
                        yTween.SetEase(dotweenYEase);
                }
                else
                {
                    var tween = tweenTarget.DOScale(1, animInTime)
                        .SetUpdate(unscaleTime).SetDelay(delayAnimIn)
                        .OnComplete(OnAnimInComplete);
                
                    if (UseAnimInCurve)
                        tween.SetEase(animInCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                
                break;
        }
    }

    public virtual void OnAnimInComplete()
    {
        if (animInCompletedEvent != null)
            animInCompletedEvent.Invoke();
    }

    public void StartAnimOut()
    {
        if (popup)
        {
            popup.DOKill();
            popup.localPosition = _initLocalPosition;
            popup.localScale = Vector3.one;
        }
        
        _target.root.DOComplete();
        
        if (animOut == AnimOutType.None)
            return;

        Ease dotweenEase = (Ease) animOutEasingType;
        
        switch (animOut)
        {
            case AnimOutType.ToTop:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime * 0.75f).SetUpdate(unscaleTime);
                }
                
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.height / 2 + (rootRect.center.y - popupRect.center.y) + popupRect.height / 2;
                    
                    var tween = popup.DOLocalMoveY(offset, animOutTime)
                        .SetUpdate(unscaleTime).SetRelative(true)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    var tween = _target.root.transform.DOLocalMoveY(Screen.height / 4f, animOutTime)
                        .SetUpdate(unscaleTime).OnComplete(OnAnimOutComplete);

                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                
                break;
            case AnimOutType.ToBot:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime * 0.75f).SetUpdate(unscaleTime);
                }
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.height / 2 - (rootRect.center.y - popupRect.center.y) + popupRect.height / 2;
                    
                    var tween = popup.DOLocalMoveY(-offset, animOutTime)
                        .SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    var tween = _target.root.transform.DOLocalMoveY(-Screen.height / 4f, animOutTime)
                        .SetUpdate(unscaleTime)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }

                break;
            case AnimOutType.ToLeft:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime * 0.75f).SetUpdate(unscaleTime);
                }
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.width / 2 - (rootRect.center.x - popupRect.center.x) + popupRect.width / 2;
                    
                    var tween = popup.DOLocalMoveX(-offset, animOutTime)
                        .SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    var tween = _target.root.transform.DOLocalMoveX(-Screen.width / 4f, animOutTime)
                        .SetUpdate(unscaleTime)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }

                break;
            case AnimOutType.ToRight:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime * 0.75f).SetUpdate(unscaleTime);
                }
                if (UsePopupAnim && popup)
                {
                    Rect rootRect = _target.root.pixelRect;
                    Rect popupRect = popup.rect;
                    
                    float offset = rootRect.width / 2 + (rootRect.center.x - popupRect.center.x) + popupRect.width / 2;
                    
                    var tween = popup.DOLocalMoveX(offset, animOutTime)
                        .SetUpdate(unscaleTime)
                        .SetRelative(true)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                else
                {
                    var tween = _target.root.transform.DOLocalMoveX(Screen.width / 4f, animOutTime)
                        .SetUpdate(unscaleTime)
                        .OnComplete(OnAnimOutComplete);
                        
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);;
                }

                break;
            case AnimOutType.FadeAlpha:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime).SetUpdate(unscaleTime).OnComplete(() => { OnAnimOutComplete(); });
                }

                break;
            case AnimOutType.ToScale:
                if (rootGraphic)
                {
                    rootGraphic.alpha = 1f;
                    rootGraphic.DOFade(0f, animOutTime * 0.75f).SetUpdate(unscaleTime);
                }
                
                Transform tweenTarget = UsePopupAnim && popup ? popup : _target.root.transform;
                
                tweenTarget.localScale = Vector3.one;

                if (separateAxisAnimOut)
                {
                    Ease dotweenXEase = (Ease) animOutXAxisEasingType;
                    Ease dotweenYEase = (Ease) animOutYAxisEasingType;
                    
                    var xTween = tweenTarget.DOScaleX(targetScale, animOutTime)
                        .SetUpdate(unscaleTime)
                        .OnComplete(OnAnimOutComplete);
                    
                    if (UseXAxisAnimOutCurve)
                        xTween.SetEase(animOutXAxisCurve);
                    else
                        xTween.SetEase(dotweenXEase);

                    var yTween = tweenTarget.DOScaleY(targetScale, animOutTime)
                        .SetUpdate(unscaleTime);

                    if (UseYAxisAnimOutCurve)
                        yTween.SetEase(animOutYAxisCurve);
                    else
                        yTween.SetEase(dotweenYEase);
                }
                else
                {
                    var tween = tweenTarget.DOScale(targetScale, animOutTime)
                        .SetUpdate(unscaleTime)
                        .OnComplete(OnAnimOutComplete);
                    
                    
                    if (UseAnimOutCurve)
                        tween.SetEase(animOutCurve);
                    else
                        tween.SetEase(dotweenEase);
                }
                
                break;
        }
    }

    public virtual void OnAnimOutComplete()
    {
        if (animOutCompletedEvent != null)
            animOutCompletedEvent.Invoke();
        CancelInvoke();
        _target.Hide();
    }
}