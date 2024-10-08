#region

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class LoadingManager : Singleton<LoadingManager>
{
    private readonly Dictionary<SceneIndex, Action> _sceneLoadedSpecialAction = new Dictionary<SceneIndex, Action>();

    private void SetSceneAction(SceneIndex scene, Action action)
    {
        if (_sceneLoadedSpecialAction.ContainsKey(scene))
            _sceneLoadedSpecialAction[scene] = action;
        else
            _sceneLoadedSpecialAction.Add(scene, action);
    }

    public void LoadScene(SceneIndex type, Action action = null, bool overrideNullAction = false)
    {
        if (overrideNullAction || action != null)
            SetSceneAction(type, action);

        EventGlobalManager.Instance.OnStartLoadScene.Dispatch();
        StartCoroutine(LoadSceneAsync(type));
    }

    private IEnumerator LoadSceneAsync(SceneIndex index)
    {
        yield return Yielders.Get(0.1f);
        var asyncLoad = SceneManager.LoadSceneAsync((int) index);
        asyncLoad.allowSceneActivation = false;
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        var waitingForAoa = 0f;
        while (!asyncLoad.isDone || (!AppOpenAdManager.showedFirstOpenAd && GameManager.EnableAds))
        {
            GUIManager.Instance.loadingUi.Progress(asyncLoad.progress, .15f);

            if (!AppOpenAdManager.showedFirstOpenAd && GameManager.EnableAds)
            {
                waitingForAoa += Time.deltaTime;
            }
            else
            {
                waitingForAoa = 999f;
            }

            if (asyncLoad.progress >= 0.90f && waitingForAoa > 6f)
            {
                asyncLoad.allowSceneActivation = true;
                
                if (asyncLoad.isDone)
                    break;
            }

            yield return null;
        }

        GC.Collect(2, GCCollectionMode.Forced);
        Resources.UnloadUnusedAssets();

        GUIManager.Instance.loadingUi.Progress(1, 2);
        yield return new WaitForSeconds(.5f);

        EventGlobalManager.Instance.OnFinishLoadScene.Dispatch();
        if (_sceneLoadedSpecialAction.ContainsKey(index) && _sceneLoadedSpecialAction[index] != null)
            _sceneLoadedSpecialAction[index].Invoke();
    }
}