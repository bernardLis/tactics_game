using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleHeroOpponentTracker : MonoBehaviour
    {
        public List<BattleEntity> OpponentsInRange = new();

        public event Action<BattleEntity> OnOpponentAdded;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BattleEntity entity))
                AddOpponent(entity);
        }

        void AddOpponent(BattleEntity entity)
        {
            if (OpponentsInRange.Contains(entity)) return;
            entity.OnDeath += RemoveOpponentOnDeath;

            OpponentsInRange.Add(entity);
            OnOpponentAdded?.Invoke(entity);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BattleEntity entity))
                RemoveOpponent(entity);
        }

        public void RemoveOpponent(BattleEntity be)
        {
            if (OpponentsInRange.Contains(be))
                OpponentsInRange.Remove(be);
        }

        void RemoveOpponentOnDeath(BattleEntity be, BattleEntity killer)
        {
            be.OnDeath -= RemoveOpponentOnDeath;
            RemoveOpponent(be);
        }
    }
}