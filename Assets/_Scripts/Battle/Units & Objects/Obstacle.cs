using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Obstacle : MonoBehaviour
{
    BoxCollider2D col;
    public Light2D spotLightPrefab;

    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.Sprite;

        col = GetComponentInChildren<BoxCollider2D>();
        if (col != null)
            col.size = _obj.Size;

        if (_obj.IsEmitingLight)
        {
            Light2D l = Instantiate(spotLightPrefab, transform.position, Quaternion.identity);
            l.transform.parent = transform;
            l.transform.localPosition = new Vector3(_obj.Offset.x, _obj.Offset.y);
            l.color = _obj.LightColor;
            l.intensity = Random.Range(_obj.LightIntensity.x, _obj.LightIntensity.y);
            l.pointLightInnerRadius = Random.Range(_obj.InnerRadius.x, _obj.InnerRadius.y);
            l.pointLightOuterRadius = Random.Range(_obj.OuterRadius.x, _obj.OuterRadius.y);
        }

    }
}
