//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
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

public partial class @InputMaster : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Map"",
            ""id"": ""342600f1-84d1-45cf-b9f5-5092c3c96e5b"",
            ""actions"": [
                {
                    ""name"": ""ArrowMovement"",
                    ""type"": ""Value"",
                    ""id"": ""c5f6c6b2-b7ba-41fa-842c-bd527ef87bc0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""OpenDesk"",
                    ""type"": ""Button"",
                    ""id"": ""0e794bfa-e59e-486d-b295-4c0ffb1d2ca3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CloseCurrentTab"",
                    ""type"": ""Button"",
                    ""id"": ""d4ab2cfa-149d-49d6-8625-2bc505c7c84f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleCommandLine"",
                    ""type"": ""Button"",
                    ""id"": ""e71b2f1d-50c9-4568-ac64-6b37d10cd15b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""47d0cca9-af75-471d-9fae-1d29668d84a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Space"",
                    ""type"": ""Button"",
                    ""id"": ""507a58eb-548c-4788-a96d-cf7a1f345c5b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""10b3e4dc-1e78-41ac-8502-dae1dec61a1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""ed617f9f-5f71-4804-8ba5-cadc58ab9c9e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""7253dc2f-382f-4cb5-9ce4-7c04073634e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shift"",
                    ""type"": ""Button"",
                    ""id"": ""ead3ea3c-67b0-4a7e-9cd1-051608458fd7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""4ba55dd0-4fff-47e6-b23a-42f0be4749cd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ArrowMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6f3c8180-2de6-4b93-adcd-4985a7ed4de2"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ArrowMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0c40f68c-3848-4c68-9da2-affedc90d5de"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ArrowMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26b505d9-c0ff-497b-bf05-dfe064f8bd35"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ArrowMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d20f40a3-4317-4c3a-9d90-667d730270aa"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ArrowMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""72c20cbd-bd46-4f49-b6e9-c7045925af89"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""OpenDesk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d1ff745-8a7c-40a1-abb4-b303fb285b49"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseCurrentTab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e4f3d52-8e89-40ec-87a5-6ba9b29affa3"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ToggleCommandLine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5b06460-93de-42cd-b3cd-722160a2e604"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff5d4f36-fbf0-4a3e-a977-06e261e6b308"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Space"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aaf02242-12d9-4da6-b5e6-0a0b3d52aa4f"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""149206bb-9bed-42db-8157-c898d5b1b944"",
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
                    ""id"": ""031003eb-bfa2-4882-8cd2-481c16928d92"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""RightMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb7c720c-b0f1-44ce-83f3-54fc585e7693"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Battle"",
            ""id"": ""db8f36eb-7f70-4b70-b0fe-4fc302187b58"",
            ""actions"": [
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
                    ""name"": ""RightMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""a9379f80-4b7f-4c44-9980-5a16e80afa14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
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
                    ""id"": ""1c6334f9-05a2-45b7-8f0b-81f48e536f44"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""2"",
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
        // Map
        m_Map = asset.FindActionMap("Map", throwIfNotFound: true);
        m_Map_ArrowMovement = m_Map.FindAction("ArrowMovement", throwIfNotFound: true);
        m_Map_OpenDesk = m_Map.FindAction("OpenDesk", throwIfNotFound: true);
        m_Map_CloseCurrentTab = m_Map.FindAction("CloseCurrentTab", throwIfNotFound: true);
        m_Map_ToggleCommandLine = m_Map.FindAction("ToggleCommandLine", throwIfNotFound: true);
        m_Map_OpenMenu = m_Map.FindAction("OpenMenu", throwIfNotFound: true);
        m_Map_Space = m_Map.FindAction("Space", throwIfNotFound: true);
        m_Map_Pause = m_Map.FindAction("Pause", throwIfNotFound: true);
        m_Map_LeftMouseClick = m_Map.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Map_RightMouseClick = m_Map.FindAction("RightMouseClick", throwIfNotFound: true);
        m_Map_Shift = m_Map.FindAction("Shift", throwIfNotFound: true);
        // Battle
        m_Battle = asset.FindActionMap("Battle", throwIfNotFound: true);
        m_Battle__1 = m_Battle.FindAction("1", throwIfNotFound: true);
        m_Battle__2 = m_Battle.FindAction("2", throwIfNotFound: true);
        m_Battle__3 = m_Battle.FindAction("3", throwIfNotFound: true);
        m_Battle__4 = m_Battle.FindAction("4", throwIfNotFound: true);
        m_Battle_LeftMouseClick = m_Battle.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Battle_RightMouseClick = m_Battle.FindAction("RightMouseClick", throwIfNotFound: true);
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

    // Map
    private readonly InputActionMap m_Map;
    private IMapActions m_MapActionsCallbackInterface;
    private readonly InputAction m_Map_ArrowMovement;
    private readonly InputAction m_Map_OpenDesk;
    private readonly InputAction m_Map_CloseCurrentTab;
    private readonly InputAction m_Map_ToggleCommandLine;
    private readonly InputAction m_Map_OpenMenu;
    private readonly InputAction m_Map_Space;
    private readonly InputAction m_Map_Pause;
    private readonly InputAction m_Map_LeftMouseClick;
    private readonly InputAction m_Map_RightMouseClick;
    private readonly InputAction m_Map_Shift;
    public struct MapActions
    {
        private @InputMaster m_Wrapper;
        public MapActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ArrowMovement => m_Wrapper.m_Map_ArrowMovement;
        public InputAction @OpenDesk => m_Wrapper.m_Map_OpenDesk;
        public InputAction @CloseCurrentTab => m_Wrapper.m_Map_CloseCurrentTab;
        public InputAction @ToggleCommandLine => m_Wrapper.m_Map_ToggleCommandLine;
        public InputAction @OpenMenu => m_Wrapper.m_Map_OpenMenu;
        public InputAction @Space => m_Wrapper.m_Map_Space;
        public InputAction @Pause => m_Wrapper.m_Map_Pause;
        public InputAction @LeftMouseClick => m_Wrapper.m_Map_LeftMouseClick;
        public InputAction @RightMouseClick => m_Wrapper.m_Map_RightMouseClick;
        public InputAction @Shift => m_Wrapper.m_Map_Shift;
        public InputActionMap Get() { return m_Wrapper.m_Map; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapActions set) { return set.Get(); }
        public void SetCallbacks(IMapActions instance)
        {
            if (m_Wrapper.m_MapActionsCallbackInterface != null)
            {
                @ArrowMovement.started -= m_Wrapper.m_MapActionsCallbackInterface.OnArrowMovement;
                @ArrowMovement.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnArrowMovement;
                @ArrowMovement.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnArrowMovement;
                @OpenDesk.started -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenDesk;
                @OpenDesk.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenDesk;
                @OpenDesk.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenDesk;
                @CloseCurrentTab.started -= m_Wrapper.m_MapActionsCallbackInterface.OnCloseCurrentTab;
                @CloseCurrentTab.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnCloseCurrentTab;
                @CloseCurrentTab.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnCloseCurrentTab;
                @ToggleCommandLine.started -= m_Wrapper.m_MapActionsCallbackInterface.OnToggleCommandLine;
                @ToggleCommandLine.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnToggleCommandLine;
                @ToggleCommandLine.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnToggleCommandLine;
                @OpenMenu.started -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnOpenMenu;
                @Space.started -= m_Wrapper.m_MapActionsCallbackInterface.OnSpace;
                @Space.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnSpace;
                @Space.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnSpace;
                @Pause.started -= m_Wrapper.m_MapActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnPause;
                @LeftMouseClick.started -= m_Wrapper.m_MapActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnLeftMouseClick;
                @RightMouseClick.started -= m_Wrapper.m_MapActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnRightMouseClick;
                @Shift.started -= m_Wrapper.m_MapActionsCallbackInterface.OnShift;
                @Shift.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnShift;
                @Shift.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnShift;
            }
            m_Wrapper.m_MapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ArrowMovement.started += instance.OnArrowMovement;
                @ArrowMovement.performed += instance.OnArrowMovement;
                @ArrowMovement.canceled += instance.OnArrowMovement;
                @OpenDesk.started += instance.OnOpenDesk;
                @OpenDesk.performed += instance.OnOpenDesk;
                @OpenDesk.canceled += instance.OnOpenDesk;
                @CloseCurrentTab.started += instance.OnCloseCurrentTab;
                @CloseCurrentTab.performed += instance.OnCloseCurrentTab;
                @CloseCurrentTab.canceled += instance.OnCloseCurrentTab;
                @ToggleCommandLine.started += instance.OnToggleCommandLine;
                @ToggleCommandLine.performed += instance.OnToggleCommandLine;
                @ToggleCommandLine.canceled += instance.OnToggleCommandLine;
                @OpenMenu.started += instance.OnOpenMenu;
                @OpenMenu.performed += instance.OnOpenMenu;
                @OpenMenu.canceled += instance.OnOpenMenu;
                @Space.started += instance.OnSpace;
                @Space.performed += instance.OnSpace;
                @Space.canceled += instance.OnSpace;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @RightMouseClick.started += instance.OnRightMouseClick;
                @RightMouseClick.performed += instance.OnRightMouseClick;
                @RightMouseClick.canceled += instance.OnRightMouseClick;
                @Shift.started += instance.OnShift;
                @Shift.performed += instance.OnShift;
                @Shift.canceled += instance.OnShift;
            }
        }
    }
    public MapActions @Map => new MapActions(this);

    // Battle
    private readonly InputActionMap m_Battle;
    private IBattleActions m_BattleActionsCallbackInterface;
    private readonly InputAction m_Battle__1;
    private readonly InputAction m_Battle__2;
    private readonly InputAction m_Battle__3;
    private readonly InputAction m_Battle__4;
    private readonly InputAction m_Battle_LeftMouseClick;
    private readonly InputAction m_Battle_RightMouseClick;
    public struct BattleActions
    {
        private @InputMaster m_Wrapper;
        public BattleActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @_1 => m_Wrapper.m_Battle__1;
        public InputAction @_2 => m_Wrapper.m_Battle__2;
        public InputAction @_3 => m_Wrapper.m_Battle__3;
        public InputAction @_4 => m_Wrapper.m_Battle__4;
        public InputAction @LeftMouseClick => m_Wrapper.m_Battle_LeftMouseClick;
        public InputAction @RightMouseClick => m_Wrapper.m_Battle_RightMouseClick;
        public InputActionMap Get() { return m_Wrapper.m_Battle; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BattleActions set) { return set.Get(); }
        public void SetCallbacks(IBattleActions instance)
        {
            if (m_Wrapper.m_BattleActionsCallbackInterface != null)
            {
                @_1.started -= m_Wrapper.m_BattleActionsCallbackInterface.On_1;
                @_1.performed -= m_Wrapper.m_BattleActionsCallbackInterface.On_1;
                @_1.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.On_1;
                @_2.started -= m_Wrapper.m_BattleActionsCallbackInterface.On_2;
                @_2.performed -= m_Wrapper.m_BattleActionsCallbackInterface.On_2;
                @_2.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.On_2;
                @_3.started -= m_Wrapper.m_BattleActionsCallbackInterface.On_3;
                @_3.performed -= m_Wrapper.m_BattleActionsCallbackInterface.On_3;
                @_3.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.On_3;
                @_4.started -= m_Wrapper.m_BattleActionsCallbackInterface.On_4;
                @_4.performed -= m_Wrapper.m_BattleActionsCallbackInterface.On_4;
                @_4.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.On_4;
                @LeftMouseClick.started -= m_Wrapper.m_BattleActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_BattleActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.OnLeftMouseClick;
                @RightMouseClick.started -= m_Wrapper.m_BattleActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.performed -= m_Wrapper.m_BattleActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.OnRightMouseClick;
            }
            m_Wrapper.m_BattleActionsCallbackInterface = instance;
            if (instance != null)
            {
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
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @RightMouseClick.started += instance.OnRightMouseClick;
                @RightMouseClick.performed += instance.OnRightMouseClick;
                @RightMouseClick.canceled += instance.OnRightMouseClick;
            }
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
    public interface IMapActions
    {
        void OnArrowMovement(InputAction.CallbackContext context);
        void OnOpenDesk(InputAction.CallbackContext context);
        void OnCloseCurrentTab(InputAction.CallbackContext context);
        void OnToggleCommandLine(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
        void OnSpace(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnRightMouseClick(InputAction.CallbackContext context);
        void OnShift(InputAction.CallbackContext context);
    }
    public interface IBattleActions
    {
        void On_1(InputAction.CallbackContext context);
        void On_2(InputAction.CallbackContext context);
        void On_3(InputAction.CallbackContext context);
        void On_4(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnRightMouseClick(InputAction.CallbackContext context);
    }
}
