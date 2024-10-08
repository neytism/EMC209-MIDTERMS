using Fusion;
using UnityEngine;
using System.Collections;

public class WeaponCrate : NetworkBehaviour
{
    public Gun gun;
    public float rotationSpeed;
    public Transform weaponIndicator;

    public float timeBeforeObtainable = 10f;

    private bool isObtainable = true; 

    public override void FixedUpdateNetwork()
    {
        weaponIndicator.Rotate(0, rotationSpeed * Runner.DeltaTime, 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (isObtainable && col.gameObject.TryGetComponent<PlayerWeapon>(out var weapon))
        {
            weapon.SetWeapon(gun);
            weaponIndicator.gameObject.SetActive(false);
            isObtainable = false;
            StartCoroutine(ReenableCrate());
        }
    }

    private IEnumerator ReenableCrate()
    {
        yield return new WaitForSeconds(timeBeforeObtainable);
        weaponIndicator.gameObject.SetActive(true);
        isObtainable = true;
    }
}