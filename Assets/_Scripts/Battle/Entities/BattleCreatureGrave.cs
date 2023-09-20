using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class BattleCreatureGrave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    [SerializeField] GameObject _entitySpawnerPrefab;
    public Creature Creature { get; private set; }

    GameObject _model;

    public event Action OnResurrected;
    public void Initialize(Creature creature)
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        AudioManager.Instance.PlayUI("Bang");

        Creature = creature;

        float scaleScaler = 0.5f;
        if (Helpers.ParseScriptableObjectName(Creature.name) == "Metalon") scaleScaler = 0.2f;

        transform.parent = BattleManager.Instance.EntityHolder;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        GameObject g = Instantiate(creature.Prefab);
        _model = Instantiate(g.GetComponentInChildren<Animator>().gameObject, transform.position, Quaternion.identity);

        _model.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = GameManager.Instance.GameDatabase.GrayScaleShader;
        _model.transform.SetParent(transform);
        _model.transform.localScale = Vector3.one * scaleScaler;
        _model.transform.localPosition = Vector3.zero;

        _model.transform.DOMoveY(1.2f, 2f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _model.transform.DOLocalMoveY(1.6f, 3f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
            });

        Destroy(g);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Here lies {Creature.EntityName} RIP in peace.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return;
        GraveCard c = new(Creature);
        _tooltipManager.ShowTooltip(c, gameObject);

        c.OnResurrected += Resurrect;
    }

    void Resurrect()
    {
        _tooltipManager.HideTooltip();
        GameObject g = Instantiate(_entitySpawnerPrefab, transform.position + Vector3.up * 3, Quaternion.identity);
        EntitySpawner es = g.GetComponent<EntitySpawner>();
        es.SpawnEntities(new List<Entity>() { Creature });
        es.OnSpawnComplete += (list) =>
        {
            OnResurrected?.Invoke();
            _model.transform.DOKill();
            _battleManager.AddPlayerArmyEntities(list);
            Destroy(gameObject, 0.2f);
        };
    }

}
