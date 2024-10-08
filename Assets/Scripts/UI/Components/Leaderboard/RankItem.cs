using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class RankItem : HCMonoBehaviour
{
    [SerializeField] private TMP_Text rankText, nameText, scoreText;
    [ReadOnly] public int rank, score;

    private AnimationCurve _rankToScoreCurve;
    private AnimationCurve _scoreToRankCurve;
    
    private float _noise = 1.054321f;
    
    public void InitByRank(int initRank, int scoreOffset = 0, bool isPlayer = false)
    {
        if (initRank < 1)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            return;
        }
        
        UpdateCurves(scoreOffset);
        
        rank = initRank;
        score = RankToScore(rank);

        Init(isPlayer);
    }

    public void InitByScore(int initScore, int scoreOffset = 0, bool isPlayer = false)
    {
        UpdateCurves(scoreOffset);
        
        score = initScore;
        rank = ScoreToRank(score);
        
        Init(isPlayer);
    }

    private void Init(bool isPlayer)
    {
        rankText.text = "#" + rank.ToFormatString();
        nameText.text = isPlayer
            ? Gm.data.user.name
            : Utils.GetNameByRank(rank);
        scoreText.text = score.ToFormatString();
    }

    public void UpdateRank(int updatedScore, float duration, float scaleDuration, Action onComplete = null)
    {
        int updatedRank = ScoreToRank(updatedScore);

        transform.DOScale(Vector3.one * 1.1f, scaleDuration);
        DOTween.To(() => rank, x => rankText.text = "#" + x.ToFormatString(), updatedRank, duration)
            .SetDelay(scaleDuration);
        DOTween.To(() => score, x => scoreText.text = x.ToFormatString(), updatedScore, duration)
            .SetDelay(scaleDuration)
            .OnComplete(() =>
            {
                transform.DOScale(Vector3.one, scaleDuration).OnComplete(() => onComplete?.Invoke());
            });
        
    }
    
    public int ScoreToRank(int s)
    {
        int result = (int) Mathf.Floor(_scoreToRankCurve.Evaluate(s * _noise));
        return result > 1 ? result : 1;
    }

    public int RankToScore(int r)
    {
        int result = (int) Mathf.Round(_rankToScoreCurve.Evaluate(r + .9f) / _noise);
        return result > 0 ? result : 0;
    }

    // Fake giảm rank
    void UpdateCurves(int scoreOffset)
    {
        _rankToScoreCurve = Cfg.gameCfg.extraFeatureConfig.rankToScoreCurve;

        var keys = _rankToScoreCurve.keys;

        _rankToScoreCurve = new AnimationCurve();
        
        keys.ForEach(keyframe =>
        {
            if (keyframe.value > 0)
                keyframe.value += scoreOffset;
            else
                keyframe.time = Cfg.gameCfg.extraFeatureConfig.GetMaxRankAtZeroScore(scoreOffset);

            _rankToScoreCurve.AddKey(keyframe);
        });
        
        InverseCurve();
    }
    
    void InverseCurve()
    {
        _rankToScoreCurve.SetCurveLinear();
        
        _scoreToRankCurve = new AnimationCurve();
        
        for (int i = 0; i < _rankToScoreCurve.length; i++)
        {
            Keyframe inverseKey = new Keyframe(_rankToScoreCurve.keys[i].value, _rankToScoreCurve.keys[i].time);
            
            _scoreToRankCurve.AddKey(inverseKey);
            _scoreToRankCurve.keys.ForEach(keyframe => keyframe.weightedMode = WeightedMode.None);
        }

        _scoreToRankCurve.SetCurveLinear();
    }
}
