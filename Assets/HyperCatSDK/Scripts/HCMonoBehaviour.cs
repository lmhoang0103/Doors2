#region

using UnityEngine;

#endregion

public class HCMonoBehaviour : MonoBehaviour
{
    private ConfigManager _cfg;
    private GameManager _gameManager;
    private EventGlobalManager _eventGlobalManager;
    private Transform _privateTransform;

    [HideInInspector]
    public Transform Transform
    {
        get
        {
            if (_privateTransform == null)
                _privateTransform = transform;

            return _privateTransform;
        }
    }

    public GameManager Gm
    {
        get
        {
            if (_gameManager == null)
                _gameManager = GameManager.Instance;

            return _gameManager;
        }
    }

    public EventGlobalManager Evm
    {
        get
        {
            if (_eventGlobalManager == null)
                _eventGlobalManager = EventGlobalManager.Instance;

            return _eventGlobalManager;
        }
    }
    
    public ConfigManager Cfg
    {
        get
        {
            if (_cfg == null)
                _cfg = ConfigManager.Instance;

            return _cfg;
        }
    }

    public virtual void OnSpawned()
    {
    }

    public virtual void OnDespawned()
    {
    }
}