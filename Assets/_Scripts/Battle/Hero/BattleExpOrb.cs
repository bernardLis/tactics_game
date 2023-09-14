using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using MoreMountains.Feedbacks;

public class BattleExpOrb : MonoBehaviour
{
    public Ease Ease;
    MMF_Player _feelPlayer;
    Color _color;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();

        _color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));

        GetComponent<Sphere>().Color = _color;

        float endY = Random.Range(2f, 4f);
        float timeY = Random.Range(1f, 3f);

        transform.DOMoveY(endY, timeY)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(transform);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out BattleHero hero))
        {
            DisplayText(Ease.ToString(), _color);
            transform.DOKill();

            float punchDuration = 0.5f;
            transform.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1, 1f);

            Vector3 jumpPos = new Vector3(
                hero.transform.position.x,
                hero.transform.position.y + 2f,
                hero.transform.position.z
            );
            float jumpDuration = 0.6f;
            
            transform.DOScale(0, jumpDuration)
                .SetDelay(punchDuration);

            transform.DOJump(jumpPos, 5f, 1, jumpDuration)
                    .SetDelay(punchDuration)
                    .SetEase(Ease)
                    .OnComplete(() =>
                    {
                        hero.CollectExpOrb(this);
                        Destroy(gameObject);
                    });
        }
    }

    void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}

