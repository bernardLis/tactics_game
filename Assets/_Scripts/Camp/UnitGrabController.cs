using System;
using Lis.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Camp
{
    public class UnitGrabController : MonoBehaviour, IGrabbable, IPointerDownHandler
    {
        Animator _animator;
        GrabManager _grabManager;

        public event Action OnGrabbed;
        public event Action OnReleased;

        public void Initialize()
        {
            _grabManager = GrabManager.Instance;
            _animator = GetComponentInChildren<Animator>();
        }

        public bool CanBeGrabbed()
        {
            return _grabManager != null;
        }

        public void Grabbed()
        {
            _animator.enabled = false;
            OnGrabbed?.Invoke();
        }

        public void Released()
        {
            _animator.enabled = true;
            OnReleased?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!CanBeGrabbed()) return;
            _grabManager.TryGrabbing(gameObject);
        }
    }
}