using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Guns/Gun")]
public class Gun : ScriptableObject
{
    [Header("Gun Stats")]
    public string gunName;
    public float gunDamage = 10f;
    public int maxAmmo = 10;
    public float spreadAmount = 0.5f;
    public float fireRate = 0.2f;
    public float camShakeDuration = 0.1f;
    public float camShakeIntensity = 0.05f;
    public GameObject gunModel;

    public Vector3 FirePoint()
    {
        return gunModel.transform.GetChild(0).transform.localPosition;
    }
    
    public Mesh GunMesh()
    {
        return gunModel.GetComponent<MeshFilter>().sharedMesh;
    }
    
    public MeshRenderer GunMeshRenderer()
    {
        return gunModel.GetComponent<MeshRenderer>();
    }

}