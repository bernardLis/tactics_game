using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero
{
    public class OpponentTracker : MonoBehaviour
    {
        public List<UnitController> OpponentsInRange = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out UnitController entity))
                AddOpponent(entity);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out UnitController entity))
                RemoveOpponent(entity);
        }

        public event Action<UnitController> OnOpponentAdded;

        private void AddOpponent(UnitController entity)
        {
            if (OpponentsInRange.Contains(entity)) return;
            entity.OnDeath += RemoveOpponentOnDeath;

            OpponentsInRange.Add(entity);
            OnOpponentAdded?.Invoke(entity);
        }

        public void RemoveOpponent(UnitController be)
        {
            if (OpponentsInRange.Contains(be))
                OpponentsInRange.Remove(be);
        }

        private void RemoveOpponentOnDeath(UnitController be, Attack.Attack attack)
        {
            be.OnDeath -= RemoveOpponentOnDeath;
            RemoveOpponent(be);
        }

        public Vector3 GetRandomOpponentPosition()
        {
            return OpponentsInRange.Count == 0
                ? Vector3.zero
                : OpponentsInRange[Random.Range(0, OpponentsInRange.Count)].transform.position;
        }
    }
}