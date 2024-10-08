using Fusion;
using UnityEngine;

public class PooledNetworkObjectProvider : NetworkObjectProviderDefault
{
    private NetworkObjectPool _pooler = new NetworkObjectPool();

    public NetworkObjectPool Pooler
    {
        get => _pooler;
        set => _pooler = value;
    }

    protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
    {
        // Get object from the pool
        return _pooler.GetObject(prefab);
    }

    protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
    {
        // Load the prefab using the prefabId and set isSynchronous to true
        var prefab = runner.Prefabs.Load(prefabId, isSynchronous: true);

        // Return the instance to the pool
        _pooler.ReturnObject(prefab, instance);
    }
}