using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class FXSpawner : NetworkBehaviour
{
    [SerializeField] private TrailRenderer _bulletTrailFX;
    [SerializeField] private ParticleSystem _hitFX;
    [SerializeField] private ParticleSystem _deathFX;

    private void Awake()
    {
        PlayerHealth.OnDeathEvent += RPC_SpawnDeathFX;
        PlayerWeapon.OnHitSomethingEvent += RPC_SpawnHitFX;
        PlayerWeapon.OnFireEvent += RPC_SpawnBulletTrail;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SpawnHitFX(Vector3 pos)
    {
        if (_hitFX == null) return;

        Instantiate(_hitFX, pos, Quaternion.identity);

    }
        
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SpawnDeathFX(Vector3 pos)
    {
        if (_deathFX == null) return;

        var fxInstance = Instantiate(_deathFX, pos, Quaternion.identity);
        fxInstance.transform.SetParent(null); 
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SpawnBulletTrail(string startEnd)
    {
        Vector3Array vectorArray = JsonUtility.FromJson<Vector3Array>(startEnd);
    
        if (vectorArray == null || vectorArray.vectors.Length < 2) return;
        
        if (_bulletTrailFX == null) return;
        var fxInstance = Instantiate(_bulletTrailFX, vectorArray.vectors[0], Quaternion.identity);
        fxInstance.transform.SetParent(null);
        fxInstance.transform.DOMove(vectorArray.vectors[1], 0.05f);
    }
    
    
    
}

