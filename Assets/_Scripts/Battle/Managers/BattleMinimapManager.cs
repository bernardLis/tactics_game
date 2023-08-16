using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleMinimapManager : MonoBehaviour
{
    const string _ussPortalIcon = "common__minimap-portal-icon";

    const string _ussPlayerEntityIcon = "common__minimap-player-entity-icon";
    const string _ussPlayerTurretIcon = "common__minimap-player-turret-icon";

    const string _ussOpponentMinionIcon = "common__minimap-opponent-minion-icon";
    const string _ussOpponentCreatureIcon = "common__minimap-opponent-creature-icon";

    BattleManager _battleManager;
    BattleCameraManager _battleCameraManager;
    BattleTooltipManager _tooltipManager;

    Vector2 mapSize = new Vector2(100, 100);
    Vector2 miniMapSize = new Vector2(400, 400);

    VisualElement _root;
    VisualElement _minimap;

    VisualElement _spireIcon;
    VisualElement _cameraIcon;

    Dictionary<Transform, VisualElement> _entityIcons = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleCameraManager = BattleCameraManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _minimap = _root.Q<VisualElement>("minimapContainer");
        _cameraIcon = _root.Q<VisualElement>("cameraIcon");
        _spireIcon = _root.Q<VisualElement>("spireIcon");

        StartCoroutine(DelayedStart());


        // TODO: minimap if it is resized then update the minimap size (and spire icon position)
        // TODO: minimap if the battle field is resized then update the size
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);

        miniMapSize = new Vector2(_minimap.resolvedStyle.width, _minimap.resolvedStyle.height);
        Vector2 spirePos = GetMiniMapPosition(new Vector2(0, 0));
        _spireIcon.style.top = spirePos.x; //- _spireIcon.resolvedStyle.width * 0.5f;
        _spireIcon.style.left = spirePos.y; //- _spireIcon.resolvedStyle.height * 0.5f;

        AddPortals();
        HandleEntities();

        _battleManager.OnPlayerTurretAdded += AddTurret;

        _battleCameraManager.OnCameraMoved += UpdateCameraIconPosition;
        _battleCameraManager.OnCameraRotated += UpdateCameraIconRotation;
    }

    void UpdateCameraIconPosition()
    {
        Vector3 camPos = _battleCameraManager.transform.position;
        float camPosX = camPos.x;
        float camPosZ = camPos.z;
        Vector2 miniMapPosition = GetMiniMapPosition(new Vector2(camPosX, camPosZ));

        _cameraIcon.style.top = miniMapPosition.x;
        _cameraIcon.style.left = miniMapPosition.y;
    }

    void UpdateCameraIconRotation()
    {
        _cameraIcon.transform.rotation = Quaternion.Euler(0, 0, _battleCameraManager.transform.eulerAngles.y + 90);
    }

    void AddPortals()
    {
        Debug.Log($"_battleManager.OpponentPortals.Count {_battleManager.OpponentPortals.Count}");
        foreach (BattleOpponentPortal portal in _battleManager.OpponentPortals)
        {
            VisualElement icon = new VisualElement();
            icon.AddToClassList(_ussPortalIcon);
            icon.style.backgroundImage = new StyleBackground(portal.Element.Icon);
            Vector2 pos = new Vector2(portal.transform.position.x, portal.transform.position.z);
            Vector2 miniMapPosition = GetMiniMapPosition(pos);
            icon.style.top = miniMapPosition.x;
            icon.style.left = miniMapPosition.y;

            icon.RegisterCallback<MouseUpEvent>((e) =>
            {
                if (e.button != 0) return;
                _battleCameraManager.CenterCameraOnTransform(portal.transform);
                // _tooltipManager.DisplayTooltip(portal);
            });

            icon.RegisterCallback<MouseEnterEvent>((e) =>
            {
                icon.style.opacity = 1;
            });

            icon.RegisterCallback<MouseLeaveEvent>((e) =>
            {
                icon.style.opacity = 0.8f;
            });

            _minimap.Add(icon);

            portal.OnWaveSpawned += () =>
            {
                icon.style.opacity = 1;

                DOTween.To(() => icon.transform.scale, x => icon.transform.scale = x, Vector3.one * 1.5f, 0.5f)
                    .SetLoops(4, LoopType.Yoyo)
                    .SetEase(Ease.Flash);
            };
        }
    }

    void HandleEntities()
    {
        AddCurrentEntities();
        _battleManager.OnPlayerCreatureAdded += (creature) => AddEntity(creature);
        _battleManager.OnPlayerEntityDeath += (entity) => RemoveEntity(entity);

        _battleManager.OnOpponentEntityAdded += (entity) => AddEntity(entity);
        _battleManager.OnOpponentEntityDeath += (entity) => RemoveEntity(entity);
        //    if (_spireUpgradeManager == null) _spireUpgradeManager = SpireUpgradeManager.Instance;
        //   _spireUpgradeManager.OnTurretAdded += (turret) => AddTurret(turret);

        StartCoroutine(UpdateEntityPositions());
    }

    IEnumerator UpdateEntityPositions()
    {
        while (true)
        {
            foreach (KeyValuePair<Transform, VisualElement> kvp in _entityIcons)
            {
                if (kvp.Key == null) continue;
                Vector2 pos = new Vector2(kvp.Key.position.x, kvp.Key.position.z);
                Vector2 miniMapPosition = GetMiniMapPosition(pos);

                kvp.Value.style.visibility = Visibility.Visible;
                kvp.Value.style.top = miniMapPosition.x;
                kvp.Value.style.left = miniMapPosition.y;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void AddCurrentEntities()
    {
        foreach (BattleEntity be in _battleManager.PlayerCreatures)
            AddEntity(be);
    }

    void AddEntity(BattleEntity be)
    {
        if (_entityIcons.ContainsKey(be.transform))
        {
            Debug.LogError("Entity already exists in minimap");
            return;
        }
        VisualElement icon = new VisualElement();
        icon.style.backgroundImage = new StyleBackground(be.Entity.Icon);
        icon.style.visibility = Visibility.Hidden;
        icon.style.opacity = 0.8f;
        icon.usageHints = UsageHints.DynamicTransform;

        icon.RegisterCallback<MouseUpEvent>((e) =>
        {
            if (e.button != 0) return;
            _battleCameraManager.CenterCameraOnTransform(be.transform);
            UpdateCameraIconRotation();
            _tooltipManager.DisplayTooltip(be);
        });

        icon.RegisterCallback<MouseEnterEvent>((e) =>
        {
            icon.style.opacity = 1;
            be.TurnHighlightOn(Color.white);
        });

        icon.RegisterCallback<MouseLeaveEvent>((e) =>
        {
            icon.style.opacity = 0.8f;
            be.TurnHighlightOff();
        });

        ResolveEntityIconStyle(be, icon);
        _entityIcons.Add(be.transform, icon);
        _minimap.Add(icon);
    }

    void ResolveEntityIconStyle(BattleEntity be, VisualElement icon)
    {
        if (be.Team == 0)
        {
            icon.AddToClassList(_ussPlayerEntityIcon);
            return;
        }

        icon.AddToClassList(_ussOpponentCreatureIcon);
    }

    void RemoveEntity(BattleEntity be)
    {
        if (!_entityIcons.ContainsKey(be.transform))
        {
            Debug.LogError("Entity does not exist in minimap");
            return;
        }
        _minimap.Remove(_entityIcons[be.transform]);
        _entityIcons.Remove(be.transform);
    }

    void AddTurret(BattleTurret turret)
    {
        if (_entityIcons.ContainsKey(turret.transform))
        {
            Debug.LogError("Entity does not exist in minimap");
            return;
        }
        VisualElement icon = new VisualElement();
        icon.style.visibility = Visibility.Hidden;
        icon.usageHints = UsageHints.DynamicTransform;
        icon.AddToClassList(_ussPlayerTurretIcon);
        _entityIcons.Add(turret.transform, icon);
        _minimap.Add(icon);
    }

    Vector2 GetMiniMapPosition(Vector2 worldPosition)
    {
        // TODO: math it eludes me why this works
        float x = miniMapSize.x * 0.5f - worldPosition.x.Remap(0, mapSize.x * 2, 0, miniMapSize.x);
        float z = miniMapSize.y * 0.5f - worldPosition.y.Remap(0, mapSize.y * 2, 0, miniMapSize.y);

        // because I want left to be right and top to be bottom
        x = miniMapSize.x - x;
        z = miniMapSize.y - z;

        // keep it in bounds
        x = Mathf.Clamp(x, 20, miniMapSize.x);
        z = Mathf.Clamp(z, 20, miniMapSize.y);

        return new Vector2(x, z);
    }
}
