
using UnityEngine;

public class GunsManager : MonoBehaviour
{
    public Gun[] Guns;

    public static GunsManager instance;

    private void Awake()
    {
        instance = this;
    }
}
