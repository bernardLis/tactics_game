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
        AddCharacterControls();
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
        VisualElement castle = new VisualElement();
        castle.style.width = 100;
        castle.style.height = 100;
        castle.style.backgroundImage = new StyleBackground(c.Sprite);
        _controls.Add(castle);
        castle.RegisterCallback<PointerDownEvent>(e =>
        {
            CastleElement el = new(_root, c);
            el.OnHide += _draggableArmies.Reset;
            _draggableArmies.Initialize();
        });

        castle.RegisterCallback<MouseEnterEvent>(e =>
        {
            // get correct map castle
            MapCastle mapCastle = null;
            foreach (MapCastle mc in _mapSetupManager.MapCastles)
                if (mc.Castle == c)
                    mapCastle = mc;
            _cameraSmoothFollow.MoveTo(mapCastle.transform.position);
            mapCastle.Highlight();
        });

        castle.RegisterCallback<MouseLeaveEvent>(e =>
        {
            // get correct map castle
            MapCastle mapCastle = null;
            foreach (MapCastle mc in _mapSetupManager.MapCastles)
                if (mc.Castle == c)
                    mapCastle = mc;
            mapCastle.ClearHighlight();
        });
    }

    void AddCharacterControls()
    {
        _gameManager.OnCharacterAddedToTroops += AddCharacterButton;
        _gameManager.OnCharacterRemovedFromTroops += RemoveCharacterButton;
        foreach (Character c in _gameManager.GetAllCharacters())
            AddCharacterButton(c);
    }

    void AddCharacterButton(Character c)
    {
        VisualElement character = new VisualElement();
        character.style.width = 100;
        character.style.height = 100;
        character.style.backgroundImage = new StyleBackground(c.Portrait.Sprite);
        _controls.Add(character);
        character.RegisterCallback<PointerDownEvent>(e =>
        {
            // TODO: maybe on click center the camera on map here and select them
            MapHero mapHero = null;
            foreach (MapHero mc in _mapSetupManager.MapHeroes)
                if (mc.Character == c)
                    mapHero = mc;

            Debug.Log($"{c.CharacterName} click show me a full screen card with all info");
        });

        character.RegisterCallback<MouseEnterEvent>(e =>
        {
            // get correct map castle
            MapHero mapHero = null;
            foreach (MapHero mc in _mapSetupManager.MapHeroes)
                if (mc.Character == c)
                    mapHero = mc;
            _cameraSmoothFollow.MoveTo(mapHero.transform.position);
        });
    }

    void RemoveCharacterButton(Character c)
    {
        // TODO: buttons are an element that knows their character
        // and I can remove them by character coz there is a list of them held by this script 
    }

}
