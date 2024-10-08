using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : NetworkBehaviour
{

    public static event Action<int> OnAmmoChangeEvent; 
    public static event Action<string> OnGunChangeEvent; 
    public static event Action<Vector3> OnHitSomethingEvent; 
    public static event Action<string> OnFireEvent; 
    public static event Action<float, float> OnFireShakeEvent;
    
    private bool _canShoot = true;

    public bool CanShoot
    {
        get => _canShoot;
        set => _canShoot = value;
    }
    
    [Networked, OnChangedRender(nameof(OnWeaponChange))]
    public string NetworkedGunName { get; set; }

    public Gun currentGun;
    public int currentAmmo = 0;
    public GameObject currentGunObject;
    public Transform currentFirePoint;
    
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;
    
    
    private float _timeSinceLastFire;
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            if (currentGun == null) return;
            currentAmmo = currentGun.maxAmmo;
            OnAmmoChangeEvent?.Invoke(currentAmmo);
        }
    }

    private void Update()
    {
        //Debug
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.K))
        {
            playerHealth.DealDamageRpc(20, Object.StateAuthority.PlayerId);
        }
    }

    public void OnWeaponChange()
    {
        if (GunsManager.instance != null)
        {
            foreach (var gun in GunsManager.instance.Guns)
            {
                if (gun.gunName == NetworkedGunName)
                {
                    currentGunObject.GetComponent<MeshFilter>().sharedMesh = gun.GunMesh();
                    currentGunObject.GetComponent<MeshRenderer>().sharedMaterials = gun.GunMeshRenderer().sharedMaterials;

                    currentFirePoint.localPosition = gun.FirePoint();

                    currentGunObject.GetComponent<MeshRenderer>().enabled = true;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("No Gun Instance");
        }
            
    }

    public void SetWeapon(Gun newGun)
    {
       if (HasStateAuthority) 
       {
           if (currentGun != null && currentGun == newGun)
           {
               Reload();
               return;
           }

           if (newGun == null)
           {
               currentGun = null;
               currentAmmo = 0;
               string gunName = "[ No gun ]";
               NetworkedGunName = gunName; //send all
           }
           else
           {
               currentGun = newGun;
               currentAmmo = newGun.maxAmmo;
               NetworkedGunName = newGun.gunName; //send all
           }

           OnAmmoChangeEvent?.Invoke(currentAmmo);
           OnGunChangeEvent?.Invoke(NetworkedGunName);
            
       }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }

        if (!_canShoot) return;
        
        if (currentGun != null)
        {
            var ray = playerMovement.mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            ray.origin += playerMovement.mainCamera.transform.forward;

            Vector3 spread = new Vector3(
                Random.Range(-currentGun.spreadAmount, currentGun.spreadAmount),
                Random.Range(-currentGun.spreadAmount, currentGun.spreadAmount),
                0f
            );

            Vector3 adjustedDirection = ray.direction + spread;

            _timeSinceLastFire -= Runner.DeltaTime;

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }

            if (Input.GetMouseButton(0))
            {
                if (_timeSinceLastFire > 0) return;

                currentAmmo--;

                if (currentAmmo < 0) currentAmmo = 0;

                OnAmmoChangeEvent?.Invoke(currentAmmo);
                

                if (currentAmmo <= 0)
                {
                    currentAmmo = 0;
                    Debug.Log("No Ammo Left");
                    return;
                }
                
                OnFireShakeEvent?.Invoke(currentGun.camShakeDuration, currentGun.camShakeIntensity);

                _timeSinceLastFire = currentGun.fireRate;

                if (Physics.Raycast(ray.origin, adjustedDirection, out var hit))
                {
                    if (HasStateAuthority)
                    {
                        OnHitSomethingEvent?.Invoke(hit.point);

                        Vector3Array startEnd = new Vector3Array
                        {
                            vectors = new[] { currentFirePoint.position, hit.point }
                        };

                        string ser = JsonUtility.ToJson(startEnd);
                        OnFireEvent?.Invoke(ser);

                        Debug.Log($"Firing and hit {hit.collider.gameObject.name}");

                        if (hit.collider.TryGetComponent<PlayerHealth>(out var health))
                        {
                            Debug.Log("Hit and dealing Damage");
                            health.DealDamageRpc(currentGun.gunDamage, Object.StateAuthority.PlayerId);
                        }
                    }
                }
            }
        }
        else
        {
            //no gun
        }
    }

    public void Reload()
    {
        if (!HasStateAuthority) return;
        
        if (currentGun == null) return;
        currentAmmo = currentGun.maxAmmo;
        OnAmmoChangeEvent?.Invoke(currentAmmo);
    }
    
    private void InteractWithObject()
    {
        var ray = playerMovement.mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.TryGetComponent<ReadyButton>(out var button)) 
            {
                button.Interact();
            }
        }
    }
}

[Serializable]
public class Vector3Array
{
    public Vector3[] vectors;
}
