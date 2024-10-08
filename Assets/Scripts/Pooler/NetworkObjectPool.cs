using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class NetworkObjectPool
{
    private Dictionary<NetworkObject, Queue<NetworkObject>> _pool = new Dictionary<NetworkObject, Queue<NetworkObject>>();

    public NetworkObject GetObject(NetworkObject prefab)
    {
        if (_pool.TryGetValue(prefab, out Queue<NetworkObject> objects) && objects.Count > 0)
        {
            NetworkObject obj = objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            return GameObject.Instantiate(prefab);
        }
    }

    public void ReturnObject(NetworkObject prefab, NetworkObject instance)
    {
        instance.gameObject.SetActive(false);  
        if (!_pool.ContainsKey(prefab))
        {
            _pool[prefab] = new Queue<NetworkObject>();
        }
        _pool[prefab].Enqueue(instance);  
    }
}