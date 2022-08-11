using UnityEngine;
using DG.Tweening;
using Shapes;

public class Flasher : ImmediateModeShapeDrawer
{
    SpriteRenderer _spriteRenderer;
    Color _col = Color.black;

    bool _currentTile;

    public override void DrawShapes(Camera cam)
    {
        if (_currentTile)
            using (Draw.Command(cam))
                Draw.Disc(GetDiscPosition(1), 0.05f, _col);
    }

    Vector3 GetDiscPosition(float t)
    {
        float ang = t * ShapesMath.TAU; // base angle
        ang += ShapesMath.TAU * Time.time * 0.25f; // add constant spin rate
        ang += Mathf.Cos(ang * 2 + Time.time * ShapesMath.TAU * 0.5f) * 0.16f; // add wave offsets~

        Vector3 dir = ShapesMath.AngToDir(ang); // convert angle to a direction/position

        //https://gamedev.stackexchange.com/questions/167750/how-do-i-move-an-object-in-a-square-pattern
        float square = (transform.localScale.x * transform.localScale.x + transform.localScale.y * transform.localScale.y);
        float radius = 0.5f * Mathf.Sqrt(square);

        dir *= radius;
        dir += transform.position;

        dir.x = Mathf.Min(dir.x, transform.position.x + transform.localScale.x * 0.5f);
        dir.x = Mathf.Max(dir.x, transform.position.x - transform.localScale.x * 0.5f);
        dir.y = Mathf.Min(dir.y, transform.position.y + transform.localScale.y * 0.5f);
        dir.y = Mathf.Max(dir.y, transform.position.y - transform.localScale.y * 0.5f);

        return dir;
    }

    public void StartFlashing(Color col)
    {
        _col = col;
        Color startColor = col * 0.5f;
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

            _currentTile = true;
            Vector3 rotate = new Vector3(0, 0, 90f);
            transform.DORotate(rotate, 0.5f).SetEase(Ease.InOutBack).OnComplete(RotateBack);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out BattleInputController controller))
        {
            if (!controller.AllowInput)
                return;

            _currentTile = false;
        }
    }


    void RotateBack()
    {
        Vector3 rotate = new Vector3(0, 0, 0);
        transform.DORotate(rotate, 0.5f).SetEase(Ease.InOutBack);
    }
}
