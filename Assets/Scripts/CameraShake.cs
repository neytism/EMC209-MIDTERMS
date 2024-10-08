using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Camera _cam;
    public float ShakeIntensity = 0.05f;

    private float shakeDuration = 0f;
    private Matrix4x4 originalProjectionMatrix;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        PlayerWeapon.OnFireShakeEvent += StartShake;
    }

    void Start()
    {
        
        if (_cam == null)
        {
            _cam = Camera.main;
        }
        
        originalProjectionMatrix = _cam.projectionMatrix;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            _cam.projectionMatrix = GetShakenMatrix(originalProjectionMatrix);

            shakeDuration -= Time.deltaTime;

            if (shakeDuration <= 0f)
            {
                _cam.projectionMatrix = originalProjectionMatrix; 
            }
        }
    }

    public void StartShake(float duration, float intensity)
    {
        shakeDuration = duration;
        ShakeIntensity = intensity;
    }

    private Matrix4x4 GetShakenMatrix(Matrix4x4 originalMatrix)
    {
        Matrix4x4 shakenMatrix = originalMatrix;

        shakenMatrix.m03 += Random.Range(-ShakeIntensity, ShakeIntensity);
        shakenMatrix.m13 += Random.Range(-ShakeIntensity, ShakeIntensity);

        return shakenMatrix;
    }
}