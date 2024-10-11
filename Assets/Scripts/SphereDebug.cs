using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDebug : MonoBehaviour
{
   public bool isRedTeam = false;
   public float radius = 0.5f;
   public float lineLength = 0.5f;

   private void OnDrawGizmos()
   {
      Gizmos.color = isRedTeam ? Color.red : Color.blue;

      Gizmos.DrawSphere(transform.position, radius);
      Gizmos.DrawLine(transform.position, transform.position + (transform.forward * lineLength)); 
        
   }
}
