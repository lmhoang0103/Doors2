#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/Game")]
public class GameConfig : ScriptableObject
{
    [TabGroup("Extra Feature")]
    public ExtraFeatureConfig extraFeatureConfig;

    [TabGroup("Other")]
    public int maxOfflineRemindMinute = 720;
}