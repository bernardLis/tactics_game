using UnityEngine;
using DG.Tweening;

public class Flasher : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;

    public void StartFlashing(Color col)
    {
        Color startColor = col * 0.7f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = startColor;

        transform.DOScale(0.9f, 1f).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopFlashing()
    {
        // TODO: errors
        DOTween.Kill(transform);
        gameObject.SetActive(false);
        Invoke("SelfDestroy", 1f);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
