using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleCreatureGrave : MonoBehaviour, IPointerDownHandler
{
    BattleTooltipManager _tooltipManager;

    [SerializeField] GameObject _entitySpawnerPrefab;

    public Creature Creature { get; private set; }

    GameObject _model;

    public void Initialize(Creature creature)
    {
        _tooltipManager = BattleTooltipManager.Instance;

        Creature = creature;

        transform.parent = BattleManager.Instance.EntityHolder;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        GameObject g = Instantiate(creature.Prefab);
        _model = Instantiate(g.GetComponentInChildren<Animator>().gameObject, transform.position, Quaternion.identity);
        //  _model.GetComponent<Animator>().enabled = false;
        _model.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = GameManager.Instance.GameDatabase.GrayScaleShader;
        _model.transform.SetParent(transform);
        _model.transform.localScale = Vector3.one * 0.5f;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log($"click click");
        GraveCard c = new(Creature);
        _tooltipManager.DisplayTooltip(c);

        c.OnResurrected += Resurrect;
    }

    void Resurrect()
    {
        // HERE: grave take into consideration the troops limit

        _tooltipManager.HideTooltip();
        GameObject g = Instantiate(_entitySpawnerPrefab, transform.position, Quaternion.identity);
        EntitySpawner es = g.GetComponent<EntitySpawner>();
        es.SpawnEntities(creatures: new List<Creature>() { Creature });
        es.OnSpawnComplete += (list) =>
        {
            BattleManager.Instance.AddPlayerArmyEntities(list);
            Destroy(gameObject, 0.2f);
        };
    }

}
