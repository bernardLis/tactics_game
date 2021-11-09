using DG.Tweening;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    SpriteRenderer rend;

    public void StartFlashing(Color col)
    {
        Color startColor = col * 0.7f;
        Color endColor = col * 0.6f; // https://docs.unity3d.com/ScriptReference/Color-operator_multiply.html

        rend = GetComponent<SpriteRenderer>();

        rend.color = startColor;
        transform.DOScale(0.9f, 1f).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
        rend.DOColor(endColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopFlashing()
    {
        // TODO: errors
        DOTween.Kill(transform);
        DOTween.Kill(rend);

        gameObject.SetActive(false);
        Invoke("SelfDestroy", 1f);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
