#region

using UnityEngine;

#endregion

public class SingletonDestroy<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                var gameObject = new GameObject();
                _instance = gameObject.AddComponent<T>();
            }

            return _instance;
        }

        set => _instance = value;
    }

    protected virtual void Awake()
    {
        _instance = GetComponent<T>();
    }
}