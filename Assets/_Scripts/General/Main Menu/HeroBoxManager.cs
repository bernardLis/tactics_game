using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class HeroBoxManager : MonoBehaviour
    {
        [SerializeField] GameObject _aura;
        [SerializeField] GameObject _effect;
        [SerializeField] GameObject _gfx;

        Animator _animator;

        public void Initialize()
        {
            _animator = _gfx.GetComponent<Animator>();
        }

        public void Show()
        {
            StartCoroutine(ShowCoroutine());
        }

        IEnumerator ShowCoroutine()
        {
            _effect.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gfx.SetActive(true);
            _animator.SetBool("Grounded", true);
            yield return new WaitForSeconds(0.5f);
            _aura.SetActive(true);
        }

        public void Hide()
        {
            _effect.SetActive(false);
            _gfx.SetActive(false);
            _aura.SetActive(false);
        }
    }
}