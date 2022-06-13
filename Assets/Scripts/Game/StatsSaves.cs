using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatsSaves : ScriptableObject
{
   [SerializeField] private float healthVar;
   [SerializeField] private float rageVar;
   [SerializeField] private float shurikenVar;

   public float HealthVar
   {
      get
      {
         return healthVar;
      }
      set
      {
         healthVar = value;
      }
   }
   
   public float RageVar
   {
      get
      {
         return rageVar;
      }
      set
      {
         rageVar = value;
      }
   }
   
   public float ShurikenVar
   {
      get
      {
         return shurikenVar;
      }
      set
      {
         shurikenVar = value;
      }
   }
}
