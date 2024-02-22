using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class HeroSelectorManager : MonoBehaviour
    {
        [SerializeField] GameObject _aura;
        [SerializeField] GameObject _effect;
        [SerializeField] GameObject _gfx;

        [SerializeField] float _gfxShowDelay = 0.5f;

        Animator _animator;

        public void Initialize()
        {
            _animator = GetComponentInChildren<Animator>();
            _gfx.SetActive(false);
            _aura.SetActive(false);
            _effect.SetActive(false);
        }

        public void Show()
        {
            StartCoroutine(ShowCoroutine());
        }

        IEnumerator ShowCoroutine()
        {
            _effect.SetActive(true);
            yield return new WaitForSeconds(_gfxShowDelay);
            _gfx.SetActive(true);
            _animator.SetBool("Grounded", true);
            yield return new WaitForSeconds(0.5f);
            _aura.SetActive(true);
        }

        public void Hide()
        {
            _gfx.SetActive(false);
            _aura.SetActive(false);
            _effect.SetActive(false);

            StartCoroutine(HideCoroutine());
        }

        IEnumerator HideCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _effect.SetActive(false);
        }
    }
}