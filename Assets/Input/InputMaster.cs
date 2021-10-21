// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""e053d83d-8535-4868-a630-72c397b5d7c3"",
            ""actions"": [
                {
                    ""name"": ""QButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""618f08ab-7a07-40cf-a3c8-0f92bedb73d3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""8b102efc-3ce3-44f2-aa6b-63c50c2f0bcb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""9dee79f0-db68-4c3e-933b-afcafc5a5a5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""3dd55fee-0310-480a-951e-d111f267683f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""7c78f711-47be-47b3-b3e1-04865a1d14a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""6a73f7fa-bf06-4d7e-ac53-fa6e2c526e46"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowUpClick"",
                    ""type"": ""Button"",
                    ""id"": ""41d80404-79c8-4c07-bb6a-55568b64eca2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowDownClick"",
                    ""type"": ""Button"",
                    ""id"": ""54747ac8-dbe3-464d-b73c-87c128a022f5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""698338ce-238a-449f-830c-270be72b0f80"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""6faa3e42-d72b-4e67-8a54-f7d6d772fd4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectClick"",
                    ""type"": ""Button"",
                    ""id"": ""581d02ac-c4e3-418e-ac16-a6dbc3bbc39e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomOut"",
                    ""type"": ""Button"",
                    ""id"": ""0f94d516-b444-4a32-af0a-d47b86fc77e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomIn"",
                    ""type"": ""Button"",
                    ""id"": ""28561bfc-00d2-47fe-97ef-87a6df8882ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""44e43fc3-222a-44bd-8b0e-66a961cf5a4c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""QButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87633496-25f8-4720-8c9c-acaa5bf5b2e0"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""EButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56e8340e-13c9-40a9-9426-b65baf585e5b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""WButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""74675439-f35a-4098-ac29-ae7ea8d87b19"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""RButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1157a2df-7454-43be-84e8-c17f80af27ba"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe2c0d37-b13f-4f2f-8028-26b55420ca74"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efbe31bd-deaa-4b88-b46a-fc30e3cc9c32"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ArrowUpClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86fac896-41c7-41f7-b264-fda9ed6f3243"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ArrowDownClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20ce7863-0d9b-41c0-8323-60a7cf95c7a3"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ArrowLeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b08bc65f-d8e4-4a61-9d7e-54de02fa536d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ArrowRightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d72a8822-1533-426c-be42-e739aa829a07"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SelectClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aba80308-1ab1-4b94-af1d-95a9276bdab4"",
                    ""path"": ""<Keyboard>/period"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c220781-60e0-4033-a4db-76c06987f93c"",
                    ""path"": ""<Keyboard>/comma"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""QuestUI"",
            ""id"": ""3052e362-56ee-4f77-b5b7-5d5a335137f0"",
            ""actions"": [
                {
                    ""name"": ""Test"",
                    ""type"": ""Button"",
                    ""id"": ""72d3cb93-a3a7-477b-9cc4-e01878d426bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisableQuestUI"",
                    ""type"": ""Button"",
                    ""id"": ""94cab8f4-a6a5-464a-8311-c96b09f59e0a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuestMovement"",
                    ""type"": ""Button"",
                    ""id"": ""a549cc41-2f19-4355-b83a-b046e5346a8b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4aab5aea-1ed9-4e5b-b4b7-d4497f1d9b99"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Test"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85302d05-7c9d-43a9-ab4e-dbd71d4ebd95"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DisableQuestUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""237c8f38-9cad-410a-8ea1-7c21fada2514"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DisableQuestUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""402f2bf1-28c2-4a26-b4bc-5985dd44ba8f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuestMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c9b06aab-4fbe-4d7f-b0e1-511af07fd10e"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""QuestMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b9355ce9-afbf-43c6-83d4-8f064bb2a79b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""QuestMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3fc4ea66-a0e7-4dd2-a02b-e46c98e4cbd4"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""QuestMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""572a15e9-a33a-4500-819a-1151735cc7ec"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""QuestMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""InventoryUI"",
            ""id"": ""00730d4c-f8a2-47ad-a1e4-3f601bbd57b6"",
            ""actions"": [
                {
                    ""name"": ""Test"",
                    ""type"": ""Button"",
                    ""id"": ""a6df8010-b323-4296-a372-4c30ba82bf15"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DisableInventoryUI"",
                    ""type"": ""Button"",
                    ""id"": ""bd0e9c66-9512-4acf-850c-45c59ce61d5a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""InventoryMovement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e49492d9-ebe1-42f9-baca-c623ebbd984d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ba5e1909-cd3c-45b0-a068-18685bd5cfa2"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Test"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a80e7dae-7a49-4b08-9de5-b913f6bed235"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DisableInventoryUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e250808-275d-4b51-bcb2-0fcf67eefa67"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DisableInventoryUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""eba39aeb-b2c9-4622-9097-478a7ceea26e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventoryMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1d6498ab-247c-47d4-a64a-4a6408675c23"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InventoryMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1c0c6b8d-94f5-4158-bb79-93c8003aef53"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InventoryMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c31b0189-e25d-4f10-90de-8abd87322d31"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InventoryMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""57a384e3-eeee-4864-8500-5ef31a3bd189"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""InventoryMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Conversation"",
            ""id"": ""bba1ee92-5585-4d47-9802-db9688f45a02"",
            ""actions"": [
                {
                    ""name"": ""ConversationInteract"",
                    ""type"": ""Button"",
                    ""id"": ""727d1062-3fd9-402e-9e81-4c31dc72bd7d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9c5db3cf-8bfe-4a22-831e-58d59fc773aa"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ConversationInteract"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46ba40a9-8ea7-49d9-9032-580a98968c6e"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ConversationInteract"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_QButtonClick = m_Player.FindAction("QButtonClick", throwIfNotFound: true);
        m_Player_WButtonClick = m_Player.FindAction("WButtonClick", throwIfNotFound: true);
        m_Player_EButtonClick = m_Player.FindAction("EButtonClick", throwIfNotFound: true);
        m_Player_RButtonClick = m_Player.FindAction("RButtonClick", throwIfNotFound: true);
        m_Player_Back = m_Player.FindAction("Back", throwIfNotFound: true);
        m_Player_LeftMouseClick = m_Player.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Player_ArrowUpClick = m_Player.FindAction("ArrowUpClick", throwIfNotFound: true);
        m_Player_ArrowDownClick = m_Player.FindAction("ArrowDownClick", throwIfNotFound: true);
        m_Player_ArrowLeftClick = m_Player.FindAction("ArrowLeftClick", throwIfNotFound: true);
        m_Player_ArrowRightClick = m_Player.FindAction("ArrowRightClick", throwIfNotFound: true);
        m_Player_SelectClick = m_Player.FindAction("SelectClick", throwIfNotFound: true);
        m_Player_ZoomOut = m_Player.FindAction("ZoomOut", throwIfNotFound: true);
        m_Player_ZoomIn = m_Player.FindAction("ZoomIn", throwIfNotFound: true);
        // QuestUI
        m_QuestUI = asset.FindActionMap("QuestUI", throwIfNotFound: true);
        m_QuestUI_Test = m_QuestUI.FindAction("Test", throwIfNotFound: true);
        m_QuestUI_DisableQuestUI = m_QuestUI.FindAction("DisableQuestUI", throwIfNotFound: true);
        m_QuestUI_QuestMovement = m_QuestUI.FindAction("QuestMovement", throwIfNotFound: true);
        // InventoryUI
        m_InventoryUI = asset.FindActionMap("InventoryUI", throwIfNotFound: true);
        m_InventoryUI_Test = m_InventoryUI.FindAction("Test", throwIfNotFound: true);
        m_InventoryUI_DisableInventoryUI = m_InventoryUI.FindAction("DisableInventoryUI", throwIfNotFound: true);
        m_InventoryUI_InventoryMovement = m_InventoryUI.FindAction("InventoryMovement", throwIfNotFound: true);
        // Conversation
        m_Conversation = asset.FindActionMap("Conversation", throwIfNotFound: true);
        m_Conversation_ConversationInteract = m_Conversation.FindAction("ConversationInteract", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_QButtonClick;
    private readonly InputAction m_Player_WButtonClick;
    private readonly InputAction m_Player_EButtonClick;
    private readonly InputAction m_Player_RButtonClick;
    private readonly InputAction m_Player_Back;
    private readonly InputAction m_Player_LeftMouseClick;
    private readonly InputAction m_Player_ArrowUpClick;
    private readonly InputAction m_Player_ArrowDownClick;
    private readonly InputAction m_Player_ArrowLeftClick;
    private readonly InputAction m_Player_ArrowRightClick;
    private readonly InputAction m_Player_SelectClick;
    private readonly InputAction m_Player_ZoomOut;
    private readonly InputAction m_Player_ZoomIn;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @QButtonClick => m_Wrapper.m_Player_QButtonClick;
        public InputAction @WButtonClick => m_Wrapper.m_Player_WButtonClick;
        public InputAction @EButtonClick => m_Wrapper.m_Player_EButtonClick;
        public InputAction @RButtonClick => m_Wrapper.m_Player_RButtonClick;
        public InputAction @Back => m_Wrapper.m_Player_Back;
        public InputAction @LeftMouseClick => m_Wrapper.m_Player_LeftMouseClick;
        public InputAction @ArrowUpClick => m_Wrapper.m_Player_ArrowUpClick;
        public InputAction @ArrowDownClick => m_Wrapper.m_Player_ArrowDownClick;
        public InputAction @ArrowLeftClick => m_Wrapper.m_Player_ArrowLeftClick;
        public InputAction @ArrowRightClick => m_Wrapper.m_Player_ArrowRightClick;
        public InputAction @SelectClick => m_Wrapper.m_Player_SelectClick;
        public InputAction @ZoomOut => m_Wrapper.m_Player_ZoomOut;
        public InputAction @ZoomIn => m_Wrapper.m_Player_ZoomIn;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @QButtonClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQButtonClick;
                @QButtonClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQButtonClick;
                @QButtonClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQButtonClick;
                @WButtonClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWButtonClick;
                @WButtonClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWButtonClick;
                @WButtonClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWButtonClick;
                @EButtonClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEButtonClick;
                @EButtonClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEButtonClick;
                @EButtonClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEButtonClick;
                @RButtonClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRButtonClick;
                @RButtonClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRButtonClick;
                @RButtonClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRButtonClick;
                @Back.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBack;
                @LeftMouseClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @ArrowUpClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowUpClick;
                @ArrowUpClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowUpClick;
                @ArrowUpClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowUpClick;
                @ArrowDownClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowDownClick;
                @ArrowDownClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowDownClick;
                @ArrowDownClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowDownClick;
                @ArrowLeftClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowLeftClick;
                @ArrowLeftClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowLeftClick;
                @ArrowLeftClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowLeftClick;
                @ArrowRightClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowRightClick;
                @ArrowRightClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowRightClick;
                @ArrowRightClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnArrowRightClick;
                @SelectClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectClick;
                @SelectClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectClick;
                @SelectClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectClick;
                @ZoomOut.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomOut;
                @ZoomOut.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomOut;
                @ZoomOut.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomOut;
                @ZoomIn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomIn;
                @ZoomIn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomIn;
                @ZoomIn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomIn;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @QButtonClick.started += instance.OnQButtonClick;
                @QButtonClick.performed += instance.OnQButtonClick;
                @QButtonClick.canceled += instance.OnQButtonClick;
                @WButtonClick.started += instance.OnWButtonClick;
                @WButtonClick.performed += instance.OnWButtonClick;
                @WButtonClick.canceled += instance.OnWButtonClick;
                @EButtonClick.started += instance.OnEButtonClick;
                @EButtonClick.performed += instance.OnEButtonClick;
                @EButtonClick.canceled += instance.OnEButtonClick;
                @RButtonClick.started += instance.OnRButtonClick;
                @RButtonClick.performed += instance.OnRButtonClick;
                @RButtonClick.canceled += instance.OnRButtonClick;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @ArrowUpClick.started += instance.OnArrowUpClick;
                @ArrowUpClick.performed += instance.OnArrowUpClick;
                @ArrowUpClick.canceled += instance.OnArrowUpClick;
                @ArrowDownClick.started += instance.OnArrowDownClick;
                @ArrowDownClick.performed += instance.OnArrowDownClick;
                @ArrowDownClick.canceled += instance.OnArrowDownClick;
                @ArrowLeftClick.started += instance.OnArrowLeftClick;
                @ArrowLeftClick.performed += instance.OnArrowLeftClick;
                @ArrowLeftClick.canceled += instance.OnArrowLeftClick;
                @ArrowRightClick.started += instance.OnArrowRightClick;
                @ArrowRightClick.performed += instance.OnArrowRightClick;
                @ArrowRightClick.canceled += instance.OnArrowRightClick;
                @SelectClick.started += instance.OnSelectClick;
                @SelectClick.performed += instance.OnSelectClick;
                @SelectClick.canceled += instance.OnSelectClick;
                @ZoomOut.started += instance.OnZoomOut;
                @ZoomOut.performed += instance.OnZoomOut;
                @ZoomOut.canceled += instance.OnZoomOut;
                @ZoomIn.started += instance.OnZoomIn;
                @ZoomIn.performed += instance.OnZoomIn;
                @ZoomIn.canceled += instance.OnZoomIn;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // QuestUI
    private readonly InputActionMap m_QuestUI;
    private IQuestUIActions m_QuestUIActionsCallbackInterface;
    private readonly InputAction m_QuestUI_Test;
    private readonly InputAction m_QuestUI_DisableQuestUI;
    private readonly InputAction m_QuestUI_QuestMovement;
    public struct QuestUIActions
    {
        private @InputMaster m_Wrapper;
        public QuestUIActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Test => m_Wrapper.m_QuestUI_Test;
        public InputAction @DisableQuestUI => m_Wrapper.m_QuestUI_DisableQuestUI;
        public InputAction @QuestMovement => m_Wrapper.m_QuestUI_QuestMovement;
        public InputActionMap Get() { return m_Wrapper.m_QuestUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(QuestUIActions set) { return set.Get(); }
        public void SetCallbacks(IQuestUIActions instance)
        {
            if (m_Wrapper.m_QuestUIActionsCallbackInterface != null)
            {
                @Test.started -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnTest;
                @Test.performed -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnTest;
                @Test.canceled -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnTest;
                @DisableQuestUI.started -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnDisableQuestUI;
                @DisableQuestUI.performed -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnDisableQuestUI;
                @DisableQuestUI.canceled -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnDisableQuestUI;
                @QuestMovement.started -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnQuestMovement;
                @QuestMovement.performed -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnQuestMovement;
                @QuestMovement.canceled -= m_Wrapper.m_QuestUIActionsCallbackInterface.OnQuestMovement;
            }
            m_Wrapper.m_QuestUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Test.started += instance.OnTest;
                @Test.performed += instance.OnTest;
                @Test.canceled += instance.OnTest;
                @DisableQuestUI.started += instance.OnDisableQuestUI;
                @DisableQuestUI.performed += instance.OnDisableQuestUI;
                @DisableQuestUI.canceled += instance.OnDisableQuestUI;
                @QuestMovement.started += instance.OnQuestMovement;
                @QuestMovement.performed += instance.OnQuestMovement;
                @QuestMovement.canceled += instance.OnQuestMovement;
            }
        }
    }
    public QuestUIActions @QuestUI => new QuestUIActions(this);

    // InventoryUI
    private readonly InputActionMap m_InventoryUI;
    private IInventoryUIActions m_InventoryUIActionsCallbackInterface;
    private readonly InputAction m_InventoryUI_Test;
    private readonly InputAction m_InventoryUI_DisableInventoryUI;
    private readonly InputAction m_InventoryUI_InventoryMovement;
    public struct InventoryUIActions
    {
        private @InputMaster m_Wrapper;
        public InventoryUIActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Test => m_Wrapper.m_InventoryUI_Test;
        public InputAction @DisableInventoryUI => m_Wrapper.m_InventoryUI_DisableInventoryUI;
        public InputAction @InventoryMovement => m_Wrapper.m_InventoryUI_InventoryMovement;
        public InputActionMap Get() { return m_Wrapper.m_InventoryUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InventoryUIActions set) { return set.Get(); }
        public void SetCallbacks(IInventoryUIActions instance)
        {
            if (m_Wrapper.m_InventoryUIActionsCallbackInterface != null)
            {
                @Test.started -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnTest;
                @Test.performed -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnTest;
                @Test.canceled -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnTest;
                @DisableInventoryUI.started -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnDisableInventoryUI;
                @DisableInventoryUI.performed -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnDisableInventoryUI;
                @DisableInventoryUI.canceled -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnDisableInventoryUI;
                @InventoryMovement.started -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnInventoryMovement;
                @InventoryMovement.performed -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnInventoryMovement;
                @InventoryMovement.canceled -= m_Wrapper.m_InventoryUIActionsCallbackInterface.OnInventoryMovement;
            }
            m_Wrapper.m_InventoryUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Test.started += instance.OnTest;
                @Test.performed += instance.OnTest;
                @Test.canceled += instance.OnTest;
                @DisableInventoryUI.started += instance.OnDisableInventoryUI;
                @DisableInventoryUI.performed += instance.OnDisableInventoryUI;
                @DisableInventoryUI.canceled += instance.OnDisableInventoryUI;
                @InventoryMovement.started += instance.OnInventoryMovement;
                @InventoryMovement.performed += instance.OnInventoryMovement;
                @InventoryMovement.canceled += instance.OnInventoryMovement;
            }
        }
    }
    public InventoryUIActions @InventoryUI => new InventoryUIActions(this);

    // Conversation
    private readonly InputActionMap m_Conversation;
    private IConversationActions m_ConversationActionsCallbackInterface;
    private readonly InputAction m_Conversation_ConversationInteract;
    public struct ConversationActions
    {
        private @InputMaster m_Wrapper;
        public ConversationActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ConversationInteract => m_Wrapper.m_Conversation_ConversationInteract;
        public InputActionMap Get() { return m_Wrapper.m_Conversation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ConversationActions set) { return set.Get(); }
        public void SetCallbacks(IConversationActions instance)
        {
            if (m_Wrapper.m_ConversationActionsCallbackInterface != null)
            {
                @ConversationInteract.started -= m_Wrapper.m_ConversationActionsCallbackInterface.OnConversationInteract;
                @ConversationInteract.performed -= m_Wrapper.m_ConversationActionsCallbackInterface.OnConversationInteract;
                @ConversationInteract.canceled -= m_Wrapper.m_ConversationActionsCallbackInterface.OnConversationInteract;
            }
            m_Wrapper.m_ConversationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ConversationInteract.started += instance.OnConversationInteract;
                @ConversationInteract.performed += instance.OnConversationInteract;
                @ConversationInteract.canceled += instance.OnConversationInteract;
            }
        }
    }
    public ConversationActions @Conversation => new ConversationActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnQButtonClick(InputAction.CallbackContext context);
        void OnWButtonClick(InputAction.CallbackContext context);
        void OnEButtonClick(InputAction.CallbackContext context);
        void OnRButtonClick(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnArrowUpClick(InputAction.CallbackContext context);
        void OnArrowDownClick(InputAction.CallbackContext context);
        void OnArrowLeftClick(InputAction.CallbackContext context);
        void OnArrowRightClick(InputAction.CallbackContext context);
        void OnSelectClick(InputAction.CallbackContext context);
        void OnZoomOut(InputAction.CallbackContext context);
        void OnZoomIn(InputAction.CallbackContext context);
    }
    public interface IQuestUIActions
    {
        void OnTest(InputAction.CallbackContext context);
        void OnDisableQuestUI(InputAction.CallbackContext context);
        void OnQuestMovement(InputAction.CallbackContext context);
    }
    public interface IInventoryUIActions
    {
        void OnTest(InputAction.CallbackContext context);
        void OnDisableInventoryUI(InputAction.CallbackContext context);
        void OnInventoryMovement(InputAction.CallbackContext context);
    }
    public interface IConversationActions
    {
        void OnConversationInteract(InputAction.CallbackContext context);
    }
}
