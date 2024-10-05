using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Base Weapons Parameters")] 
    public string weaponName;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public abstract void Shoot();
    
}
