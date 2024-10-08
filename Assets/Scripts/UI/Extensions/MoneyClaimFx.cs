using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HyperCatSdk;
using MoreMountains.NiceVibrations;
using Sigtrap.Relays;
using Sirenix.OdinInspector;
using TMPro;    
using UnityEngine;
using Math = System.Math;

public class MoneyClaimFx : HCMonoBehaviour
{
    [SerializeField] private Transform moneyIcon, spawnTrans, maxRdTrans, minRdTrans;
    [SerializeField] private GameObject moneyIconPrefab;

    private Vector3 _maxRdPos, _minRdPos;
    
    void CalculateSpawnBounds()
    {
        _maxRdPos = maxRdTrans.position;
        _minRdPos = minRdTrans.position;
    
    }

    [Button]
    public void ClaimMoney(int value, Transform spawn = null, int loops = 10, float interval = .05f)
    {
        Vector3 spawnOffset = Vector3.zero;
        if (spawn)
            spawnOffset = spawn.position - spawnTrans.position;
            
        StartCoroutine(ClaimMoneyCoroutine(value, spawnOffset, loops, interval));
    }
    
    public IEnumerator ClaimMoneyCoroutine(int value, Vector3 spawnOffset, int loops, float interval)
    {
        int val = Math.DivRem(value, loops, out int remain);
        
        for (int i = 0; i < loops; i++)
        {
            SpawnMoney(i == loops - 1 ? val + remain : val, spawnOffset);
            yield return new WaitForSeconds(interval);
        }
    }
    
    void SpawnMoney(int value, Vector3 spawnOffset)
    {
        CalculateSpawnBounds();
    
        Vector3 rdPos = spawnOffset + new Vector3(Random.Range(_minRdPos.x, _maxRdPos.x),
            Random.Range(_minRdPos.y, _maxRdPos.y),
            Random.Range(_minRdPos.z, _maxRdPos.z));

        Vector3 spawnPos = spawnTrans.position + spawnOffset;
        Transform spawnedMoney = Instantiate(moneyIconPrefab, spawnPos, Quaternion.identity, transform).transform;

        spawnedMoney.localScale = Vector3.zero;
        spawnedMoney.DOScale(Vector3.one, .3f).SetEase(Ease.OutExpo);
        spawnedMoney.DOMove(rdPos, .5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            spawnedMoney.DOMove(moneyIcon.position, .3f).SetEase(Ease.InExpo).SetDelay(.1f).OnComplete(() =>
            {
                HCVibrate.Haptic(HcHapticTypes.LightImpact);
                Gm.AddMoney(value);
                
                DOTween.Kill(moneyIcon);
                moneyIcon.DOScale(Vector3.one * 1.1f, .05f).OnComplete(() =>
                {
                    moneyIcon.DOScale(Vector3.one, .05f);
                });

                spawnedMoney.DOScale(Vector3.zero, .1f).OnComplete(() =>
                {
                    Destroy(spawnedMoney.gameObject);
                });
            });
        });
    }
}
