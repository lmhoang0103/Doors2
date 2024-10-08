
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class LuckyWheelProgressReward : HCMonoBehaviour
{
    [SerializeField] private Transform rewardContainer;
    [SerializeField] private GameObject claimed;
    [SerializeField] private UnityEvent onClaim;
    
    public void Claim()
    {
        SetClaimed(true);
        rewardContainer.DOScale(Vector3.one * 1.1f, .1f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => onClaim?.Invoke());
    }
    
    public void SetClaimed(bool isClaimed)
    {
        claimed.SetActive(isClaimed);
    }
}