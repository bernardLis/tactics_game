using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Obstacle : MonoBehaviour
{
    BoxCollider2D col;
    public Light2D spotLightPrefab;

    void Start()
    {
        col = GetComponentInChildren<BoxCollider2D>();
    }

    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.sprite;

        if (col != null)
            col.size = _obj.size;

        if (_obj.emitsLight)
        {
            Light2D l = Instantiate(spotLightPrefab, transform.position, Quaternion.identity);
            l.transform.parent = transform;
            l.transform.localPosition = new Vector3(_obj.offset.x, _obj.offset.y);
            l.color = _obj.lightColor;
            l.intensity = Random.Range(_obj.lightIntensity.x, _obj.lightIntensity.y);
            l.pointLightInnerRadius = Random.Range(_obj.innerRadius.x, _obj.innerRadius.y);
            l.pointLightOuterRadius = Random.Range(_obj.outerRadius.x, _obj.outerRadius.y);
        }

    }
}
