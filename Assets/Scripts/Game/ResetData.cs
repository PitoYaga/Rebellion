using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
   [SerializeField] private StatsSaves _statsSaves;
   private Player _playerCs;

   private void Start()
   {
      _playerCs = FindObjectOfType<Player>();
      
      _statsSaves.HealthVar = _playerCs.playerMaxHealth;
      _playerCs.playerHeathSlider.maxValue = _playerCs.playerMaxHealth;
      
      _statsSaves.RageVar = 0;
      _playerCs.rageBarSlider.maxValue = 0;
      
      _statsSaves.ShurikenVar = _playerCs.shurikenMagazine;
   }
}
