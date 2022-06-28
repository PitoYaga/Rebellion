using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
   [SerializeField] private StatsSaves _statsSaves;
   [SerializeField] private Player _playerCs;


   private void Start()
   {
      _statsSaves.HealthVar = _playerCs.playerMaxHealth;
      _statsSaves.RageVar = 0;
      _statsSaves.ShurikenVar = _playerCs.shurikenMagazine;
   }
}
