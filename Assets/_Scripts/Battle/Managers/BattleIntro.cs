using System.Collections;
using Codice.Client.Commands;
using DG.Tweening;
using UnityEngine;
using TMPro;


namespace Lis
{
    public class BattleIntro : MonoBehaviour
    {
        BattleManager _battleManager;

        [SerializeField] TMP_Text _timerText;
        [SerializeField] TMP_Text _introText;

        [SerializeField] Transform _effect;

        readonly string _introString = "Time is the soul of this world";

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _introText.text = _introString;
            StartCoroutine(DisplayIntroTextCoroutine());
            StartCoroutine(DisplayTimer());
        }

        IEnumerator DisplayIntroTextCoroutine()
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
            gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);

            // foreach (string s in _introStrings)
            // {
            //     _introText.text = s;
            //     yield return new WaitForSeconds(3f);
            // }
        }

        IEnumerator DisplayTimer()
        {
            while (true)
            {
                float t = _battleManager.GetTimeLeft();
                int minutes = Mathf.FloorToInt(t / 60f);
                int seconds = Mathf.FloorToInt(t - minutes * 60);

                _timerText.text = $"{minutes:00}:{seconds:00}";
                yield return new WaitForSeconds(1f);

                if (t <= 0) yield break;
            }
        }
    }
}