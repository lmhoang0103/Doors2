#region

using PathologicalGames;
using UnityEngine;

#endregion

public static class PoolHelper
{
    public static Transform Spawn(this GameObject obj, Pool pool)
    {
        var poolName = pool.ToString();
        if (pool == Pool.None)
            poolName = obj.name;
        return PoolManager.Pools[poolName].Spawn(obj, Vector3.zero, Quaternion.identity);
    }

    public static Transform Spawn(this GameObject obj, Transform parent, Pool pool)
    {
        var poolName = pool.ToString();
        if (pool == Pool.None)
            poolName = obj.name;
        return PoolManager.Pools[poolName].Spawn(obj, Vector3.zero, Quaternion.identity, parent);
    }

    public static Transform Spawn(this GameObject prefab, Vector3 pos, Quaternion rot, Pool pool)
    {
        var poolName = pool.ToString();
        if (pool == Pool.None)
            poolName = prefab.name;
        return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
    }

    public static Transform Spawn(this GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, Pool pool)
    {
        var poolName = pool.ToString();
        if (pool == Pool.None)
            poolName = prefab.name;
        return PoolManager.Pools[poolName].Spawn(prefab, pos, rot, parent);
    }

    public static void Despawn(this GameObject prefab, Pool pool)
    {
        var poolName = pool.ToString();
        if (pool == Pool.None)
        {
            poolName = prefab.name;
            if (poolName.IndexOf('(') > 0)
                poolName = prefab.name.Substring(0, poolName.IndexOf('('));
        }

        PoolManager.Pools[poolName].Despawn(prefab.transform);
    }
}