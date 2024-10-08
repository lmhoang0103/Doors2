public partial class IronSourceManager
{
    private void RegisterRevenuePaidCallback()
    {
        IronSourceEvents.onImpressionDataReadyEvent += OnImpressionDataReady;
    }

    private void OnImpressionDataReady(IronSourceImpressionData impressionData)
    {
        if (impressionData != null && !string.IsNullOrEmpty(impressionData.adNetwork))
            AnalyticsRevenueAds.SendEvent(impressionData);
    }
}