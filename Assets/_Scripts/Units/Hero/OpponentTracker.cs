using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class OpponentTracker : MonoBehaviour
    {
        public List<UnitController> OpponentsInRange = new();

        public event Action<UnitController> OnOpponentAdded;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out UnitController entity))
                AddOpponent(entity);
        }

        void AddOpponent(UnitController entity)
        {
            if (OpponentsInRange.Contains(entity)) return;
            entity.OnDeath += RemoveOpponentOnDeath;

            OpponentsInRange.Add(entity);
            OnOpponentAdded?.Invoke(entity);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out UnitController entity))
                RemoveOpponent(entity);
        }

        public void RemoveOpponent(UnitController be)
        {
            if (OpponentsInRange.Contains(be))
                OpponentsInRange.Remove(be);
        }

        void RemoveOpponentOnDeath(UnitController be, UnitController killer)
        {
            be.OnDeath -= RemoveOpponentOnDeath;
            RemoveOpponent(be);
        }
    }
}