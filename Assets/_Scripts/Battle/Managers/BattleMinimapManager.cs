using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleMinimapManager : MonoBehaviour
{
    const string _ussClassName = "minimap__";
    const string _ussPortalIcon = _ussClassName + "portal-icon";
    const string _ussPlayerEntityIcon = _ussClassName + "player-entity-icon";
    const string _ussPlayerTurretIcon = _ussClassName + "player-turret-icon";
    const string _ussPlayerEntityGraveIcon = _ussClassName + "grave-icon";
    const string _ussOpponentMinionIcon = _ussClassName + "opponent-minion-icon";
    const string _ussOpponentCreatureIcon = _ussClassName + "opponent-creature-icon";

    GameManager _gameManager;
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
    List<Transform> _keysToRemove = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MinimapStyles);
        if (ss != null)
            _battleManager.Root.styleSheets.Add(ss);

        _battleCameraManager = BattleCameraManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _minimap = _root.Q<VisualElement>("minimapContainer");
        _cameraIcon = _root.Q<VisualElement>("cameraIcon");
        _spireIcon = _root.Q<VisualElement>("spireIcon");

        StartCoroutine(DelayedStart());

        if (BattleIntroManager.Instance != null)
            BattleIntroManager.Instance.OnIntroFinished += () =>
                    DOTween.To(x => _minimap.style.opacity = x, 0, 1, 0.5f).SetDelay(2f);
        else
            DOTween.To(x => _minimap.style.opacity = x, 0, 1, 0.5f);
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
        _cameraIcon.transform.rotation = Quaternion.Euler(0, 0,
                _battleCameraManager.transform.eulerAngles.y - 90);
    }

    void AddPortals()
    {
        foreach (BattleOpponentPortal portal in _battleManager.OpponentPortals)
        {
            VisualElement icon = new();
            icon.AddToClassList(_ussPortalIcon);
            icon.style.backgroundImage = new StyleBackground(portal.Element.Icon);
            Vector2 pos = new Vector2(portal.transform.position.x, portal.transform.position.z);
            Vector2 miniMapPosition = GetMiniMapPosition(pos);
            icon.style.top = miniMapPosition.x;
            icon.style.left = miniMapPosition.y;

            icon.RegisterCallback<MouseUpEvent>((e) =>
            {
                if (e.button != 0) return;
                _battleCameraManager.CenterCameraOn(portal.transform);
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

            portal.OnGroupSpawned += () =>
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

        StartCoroutine(UpdateIcons());
    }

    IEnumerator UpdateIcons()
    {
        while (true)
        {
            foreach (KeyValuePair<Transform, VisualElement> kvp in _entityIcons)
            {
                if (kvp.Key == null) continue;
                Vector2 pos = new(kvp.Key.position.x, kvp.Key.position.z);
                Vector2 miniMapPosition = GetMiniMapPosition(pos);

                kvp.Value.style.visibility = Visibility.Visible;
                kvp.Value.style.top = miniMapPosition.x;
                kvp.Value.style.left = miniMapPosition.y;
            }

            foreach (Transform key in _keysToRemove)
            {
                if (_entityIcons.ContainsKey(key))
                {
                    _minimap.Remove(_entityIcons[key]);
                    _entityIcons.Remove(key);
                }
            }

            _keysToRemove.Clear();

            UpdateCameraIconPosition();
            UpdateCameraIconRotation();
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
        if (be is not BattleCreature bc) return;

        if (_entityIcons.ContainsKey(be.transform))
        {
            Debug.LogError("Entity already exists in minimap");
            return;
        }
        VisualElement icon = new();
        icon.style.backgroundImage = new StyleBackground(be.Entity.Icon);
        icon.style.visibility = Visibility.Hidden;
        icon.style.opacity = 0.8f;
        icon.usageHints = UsageHints.DynamicTransform;

        icon.RegisterCallback<MouseUpEvent>((e) =>
        {
            if (e.button != 0) return;
            _battleCameraManager.CenterCameraOn(be.transform);
            UpdateCameraIconRotation();
            _tooltipManager.ShowTooltip(be);
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
        if (be is not BattleCreature bc) return;

        if (be.Team == 0)
        {
            icon.AddToClassList(_ussPlayerEntityIcon);
            return;
        }

        icon.AddToClassList(_ussOpponentCreatureIcon);
    }

    public void AddGrave(BattleCreatureGrave grave)
    {
        VisualElement graveIcon = new();
        graveIcon.AddToClassList(_ussPlayerEntityGraveIcon);
        _minimap.Add(graveIcon);
        Vector2 pos = new(grave.transform.position.x, grave.transform.position.z);
        Vector2 miniMapPosition = GetMiniMapPosition(pos);

        graveIcon.style.top = miniMapPosition.x;
        graveIcon.style.left = miniMapPosition.y;

        grave.OnResurrected += () => _minimap.Remove(graveIcon);
    }

    void RemoveEntity(BattleEntity be)
    {
        if (be is not BattleCreature) return;
        if (!_entityIcons.ContainsKey(be.transform))
        {
            Debug.LogError("Entity does not exist in minimap");
            return;
        }
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
