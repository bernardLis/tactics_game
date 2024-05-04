using System;
using Lis.Battle;
using Lis.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Units
{
    public class UnitGrabController : MonoBehaviour, IGrabbable, IPointerDownHandler
    {
        UnitController _unitController;
        GrabManager _grabManager;
        Animator _animator;

        public event Action OnGrabbed;
        public event Action OnReleased;

        public void Initialize(UnitController unitController)
        {
            _unitController = unitController;
            _grabManager = GrabManager.Instance;
            _animator = GetComponentInChildren<Animator>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!CanBeGrabbed()) return;
            _grabManager.TryGrabbing(gameObject);
        }


        public bool CanBeGrabbed()
        {
            if (_unitController.IsDead) return false;
            if (_unitController.Team == 1) return false;
            return _grabManager != null;
        }

        public void Grabbed()
        {
            _unitController.AddToLog("Grabbed");
            _animator.enabled = false;
            OnGrabbed?.Invoke();
        }

        public void Released()
        {
            _unitController.AddToLog("Released");
            _animator.enabled = true;
            OnReleased?.Invoke();
        }
    }
}