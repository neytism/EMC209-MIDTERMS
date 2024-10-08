using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnSeconds : MonoBehaviour
{
   public float secsBeforeDestroy = 3f;
   private void OnEnable()
   {
      Destroy(gameObject, secsBeforeDestroy);
   }

   
}
