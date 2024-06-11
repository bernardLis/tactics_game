using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Lis.Battle
{
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _introText;
        [SerializeField] private Transform _effect;

        private readonly string _introString = "Time is the soul of this world";

        public void Initialize()
        {
            _introText.text = _introString;
            StartCoroutine(DisplayIntroTextCoroutine());
        }

        private IEnumerator DisplayIntroTextCoroutine()
        {
            yield return new WaitForSeconds(3f);
            _effect.DOScale(1.1f, 6f)
                .SetEase(Ease.InOutSine);

            _effect.DORotate(new(0, 360, 0), 6f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(3f);
            for (int i = 0; i < _introString.Length; i++)
            {
                if (_introString.Length - i == 4) break;
                _introText.text = _introString.Substring(0, _introString.Length - i);
                yield return new WaitForSeconds(0.1f);
            }

            _effect.DOScale(0f, 1f);


            yield return new WaitForSeconds(1f);
            transform.DOScale(0, 0.5f).SetEase(Ease.OutBack);
            StopAllCoroutines();
            transform.DOKill();
            GameObject g = gameObject;
            g.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Destroy(g);

            // foreach (string s in _introStrings)
            // {
            //     _introText.text = s;
            //     yield return new WaitForSeconds(3f);
            // }
        }
    }
}