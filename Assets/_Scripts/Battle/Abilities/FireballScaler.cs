using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireballScaler : AbilityEffectScaler
{
    [SerializeField] Transform _cloud;
    [SerializeField] Transform _tornadoLoop;

    public override void Initialize(float scale)
    {
        base.Initialize(scale);

        float newCloudY = _cloud.position.y * scale;
        _cloud.position = new Vector3(_cloud.position.x, newCloudY, _cloud.position.z);

        _tornadoLoop.localScale *= scale;

        float newTornadoMaxPositionY = 3.3f * scale;
        _tornadoLoop.position = new Vector3(_tornadoLoop.position.x, -newTornadoMaxPositionY, _tornadoLoop.position.z);
        float newTornadoMaxScaleY = 1.5f * scale;

        _tornadoLoop.DOMoveY(0, 2f);
        _tornadoLoop.DOScaleY(newTornadoMaxScaleY, 2f);
    }
}
