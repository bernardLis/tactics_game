using Lis.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Lis.HeroCreation
{
    public class HeroRotator : MonoBehaviour, IDragHandler
    {
        Mouse _mouse;

        void Start()
        {
            _mouse = Mouse.current;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.Rotate(Vector3.up, -_mouse.delta.x.ReadValue());
        }
    }
}