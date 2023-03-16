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
            ""name"": ""Dashboard"",
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
                    ""name"": ""OpenCamp"",
                    ""type"": ""Button"",
                    ""id"": ""4d73cc88-7be5-4377-bc88-2cfa7dbe75ac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenAbilities"",
                    ""type"": ""Button"",
                    ""id"": ""d220e32f-b253-4ff5-a151-a7cc4ab60a40"",
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
                    ""id"": ""37cbebb5-9842-4562-9e7b-3fe357c3eea2"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenCamp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42ff2eb8-2fd2-4e57-9976-2ae1ee826fbb"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenAbilities"",
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
                    ""id"": ""9598014a-7b24-4c50-94c3-2cdbeccfef8a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
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
        // Dashboard
        m_Dashboard = asset.FindActionMap("Dashboard", throwIfNotFound: true);
        m_Dashboard_ArrowMovement = m_Dashboard.FindAction("ArrowMovement", throwIfNotFound: true);
        m_Dashboard_OpenDesk = m_Dashboard.FindAction("OpenDesk", throwIfNotFound: true);
        m_Dashboard_OpenCamp = m_Dashboard.FindAction("OpenCamp", throwIfNotFound: true);
        m_Dashboard_OpenAbilities = m_Dashboard.FindAction("OpenAbilities", throwIfNotFound: true);
        m_Dashboard_CloseCurrentTab = m_Dashboard.FindAction("CloseCurrentTab", throwIfNotFound: true);
        m_Dashboard_ToggleCommandLine = m_Dashboard.FindAction("ToggleCommandLine", throwIfNotFound: true);
        m_Dashboard_OpenMenu = m_Dashboard.FindAction("OpenMenu", throwIfNotFound: true);
        m_Dashboard_Space = m_Dashboard.FindAction("Space", throwIfNotFound: true);
        m_Dashboard_Pause = m_Dashboard.FindAction("Pause", throwIfNotFound: true);
        m_Dashboard_LeftMouseClick = m_Dashboard.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Dashboard_RightMouseClick = m_Dashboard.FindAction("RightMouseClick", throwIfNotFound: true);
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

    // Dashboard
    private readonly InputActionMap m_Dashboard;
    private IDashboardActions m_DashboardActionsCallbackInterface;
    private readonly InputAction m_Dashboard_ArrowMovement;
    private readonly InputAction m_Dashboard_OpenDesk;
    private readonly InputAction m_Dashboard_OpenCamp;
    private readonly InputAction m_Dashboard_OpenAbilities;
    private readonly InputAction m_Dashboard_CloseCurrentTab;
    private readonly InputAction m_Dashboard_ToggleCommandLine;
    private readonly InputAction m_Dashboard_OpenMenu;
    private readonly InputAction m_Dashboard_Space;
    private readonly InputAction m_Dashboard_Pause;
    private readonly InputAction m_Dashboard_LeftMouseClick;
    private readonly InputAction m_Dashboard_RightMouseClick;
    public struct DashboardActions
    {
        private @InputMaster m_Wrapper;
        public DashboardActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ArrowMovement => m_Wrapper.m_Dashboard_ArrowMovement;
        public InputAction @OpenDesk => m_Wrapper.m_Dashboard_OpenDesk;
        public InputAction @OpenCamp => m_Wrapper.m_Dashboard_OpenCamp;
        public InputAction @OpenAbilities => m_Wrapper.m_Dashboard_OpenAbilities;
        public InputAction @CloseCurrentTab => m_Wrapper.m_Dashboard_CloseCurrentTab;
        public InputAction @ToggleCommandLine => m_Wrapper.m_Dashboard_ToggleCommandLine;
        public InputAction @OpenMenu => m_Wrapper.m_Dashboard_OpenMenu;
        public InputAction @Space => m_Wrapper.m_Dashboard_Space;
        public InputAction @Pause => m_Wrapper.m_Dashboard_Pause;
        public InputAction @LeftMouseClick => m_Wrapper.m_Dashboard_LeftMouseClick;
        public InputAction @RightMouseClick => m_Wrapper.m_Dashboard_RightMouseClick;
        public InputActionMap Get() { return m_Wrapper.m_Dashboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DashboardActions set) { return set.Get(); }
        public void SetCallbacks(IDashboardActions instance)
        {
            if (m_Wrapper.m_DashboardActionsCallbackInterface != null)
            {
                @ArrowMovement.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnArrowMovement;
                @ArrowMovement.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnArrowMovement;
                @ArrowMovement.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnArrowMovement;
                @OpenDesk.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenDesk;
                @OpenDesk.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenDesk;
                @OpenDesk.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenDesk;
                @OpenCamp.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenCamp;
                @OpenCamp.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenCamp;
                @OpenCamp.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenCamp;
                @OpenAbilities.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenAbilities;
                @OpenAbilities.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenAbilities;
                @OpenAbilities.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenAbilities;
                @CloseCurrentTab.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnCloseCurrentTab;
                @CloseCurrentTab.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnCloseCurrentTab;
                @CloseCurrentTab.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnCloseCurrentTab;
                @ToggleCommandLine.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnToggleCommandLine;
                @ToggleCommandLine.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnToggleCommandLine;
                @ToggleCommandLine.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnToggleCommandLine;
                @OpenMenu.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnOpenMenu;
                @Space.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnSpace;
                @Space.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnSpace;
                @Space.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnSpace;
                @Pause.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnPause;
                @LeftMouseClick.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnLeftMouseClick;
                @RightMouseClick.started -= m_Wrapper.m_DashboardActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.performed -= m_Wrapper.m_DashboardActionsCallbackInterface.OnRightMouseClick;
                @RightMouseClick.canceled -= m_Wrapper.m_DashboardActionsCallbackInterface.OnRightMouseClick;
            }
            m_Wrapper.m_DashboardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ArrowMovement.started += instance.OnArrowMovement;
                @ArrowMovement.performed += instance.OnArrowMovement;
                @ArrowMovement.canceled += instance.OnArrowMovement;
                @OpenDesk.started += instance.OnOpenDesk;
                @OpenDesk.performed += instance.OnOpenDesk;
                @OpenDesk.canceled += instance.OnOpenDesk;
                @OpenCamp.started += instance.OnOpenCamp;
                @OpenCamp.performed += instance.OnOpenCamp;
                @OpenCamp.canceled += instance.OnOpenCamp;
                @OpenAbilities.started += instance.OnOpenAbilities;
                @OpenAbilities.performed += instance.OnOpenAbilities;
                @OpenAbilities.canceled += instance.OnOpenAbilities;
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
            }
        }
    }
    public DashboardActions @Dashboard => new DashboardActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IDashboardActions
    {
        void OnArrowMovement(InputAction.CallbackContext context);
        void OnOpenDesk(InputAction.CallbackContext context);
        void OnOpenCamp(InputAction.CallbackContext context);
        void OnOpenAbilities(InputAction.CallbackContext context);
        void OnCloseCurrentTab(InputAction.CallbackContext context);
        void OnToggleCommandLine(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
        void OnSpace(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnRightMouseClick(InputAction.CallbackContext context);
    }
}
