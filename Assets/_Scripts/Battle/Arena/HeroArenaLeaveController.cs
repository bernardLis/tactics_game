using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    public class HeroArenaLeaveController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (FightManager.IsFightActive == false) return;
            if (other.TryGetComponent(out HeroController heroController))
            {
                heroController.StartAllAbilities();
                heroController.Hero.Speed.ApplyBonusValueChange(-3);
            }
        }
    }
}