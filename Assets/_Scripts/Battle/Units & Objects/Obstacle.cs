using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Obstacle : MonoBehaviour
{
    BoxCollider2D _col;
    [SerializeField] Light2D _spotLightPrefab;

    public void Initialise(TilemapObject obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = obj.Sprite;

        _col = GetComponentInChildren<BoxCollider2D>();
        if (_col != null)
            _col.size = obj.Size;

        if (obj.IsEmitingLight)
        {
            Light2D l = Instantiate(_spotLightPrefab, transform.position, Quaternion.identity);
            l.transform.parent = transform;
            l.transform.localPosition = new Vector3(obj.Offset.x, obj.Offset.y);
            l.color = obj.LightColor;
            l.intensity = Random.Range(obj.LightIntensity.x, obj.LightIntensity.y);
            l.pointLightInnerRadius = Random.Range(obj.InnerRadius.x, obj.InnerRadius.y);
            l.pointLightOuterRadius = Random.Range(obj.OuterRadius.x, obj.OuterRadius.y);
        }

    }
}
