#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

public class AutoDestroy : HCMonoBehaviour
{
    private float _timeStart;

    [ShowIf("typeDestroy", TypeDestroy.Despawn)]
    public Pool pool;

    [SerializeField]
    public float timeOut = 0.5f;

    public TypeDestroy typeDestroy = TypeDestroy.Disable;

    private void OnEnable()
    {
        _timeStart = Time.time;
    }

    public override void OnSpawned()
    {
        _timeStart = Time.time;
    }

    private void Update()
    {
        if (Time.time - _timeStart > timeOut)
            Action();
    }

    public void Action()
    {
        if (typeDestroy == TypeDestroy.Disable)
            gameObject.SetActive(false);
        else if (typeDestroy == TypeDestroy.Despawn)
            gameObject.Despawn(pool);
        else if (typeDestroy == TypeDestroy.Destroy)
            Destroy(gameObject);
    }
}

public enum TypeDestroy
{
    Disable,
    Despawn,
    Destroy
}