using System;
using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public static event Action<float,float> OnLocalPlayerChangeHealthEvent; 
    public static event Action<int,int> OnDeathUpdateKDEvent; 
    public static event Action OnLocalPlayerHurtEvent; 
    public static event Action<Vector3> OnDeathEvent;

    private const float MaxHealth = 100f;
    public event Action<float> OnDamageEvent;

    [Networked] private int _shooterID { get; set; }

    public GameObject healthCrate;
    public PlayerTeam playerTeam;
    public Transform deathCamTransform;

    [Header("Things to toggle on death")] 
    public Collider bodyCollider;
    public CharacterController characterController;
    public SkinnedMeshRenderer bodyMeshRen;
    public MeshRenderer gunMeshRen;
    [Networked] public bool IsDead { get; set; }
    [Networked, OnChangedRender(nameof(HealthChanged))]
    private float NetworkedHealth { get; set; }

    public override void Spawned()
    {
        NetworkedHealth = MaxHealth;
        IsDead = false;
    }

    public void Respawn()
    {
        RPC_ResetPlayer();
        
        if (HasStateAuthority)
        {
            IsDead = false;
            bodyCollider.enabled = true;
            characterController.enabled = true;
            bodyMeshRen.enabled = true;
                
            if (Camera.main != null) Camera.main.GetComponent<FirstPersonCamera>().isDead = false;
            GetComponent<PlayerWeapon>().CanShoot = true;
            GetComponent<PlayerWeapon>().SetWeapon(null);
        }
    }

    [Rpc]
    public void RPC_ResetPlayer()
    {
        NetworkedHealth = MaxHealth;
        bodyCollider.enabled = true;
        characterController.enabled = true;
        bodyMeshRen.enabled = true;
        //gunMeshRen.enabled = true;
    }


    void HealthChanged()
    {
       
        Debug.Log($"Health changed to: {NetworkedHealth}");
        
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth); // event for all

        if (NetworkedHealth <= 0)
        {
            //crate.GetComponent<HealthCrate>().idOfSpawner = Object.StateAuthority.PlayerId;
            
            //death for all
            bodyCollider.enabled = false;
            characterController.enabled = false;
            bodyMeshRen.enabled = false;
            gunMeshRen.enabled = false;
        }

        if (HasStateAuthority)
        {
            OnLocalPlayerChangeHealthEvent?.Invoke(NetworkedHealth,MaxHealth); // event for own UI HUD
            
            if (NetworkedHealth <= 0)
            {
                //death 
                if (IsDead) return;
                IsDead = true;

                RPC_SpawnHealthCrate();

                GetComponent<PlayerMovement>().CanMove = false;
                GetComponent<PlayerWeapon>().CanShoot = false;
                
                bodyCollider.enabled = false;
                characterController.enabled = false;
                bodyMeshRen.enabled = false;
                gunMeshRen.enabled = false;
                
                
                
                if (Camera.main != null) Camera.main.GetComponent<FirstPersonCamera>().SetDeathCamPos(deathCamTransform);
                OnDeathEvent?.Invoke(transform.position); // effect
                //add update count kill and death
                RPC_RelayDeathInfo(Object.StateAuthority.PlayerId, _shooterID);
                //Start coroutine here before respawning
                
                playerTeam.SpawnAndSetColor();
                
                StartCoroutine(RespawnTime());
                //Runner.Despawn(Object);
            }
        }

        
    }
    
    Vector3 GetGroundPosition()
    {
        if (Physics.Raycast(transform.position, Vector3.down,out var hit))
        {
            return hit.point;
        }
    
        return transform.position;
    }

    public bool GetIsFullHealth()
    {
        return NetworkedHealth == MaxHealth;
    }

    
    [Rpc]
    public void RPC_RelayDeathInfo(int idDead, int idKiller)
    {
        OnDeathUpdateKDEvent?.Invoke(idDead, idKiller);
    }
    
    [Rpc]
    public void RPC_SpawnHealthCrate()
    {
        Runner.Spawn(healthCrate, GetGroundPosition(), Quaternion.identity);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage, int idShooter)
    {
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        float tempHealth = NetworkedHealth;

        tempHealth -= damage;

        if (tempHealth <= 0)
        {
            tempHealth = 0;
        }
        
        NetworkedHealth = tempHealth;
        _shooterID = idShooter;
        
        if (HasStateAuthority)
        {
            OnLocalPlayerHurtEvent?.Invoke(); // event for own UI HUD
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void HealRpc(float healAmount)
    {
        if (!HasStateAuthority) return;
        float currentHealth = NetworkedHealth;
        currentHealth += healAmount;

        if (currentHealth >= MaxHealth) currentHealth = MaxHealth;
        NetworkedHealth = currentHealth;
    }

    public IEnumerator RespawnTime()
    {
        yield return new WaitForSeconds(5f);
        Respawn();
    }

}