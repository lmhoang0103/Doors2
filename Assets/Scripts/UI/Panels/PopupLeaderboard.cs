using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PopupLeaderboard : UIPanel
{
    [Title("Tier")]
    [SerializeField] private Image tierIcon;
    [SerializeField] private TMP_Text tierLabel;
    [SerializeField] private TMP_Text tierMinScoreLabel;
    [SerializeField] private TMP_Text tierMaxScoreLabel;
    [SerializeField] private List<GameObject> maxScores;

    [Space]
    [Title("Next Tier Reward")]
    [SerializeField] private GameObject nextTierReward;
    [SerializeField] private TierReward tierReward;
    
    [Space]
    [Title("Leaderboard Item")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform rankItemContainer;
    [SerializeField] private RankItem playerRank, rankItemPrefab;

    [Space] [Title("Buttons")] 
    [SerializeField] private HCButton continueButton;
    [SerializeField] private HCButton addRankButton;
    [SerializeField] private TMP_Text addRankLabel;
    [SerializeField] private HCButton refuseAddRankButton;
    
    private TierConfig _currentTierConfig;
    private TierConfig _nextTierConfig;
    private int _levelScore;
    private List<RankItem> _rankItems;
    
    const int MaximumRankDiffToOfferNextTier = 10;
    
    public static PopupLeaderboard Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupLeaderboard;
    }

    public static void Show(int lvScore)
    {
        var newInstance = (PopupLeaderboard) GUIManager.Instance.NewPanel(UiPanelType.PopupLeaderboard);
        Instance = newInstance;
        newInstance.OnAppear(lvScore);
    }

    public void OnAppear(int lvScore)
    {
        if (isInited)
            return;

        base.OnAppear();

        Init(lvScore);
    }

    private void Init(int lvScore)
    {
        continueButton.Hide(true);
        addRankButton.Hide(true);
        refuseAddRankButton.Hide(true);

        UpdateTier();
        UpdateTierReward();

        _rankItems = new List<RankItem>();
        _levelScore = lvScore;

        // TODO: Score offset
        int offlineTime = (int) (DateTime.Now - Gm.data.user.lastTimeLogOut).TotalSeconds;
        int scoreOffset = (offlineTime - Cfg.gameCfg.extraFeatureConfig.minDecreaseRankOfflineTime)
                          / Cfg.gameCfg.extraFeatureConfig.decreaseRankTimeInterval
                          * Cfg.gameCfg.extraFeatureConfig.decreaseRankScoreOffset;

        if (scoreOffset > 0)
            Gm.data.user.totalScoreOffset += scoreOffset;

        Gm.data.user.lastTimeLogOut = DateTime.Now;

        playerRank.InitByScore(Gm.data.user.score, Gm.data.user.totalScoreOffset, true);

        Utils.DestroyChildren(rankItemContainer);
        rankItemContainer.DetachChildren();

        int currentRank = playerRank.rank;
        int nextRank = playerRank.ScoreToRank(Gm.data.user.score + _levelScore);

        int rankOffset = currentRank - nextRank;

        const int maxListItem = 20;
        const int belowFillCount = 2;
        const int aboveFillCount = 2;

        for (int i = aboveFillCount; i >= 1; i--)
        {
            SpawnRankItem(nextRank - i);
        }

        if (rankOffset < maxListItem - 4)
        {
            for (int i = 0; i <= rankOffset; i++)
                SpawnRankItem(nextRank + i);
        }
        else
        {
            for (int i = 0; i >= -2; i--)
                SpawnRankItem(nextRank - i);

            for (int i = 0; i < maxListItem - 10; i++)
                SpawnRankItem(nextRank + (int)Mathf.Round((float)(rankOffset * i) / (maxListItem - 9)));

            for (int i = -2; i <= 0; i++)
                SpawnRankItem(currentRank + i);
        }

        for (int i = 1; i <= belowFillCount; i++)
            SpawnRankItem(currentRank + i);

        VerticalLayoutGroup layoutGroup = rankItemContainer.GetComponent<VerticalLayoutGroup>();
        layoutGroup.CalculateLayoutInputHorizontal();
        layoutGroup.CalculateLayoutInputVertical();
        layoutGroup.SetLayoutHorizontal();
        layoutGroup.SetLayoutVertical();

        RectTransform topSnap = _rankItems.Find(x => x.rank == currentRank)
                .GetComponent<RectTransform>();

        scrollRect.SnapTo(topSnap, 0);

        void SpawnRankItem(int initRank)
        {
            RankItem topRankItem = Instantiate(rankItemPrefab, rankItemContainer);
            topRankItem.InitByRank(initRank, Gm.data.user.totalScoreOffset);
            _rankItems.Add(topRankItem);
        }

        DOVirtual.DelayedCall(1, UpdateRank);
    }

    void UpdateRank()
    {
        Gm.data.user.score += _levelScore;
        playerRank.UpdateRank(Gm.data.user.score, 1, .3f);

        RectTransform topSnap = _rankItems.Find(x =>
                x.rank == playerRank.ScoreToRank(Gm.data.user.score))
            .GetComponent<RectTransform>();

        scrollRect.SnapTo(topSnap, 1.0f, .3f, () =>
        {
            if (UpdateTier())
            {
                tierIcon.transform.DOScale(1.05f, .15f).SetLoops(2, LoopType.Yoyo);
                tierLabel.transform.DOScale(1.05f, .15f).SetLoops(2, LoopType.Yoyo);
                
                tierReward.ClaimReward();
            }
            else if (_nextTierConfig && _nextTierConfig.scoreRequire - Gm.data.user.score <= MaximumRankDiffToOfferNextTier)
            {
                addRankButton.Show();
                addRankLabel.text = $"+{_nextTierConfig.scoreRequire - Gm.data.user.score} scores";
                
                DOVirtual.DelayedCall(2, () => refuseAddRankButton.Show()).SetTarget(this);
            }
            else
                continueButton.Show();
        });
    }
    
    bool UpdateTier()
    {
        TierConfig lastTierConfig = _currentTierConfig;

        bool isMaxTier = GetTierConfig();

        maxScores.SetActive(!isMaxTier);
        tierMinScoreLabel.text = (isMaxTier ? ">" : "") + _currentTierConfig.scoreRequire.ToFormatString();
        if (!isMaxTier)
            tierMaxScoreLabel.text = _nextTierConfig.scoreRequire.ToFormatString();

        tierIcon.sprite = _currentTierConfig.icon;
        tierLabel.text = $"{_currentTierConfig.tierName} Tier";

        return _currentTierConfig != lastTierConfig;
    }
    
    void UpdateTierReward()
    {
        bool isMaxTier = GetTierConfig();

        if (!isMaxTier)
        {
            nextTierReward.SetActive(true);

            if (_nextTierConfig.rewardType == RewardType.Coin)
                tierReward.InitCoin(_nextTierConfig.coinRewardValue);
            else if (_nextTierConfig.rewardType == RewardType.Gift)
                tierReward.InitGift();
        }
        else
        {
            nextTierReward.SetActive(false);
        }
    }

    private bool GetTierConfig()
    {
        var sortedTierConfigs = Cfg.gameCfg.extraFeatureConfig.tierConfigs.OrderByDescending(x => x.scoreRequire).ToList();

        _currentTierConfig = sortedTierConfigs[0];
        int currentTierIndex = 0;

        for (int i = 0; i < sortedTierConfigs.Count; i++)
        {
            TierConfig rankConfig = sortedTierConfigs[i];
            if (Gm.data.user.score >= rankConfig.scoreRequire)
            {
                _currentTierConfig = rankConfig;
                currentTierIndex = i;
                break;
            }
        }

        bool isMaxTier = currentTierIndex == 0;

        _nextTierConfig = isMaxTier ? null : sortedTierConfigs[currentTierIndex - 1];

        return isMaxTier;
    }

    public void AddRank()
    {
        int rankToNextTier = _nextTierConfig.scoreRequire - Gm.data.user.score;
        if (rankToNextTier <= 0 || rankToNextTier > MaximumRankDiffToOfferNextTier)
            return;

        AdManager.Instance.ShowRewardedAds("UpRankToNextTier", () =>
        {
            Init(rankToNextTier);
        });
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}