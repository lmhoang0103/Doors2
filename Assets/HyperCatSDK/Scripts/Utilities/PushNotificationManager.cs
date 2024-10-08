#region

using System;
using System.Collections;
#if UNITY_ANDROID
using Unity.Notifications.Android;

#endif
#if UNITY_IOS
using Unity.Notifications.iOS;

#endif

#endregion

public class PushNotificationManager : Singleton<PushNotificationManager>
{
    public void StartRequest()
    {
        GameManager.Instance.data.setting.requestedPn = true;
        Database.SaveData();

        RequestAuthorization();
    }

    #region Schedule A Notification

    public void ScheduleNotification(PushNotificationType type, string content, int timer)
    {
        if (!GameManager.Instance.data.setting.enablePn)
            return;

        HCDebug.Log("PSN >Set Notification: " + content + " - After " + timer + " secs", HcColor.Orange);

        var triggerTime = DateTime.Now.AddSeconds(timer);

#if UNITY_IOS
        var iOsTrigger = new iOSNotificationCalendarTrigger
        {
            Hour = triggerTime.Hour, Minute = triggerTime.Minute, Second = triggerTime.Second, Repeats = false
        };

        var iOsNotification = new iOSNotification
        {
            Identifier = type.ToString(),
            Body = content,
            Subtitle = "",
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            CategoryIdentifier = "TaskNotification",
            ThreadIdentifier = "DefaultThread",
            Trigger = iOsTrigger
        };

        iOSNotificationCenter.ScheduleNotification(iOsNotification);
#endif

#if UNITY_ANDROID
        var androidNotification = new AndroidNotification();
        androidNotification.Text = content;
        androidNotification.FireTime = triggerTime;
        androidNotification.SmallIcon = "icon_0";
        androidNotification.LargeIcon = "icon_1";

        var newIndex = AndroidNotificationCenter.SendNotification(androidNotification, "GameNotification");
        if (!GameManager.Instance.data.setting.androidPnIndexes.ContainsKey(type))
        {
            GameManager.Instance.data.setting.androidPnIndexes.Add(type, newIndex);
            Database.SaveData();
        }
#endif
    }

    #endregion

    public void CancelNotification(PushNotificationType noti)
    {
#if UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(noti.ToString());
#endif
#if UNITY_ANDROID
        if (!GameManager.Instance.data.setting.androidPnIndexes.ContainsKey(noti))
            return;

        var index = GameManager.Instance.data.setting.androidPnIndexes[noti];
        AndroidNotificationCenter.CancelNotification(index);
        GameManager.Instance.data.setting.androidPnIndexes.Remove(noti);
#endif
    }

    #region Register

    private void RequestAuthorization()
    {
#if UNITY_IOS
        StartCoroutine(RequestAuthorizationForiOS());
#endif

#if UNITY_ANDROID
        RequestNotificationChannelForAndroid();
#endif

        HCDebug.Log("PSN >Request for Push Notification", HcColor.Orange);
    }

#if UNITY_IOS
    private IEnumerator RequestAuthorizationForiOS()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
                yield return null;

            var res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            HcDebug.Log(res);

            if (req.Granted)
            {
                GameManager.Instance.data.setting.enablePn = true;
                Database.SaveData();

                GameManager.Instance.SetupRemindOfflinePushNotification();
            }
        }
    }
#endif

#if UNITY_ANDROID
    private void RequestNotificationChannelForAndroid()
    {
        var c = new AndroidNotificationChannel
        {
            Id = "GameNotification", Name = "HyperCat Channel", Importance = Importance.High, Description = "Generic notifications"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
        GameManager.Instance.data.setting.enablePn = true;
        Database.SaveData();

        GameManager.Instance.SetupRemindOfflinePushNotification();
    }
#endif

    #endregion
}

public enum PushNotificationType
{
    None,
    RemindComeback,
    Total
}