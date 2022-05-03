using UnityEngine;
using DG.Tweening;

public class Flasher : MonoBehaviour
{
    //BoxCollider2D _selfCollider;
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


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BattleInputController controller))
        {
            if (!controller.AllowInput)
                return;
            Vector3 rotate = new Vector3(0, 0, 90f);
            transform.DORotate(rotate, 0.5f).SetEase(Ease.InOutBack).OnComplete(RotateBack);
        }
    }

    void RotateBack()
    {
        Vector3 rotate = new Vector3(0, 0, 0);
        transform.DORotate(rotate, 0.5f).SetEase(Ease.InOutBack);
    }
}
