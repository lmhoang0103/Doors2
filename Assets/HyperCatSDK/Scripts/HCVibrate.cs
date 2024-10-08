#region

using MoreMountains.NiceVibrations;

#endregion

namespace HyperCatSdk
{
    public static class HCVibrate
    {
        public static void Haptic(HcHapticTypes type)
        {
            if (!GameManager.Instance.data.setting.haptic)
                return;

            MMVibrationManager.Haptic((HapticTypes) type, false, true);
        }
    }
}

public enum HcHapticTypes
{
    Selection,
    Success,
    Warning,
    Failure,
    LightImpact,
    MediumImpact,
    HeavyImpact,
    RigidImpact,
    SoftImpact,
    None
}