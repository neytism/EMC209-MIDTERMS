using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private Vector3 pos = new Vector3();
    private Quaternion rot = new Quaternion();

    public bool isDead;
    

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        if (isDead)
        {
            transform.position = pos;
            transform.rotation = rot;
            return;
        }

        transform.position = Target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

    public void SetDeathCamPos(Transform t)
    {
        isDead = true;
        pos = t.position;
        rot = t.rotation;
    }
}