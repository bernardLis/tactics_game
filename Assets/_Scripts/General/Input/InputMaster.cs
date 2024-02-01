//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/_Scripts/General/Input/InputMaster.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputMaster: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Battle"",
            ""id"": ""db8f36eb-7f70-4b70-b0fe-4fc302187b58"",
            ""actions"": [
                {
                    ""name"": ""ToggleCommandLine"",
                    ""type"": ""Button"",
                    ""id"": ""7872737d-8ce3-4153-8ef3-20c5c317d60b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleMenu"",
                    ""type"": ""Button"",
                    ""id"": ""ca7abc80-980a-4853-9771-b52a4f1792f4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlayerMovement"",
                    ""type"": ""Value"",
                    ""id"": ""0ee4eba9-19cd-4e20-83fc-65833f2af581"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shift"",
                    ""type"": ""Button"",
                    ""id"": ""cd30d0d9-d7f3-41e6-b3d8-950b540eb6b9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0ebe2236-c7f9-41e5-9143-195604586563"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ZoomCamera"",
                    ""type"": ""Value"",
                    ""id"": ""0acae7ae-0888-49c3-a1cb-e12b00201f40"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""a9379f80-4b7f-4c44-9980-5a16e80afa14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""1a7ea72d-fc37-4603-bb31-fa5597b5bd78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Space"",
                    ""type"": ""Button"",
                    ""id"": ""48c57600-1474-40eb-ae2a-7e1dc5b04924"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Enter"",
                    ""type"": ""Button"",
                    ""id"": ""7c4b637b-d155-46c8-8f7d-3159f2b1f1cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugSpawnMinionWave"",
                    ""type"": ""Button"",
                    ""id"": ""b9700237-27eb-4342-a7fb-c61afa329c42"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugSpawnTile"",
                    ""type"": ""Button"",
                    ""id"": ""7314b5d1-03c1-4a0e-a06d-2f6c252b9470"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugSpawnBossTile"",
                    ""type"": ""Button"",
                    ""id"": ""94f07135-4971-4f83-a3f6-199b6108e92a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugKillHero"",
                    ""type"": ""Button"",
                    ""id"": ""77c90cfd-3296-4dd9-b49b-7b450be68c53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""1"",
                    ""type"": ""Button"",
                    ""id"": ""a9fbf538-eea3-4950-b46b-7d02b60335dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""2"",
                    ""type"": ""Button"",
                    ""id"": ""a41d09fa-6b54-4ef0-adfe-affccd87b0c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""3"",
                    ""type"": ""Button"",
                    ""id"": ""a11f8ce1-191f-48cf-8889-9031b36f2a02"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""4"",
                    ""type"": ""Button"",
                    ""id"": ""15ac7324-c7ec-4d0e-a088-8eb757b62ca6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""215f93c1-5c23-430f-8dac-32e22923ab48"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleCommandLine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""acf0e8aa-0cd6-4035-bd7d-505abcb542cf"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""DebugSpawnMinionWave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f0ee215b-2bda-443c-8cc1-1c3980841876"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""DebugSpawnTile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18ff402d-bf6d-483f-8539-4f2e555f9063"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""DebugSpawnBossTile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0da91ea9-5dfe-4086-81bb-8cc412863e0c"",
                    ""path"": ""<Keyboard>/leftBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""DebugKillHero"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e35ab860-cd72-4a27-9bd9-3c1e6c952836"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""72261d3a-2946-4b1a-ad26-5ce3e17ddbb1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f571308f-f538-4e54-964a-2d1ed126372c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""41e75b3d-ec37-4d89-97ea-a8f4dc05aa78"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f2e1bd1e-6d00-4d91-9ec3-0d1708fa9a58"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""40d0221f-e831-4f0b-b5e3-1146345ff2ca"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1806e549-2603-4bd6-9f38-4f9c315f4279"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""db690b1a-21c7-4526-8b9a-255ce07d9528"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0963ba9a-bb97-491b-b820-5a97b60426c6"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""39ca2288-a209-4d47-b467-d86860fcbabe"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7a552cbc-ae10-4dba-8005-668862412bf8"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ZoomCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9b8dc67-1e67-4c34-84ff-9d98f04ebb0d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ToggleMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""484185e2-1cbc-4e7a-9c43-511d656da14b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9df3ee30-15d4-444b-88be-7fff8c6b4a62"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""769fd4b8-c18a-45a7-8365-f4bb2fb1dea7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7891c333-9207-4927-971d-1c5bfc27854a"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16325e6b-391d-4203-b81f-c6390e663714"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c866da6-cc23-4720-8728-97a4e3590d69"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Space"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""29c04557-3559-4b26-a3b5-8379667fbe17"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""980e61c6-42f3-4827-949f-33c139270543"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24feca61-bc21-4f83-b2cd-5b4ed3826bf8"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c6334f9-05a2-45b7-8f0b-81f48e536f44"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Battle
        m_Battle = asset.FindActionMap("Battle", throwIfNotFound: true);
        m_Battle_ToggleCommandLine = m_Battle.FindAction("ToggleCommandLine", throwIfNotFound: true);
        m_Battle_ToggleMenu = m_Battle.FindAction("ToggleMenu", throwIfNotFound: true);
        m_Battle_PlayerMovement = m_Battle.FindAction("PlayerMovement", throwIfNotFound: true);
        m_Battle_Shift = m_Battle.FindAction("Shift", throwIfNotFound: true);
        m_Battle_Interact = m_Battle.FindAction("Interact", throwIfNotFound: true);
        m_Battle_ZoomCamera = m_Battle.FindAction("ZoomCamera", throwIfNotFound: true);
        m_Battle_RightMouseClick = m_Battle.FindAction("RightMouseClick", throwIfNotFound: true);
        m_Battle_LeftMouseClick = m_Battle.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Battle_Space = m_Battle.FindAction("Space", throwIfNotFound: true);
        m_Battle_Enter = m_Battle.FindAction("Enter", throwIfNotFound: true);
        m_Battle_DebugSpawnMinionWave = m_Battle.FindAction("DebugSpawnMinionWave", throwIfNotFound: true);
        m_Battle_DebugSpawnTile = m_Battle.FindAction("DebugSpawnTile", throwIfNotFound: true);
        m_Battle_DebugSpawnBossTile = m_Battle.FindAction("DebugSpawnBossTile", throwIfNotFound: true);
        m_Battle_DebugKillHero = m_Battle.FindAction("DebugKillHero", throwIfNotFound: true);
        m_Battle__1 = m_Battle.FindAction("1", throwIfNotFound: true);
        m_Battle__2 = m_Battle.FindAction("2", throwIfNotFound: true);
        m_Battle__3 = m_Battle.FindAction("3", throwIfNotFound: true);
        m_Battle__4 = m_Battle.FindAction("4", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Battle
    private readonly InputActionMap m_Battle;
    private List<IBattleActions> m_BattleActionsCallbackInterfaces = new List<IBattleActions>();
    private readonly InputAction m_Battle_ToggleCommandLine;
    private readonly InputAction m_Battle_ToggleMenu;
    private readonly InputAction m_Battle_PlayerMovement;
    private readonly InputAction m_Battle_Shift;
    private readonly InputAction m_Battle_Interact;
    private readonly InputAction m_Battle_ZoomCamera;
    private readonly InputAction m_Battle_RightMouseClick;
    private readonly InputAction m_Battle_LeftMouseClick;
    private readonly InputAction m_Battle_Space;
    private readonly InputAction m_Battle_Enter;
    private readonly InputAction m_Battle_DebugSpawnMinionWave;
    private readonly InputAction m_Battle_DebugSpawnTile;
    private readonly InputAction m_Battle_DebugSpawnBossTile;
    private readonly InputAction m_Battle_DebugKillHero;
    private readonly InputAction m_Battle__1;
    private readonly InputAction m_Battle__2;
    private readonly InputAction m_Battle__3;
    private readonly InputAction m_Battle__4;
    public struct BattleActions
    {
        private @InputMaster m_Wrapper;
        public BattleActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleCommandLine => m_Wrapper.m_Battle_ToggleCommandLine;
        public InputAction @ToggleMenu => m_Wrapper.m_Battle_ToggleMenu;
        public InputAction @PlayerMovement => m_Wrapper.m_Battle_PlayerMovement;
        public InputAction @Shift => m_Wrapper.m_Battle_Shift;
        public InputAction @Interact => m_Wrapper.m_Battle_Interact;
        public InputAction @ZoomCamera => m_Wrapper.m_Battle_ZoomCamera;
        public InputAction @RightMouseClick => m_Wrapper.m_Battle_RightMouseClick;
        public InputAction @LeftMouseClick => m_Wrapper.m_Battle_LeftMouseClick;
        public InputAction @Space => m_Wrapper.m_Battle_Space;
        public InputAction @Enter => m_Wrapper.m_Battle_Enter;
        public InputAction @DebugSpawnMinionWave => m_Wrapper.m_Battle_DebugSpawnMinionWave;
        public InputAction @DebugSpawnTile => m_Wrapper.m_Battle_DebugSpawnTile;
        public InputAction @DebugSpawnBossTile => m_Wrapper.m_Battle_DebugSpawnBossTile;
        public InputAction @DebugKillHero => m_Wrapper.m_Battle_DebugKillHero;
        public InputAction @_1 => m_Wrapper.m_Battle__1;
        public InputAction @_2 => m_Wrapper.m_Battle__2;
        public InputAction @_3 => m_Wrapper.m_Battle__3;
        public InputAction @_4 => m_Wrapper.m_Battle__4;
        public InputActionMap Get() { return m_Wrapper.m_Battle; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BattleActions set) { return set.Get(); }
        public void AddCallbacks(IBattleActions instance)
        {
            if (instance == null || m_Wrapper.m_BattleActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BattleActionsCallbackInterfaces.Add(instance);
            @ToggleCommandLine.started += instance.OnToggleCommandLine;
            @ToggleCommandLine.performed += instance.OnToggleCommandLine;
            @ToggleCommandLine.canceled += instance.OnToggleCommandLine;
            @ToggleMenu.started += instance.OnToggleMenu;
            @ToggleMenu.performed += instance.OnToggleMenu;
            @ToggleMenu.canceled += instance.OnToggleMenu;
            @PlayerMovement.started += instance.OnPlayerMovement;
            @PlayerMovement.performed += instance.OnPlayerMovement;
            @PlayerMovement.canceled += instance.OnPlayerMovement;
            @Shift.started += instance.OnShift;
            @Shift.performed += instance.OnShift;
            @Shift.canceled += instance.OnShift;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @ZoomCamera.started += instance.OnZoomCamera;
            @ZoomCamera.performed += instance.OnZoomCamera;
            @ZoomCamera.canceled += instance.OnZoomCamera;
            @RightMouseClick.started += instance.OnRightMouseClick;
            @RightMouseClick.performed += instance.OnRightMouseClick;
            @RightMouseClick.canceled += instance.OnRightMouseClick;
            @LeftMouseClick.started += instance.OnLeftMouseClick;
            @LeftMouseClick.performed += instance.OnLeftMouseClick;
            @LeftMouseClick.canceled += instance.OnLeftMouseClick;
            @Space.started += instance.OnSpace;
            @Space.performed += instance.OnSpace;
            @Space.canceled += instance.OnSpace;
            @Enter.started += instance.OnEnter;
            @Enter.performed += instance.OnEnter;
            @Enter.canceled += instance.OnEnter;
            @DebugSpawnMinionWave.started += instance.OnDebugSpawnMinionWave;
            @DebugSpawnMinionWave.performed += instance.OnDebugSpawnMinionWave;
            @DebugSpawnMinionWave.canceled += instance.OnDebugSpawnMinionWave;
            @DebugSpawnTile.started += instance.OnDebugSpawnTile;
            @DebugSpawnTile.performed += instance.OnDebugSpawnTile;
            @DebugSpawnTile.canceled += instance.OnDebugSpawnTile;
            @DebugSpawnBossTile.started += instance.OnDebugSpawnBossTile;
            @DebugSpawnBossTile.performed += instance.OnDebugSpawnBossTile;
            @DebugSpawnBossTile.canceled += instance.OnDebugSpawnBossTile;
            @DebugKillHero.started += instance.OnDebugKillHero;
            @DebugKillHero.performed += instance.OnDebugKillHero;
            @DebugKillHero.canceled += instance.OnDebugKillHero;
            @_1.started += instance.On_1;
            @_1.performed += instance.On_1;
            @_1.canceled += instance.On_1;
            @_2.started += instance.On_2;
            @_2.performed += instance.On_2;
            @_2.canceled += instance.On_2;
            @_3.started += instance.On_3;
            @_3.performed += instance.On_3;
            @_3.canceled += instance.On_3;
            @_4.started += instance.On_4;
            @_4.performed += instance.On_4;
            @_4.canceled += instance.On_4;
        }

        private void UnregisterCallbacks(IBattleActions instance)
        {
            @ToggleCommandLine.started -= instance.OnToggleCommandLine;
            @ToggleCommandLine.performed -= instance.OnToggleCommandLine;
            @ToggleCommandLine.canceled -= instance.OnToggleCommandLine;
            @ToggleMenu.started -= instance.OnToggleMenu;
            @ToggleMenu.performed -= instance.OnToggleMenu;
            @ToggleMenu.canceled -= instance.OnToggleMenu;
            @PlayerMovement.started -= instance.OnPlayerMovement;
            @PlayerMovement.performed -= instance.OnPlayerMovement;
            @PlayerMovement.canceled -= instance.OnPlayerMovement;
            @Shift.started -= instance.OnShift;
            @Shift.performed -= instance.OnShift;
            @Shift.canceled -= instance.OnShift;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @ZoomCamera.started -= instance.OnZoomCamera;
            @ZoomCamera.performed -= instance.OnZoomCamera;
            @ZoomCamera.canceled -= instance.OnZoomCamera;
            @RightMouseClick.started -= instance.OnRightMouseClick;
            @RightMouseClick.performed -= instance.OnRightMouseClick;
            @RightMouseClick.canceled -= instance.OnRightMouseClick;
            @LeftMouseClick.started -= instance.OnLeftMouseClick;
            @LeftMouseClick.performed -= instance.OnLeftMouseClick;
            @LeftMouseClick.canceled -= instance.OnLeftMouseClick;
            @Space.started -= instance.OnSpace;
            @Space.performed -= instance.OnSpace;
            @Space.canceled -= instance.OnSpace;
            @Enter.started -= instance.OnEnter;
            @Enter.performed -= instance.OnEnter;
            @Enter.canceled -= instance.OnEnter;
            @DebugSpawnMinionWave.started -= instance.OnDebugSpawnMinionWave;
            @DebugSpawnMinionWave.performed -= instance.OnDebugSpawnMinionWave;
            @DebugSpawnMinionWave.canceled -= instance.OnDebugSpawnMinionWave;
            @DebugSpawnTile.started -= instance.OnDebugSpawnTile;
            @DebugSpawnTile.performed -= instance.OnDebugSpawnTile;
            @DebugSpawnTile.canceled -= instance.OnDebugSpawnTile;
            @DebugSpawnBossTile.started -= instance.OnDebugSpawnBossTile;
            @DebugSpawnBossTile.performed -= instance.OnDebugSpawnBossTile;
            @DebugSpawnBossTile.canceled -= instance.OnDebugSpawnBossTile;
            @DebugKillHero.started -= instance.OnDebugKillHero;
            @DebugKillHero.performed -= instance.OnDebugKillHero;
            @DebugKillHero.canceled -= instance.OnDebugKillHero;
            @_1.started -= instance.On_1;
            @_1.performed -= instance.On_1;
            @_1.canceled -= instance.On_1;
            @_2.started -= instance.On_2;
            @_2.performed -= instance.On_2;
            @_2.canceled -= instance.On_2;
            @_3.started -= instance.On_3;
            @_3.performed -= instance.On_3;
            @_3.canceled -= instance.On_3;
            @_4.started -= instance.On_4;
            @_4.performed -= instance.On_4;
            @_4.canceled -= instance.On_4;
        }

        public void RemoveCallbacks(IBattleActions instance)
        {
            if (m_Wrapper.m_BattleActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IBattleActions instance)
        {
            foreach (var item in m_Wrapper.m_BattleActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BattleActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public BattleActions @Battle => new BattleActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IBattleActions
    {
        void OnToggleCommandLine(InputAction.CallbackContext context);
        void OnToggleMenu(InputAction.CallbackContext context);
        void OnPlayerMovement(InputAction.CallbackContext context);
        void OnShift(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnZoomCamera(InputAction.CallbackContext context);
        void OnRightMouseClick(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnSpace(InputAction.CallbackContext context);
        void OnEnter(InputAction.CallbackContext context);
        void OnDebugSpawnMinionWave(InputAction.CallbackContext context);
        void OnDebugSpawnTile(InputAction.CallbackContext context);
        void OnDebugSpawnBossTile(InputAction.CallbackContext context);
        void OnDebugKillHero(InputAction.CallbackContext context);
        void On_1(InputAction.CallbackContext context);
        void On_2(InputAction.CallbackContext context);
        void On_3(InputAction.CallbackContext context);
        void On_4(InputAction.CallbackContext context);
    }
}
