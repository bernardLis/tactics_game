using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapControlsManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    MapSetupManager _mapSetupManager;
    DraggableArmies _draggableArmies;
    CameraSmoothFollow _cameraSmoothFollow;

    VisualElement _root;

    VisualElement _controls;

    List<CastleControlButton> _castleControlsButtons = new();
    List<HeroControlButton> _heroControlsButtons = new();
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;
        _draggableArmies = GetComponent<DraggableArmies>();
        _mapSetupManager = GetComponent<MapSetupManager>();

        _cameraSmoothFollow = Camera.main.GetComponent<CameraSmoothFollow>();

        _root = _dashboardManager.Root;
        _controls = _root.Q<VisualElement>("controls");
        AddControlButtons();
    }

    void AddControlButtons()
    {
        AddCastleControls();
        AddHeroControls();
    }

    void AddCastleControls()
    {
        Map map = _gameManager.Map;
        // TODO: when are owned then add to controls
        // TODO: when new castle is taken then add to controls
        // TODO: when castle is lost then remove from controls
        foreach (Castle c in map.Castles)
            AddCastleButton(c);
    }

    void AddCastleButton(Castle c)
    {
        // get correct map castle
        MapCastle mapCastle = null;
        foreach (MapCastle mc in _mapSetupManager.MapCastles)
            if (mc.Castle == c)
                mapCastle = mc;

        CastleControlButton button = new(mapCastle, _root, _draggableArmies);
        _controls.Add(button);
        _castleControlsButtons.Add(button);
    }

    void AddHeroControls()
    {
        _gameManager.OnCharacterAddedToTroops += AddHeroButton;
        _gameManager.OnCharacterRemovedFromTroops += RemoveHeroButton;
        foreach (Character c in _gameManager.GetAllCharacters())
            AddHeroButton(c);
    }

    void AddHeroButton(Character c)
    {
        MapHero mapHero = null;
        foreach (MapHero mc in _mapSetupManager.MapHeroes)
            if (mc.Character == c)
                mapHero = mc;

        HeroControlButton button = new(mapHero, _root, _draggableArmies);
        _controls.Add(button);
        _heroControlsButtons.Add(button);
    }

    void RemoveHeroButton(Character c)
    {
        foreach (HeroControlButton b in _heroControlsButtons)
        {
            if (b.MapHero.Character == c)
            {
                _controls.Remove(b);
                _heroControlsButtons.Remove(b);
                return;
            }
        }
    }

}
