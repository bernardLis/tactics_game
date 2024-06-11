using System.Collections;
using UnityEngine;

namespace Lis.HeroSelection
{
    public class HeroDisplayer : MonoBehaviour
    {
        private static readonly int AnimGrounded = Animator.StringToHash("Grounded");
        [SerializeField] private GameObject _aura;
        [SerializeField] private GameObject _effect;
        [SerializeField] private GameObject _gfx;

        [SerializeField] private float _gfxShowDelay = 0.5f;

        private Animator _animator;

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

        private IEnumerator ShowCoroutine()
        {
            _effect.SetActive(true);
            yield return new WaitForSeconds(_gfxShowDelay);
            _gfx.SetActive(true);
            _animator.SetBool(AnimGrounded, true);
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

        private IEnumerator HideCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _effect.SetActive(false);
        }
    }
}