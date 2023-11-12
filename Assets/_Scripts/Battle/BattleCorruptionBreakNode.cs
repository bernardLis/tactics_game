using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DigitalRuby.ThunderAndLightning;
using System;

public class BattleCorruptionBreakNode : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] GameObject _corruptionNodeExplosionPrefab;

    BoxCollider _collider;
    LightningBoltPrefabScript _lightningScript;

    BattleBoss _boss;

    public event Action<BattleCorruptionBreakNode> OnNodeBroken;

    public void Initialize(BattleBoss boss, Vector3 pos)
    {
        _gameManager = GameManager.Instance;

        _boss = boss;

        _collider = GetComponent<BoxCollider>();

        _lightningScript = GetComponent<LightningBoltPrefabScript>();
        _lightningScript.Source = gameObject;
        _lightningScript.Destination = boss.gameObject;
        _lightningScript.GlowTintColor = _gameManager.GameDatabase.GetColorByName("Stun").Color;
        _lightningScript.LightningTintColor = _gameManager.GameDatabase.GetColorByName("Stun").Color;
        _lightningScript.MainTrunkTintColor = _gameManager.GameDatabase.GetColorByName("Stun").Color;

        // fake arc movement
        transform.DOMoveX(pos.x, 1f).SetEase(Ease.OutQuad);
        transform.DOMoveZ(pos.z, 1f).SetEase(Ease.OutQuad);
        transform.DOMoveY(0.5f, 1f).SetEase(Ease.InQuad);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;

        StartCoroutine(NodeBrokenCoroutine());
    }

    IEnumerator NodeBrokenCoroutine()
    {
        yield return transform.DOMove(_boss.transform.position, 0.4f)
                              .SetEase(Ease.Flash)
                              .WaitForCompletion();
        Destroy(Instantiate(_corruptionNodeExplosionPrefab, transform.position, Quaternion.identity), 2f);
        yield return new WaitForSeconds(0.5f);
        OnNodeBroken?.Invoke(this);
        DestroySelf();
    }

    public void DestroySelf()
    {
        _collider.enabled = false;
        _lightningScript.enabled = false;
        transform.DOScale(0f, 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
