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
            ""name"": ""FMPlayer"",
            ""id"": ""0ccd06b9-f131-470c-be50-ef1863cc3e26"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""750d8564-d89e-4c1d-b869-d067d9006619"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sneak"",
                    ""type"": ""Button"",
                    ""id"": ""c0bb7937-33d3-412f-b434-76c14eeba3d1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""2dcc2bfe-8366-4ed3-982e-b6c6e1b40f74"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rush"",
                    ""type"": ""Button"",
                    ""id"": ""77c52d3d-b8c2-4653-ab31-e40a7b9ee1fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EnableQuestUI"",
                    ""type"": ""Button"",
                    ""id"": ""2c929326-04e1-4033-a0ce-3c1758da7867"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EnableInventoryUI"",
                    ""type"": ""Button"",
                    ""id"": ""bd26f55a-b710-4483-9573-d99630bbe1d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""arrowKeys"",
                    ""id"": ""3b9bee84-1b33-40c1-a54b-e0a94674e0b0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""89328f61-590e-4d63-9eb9-2cd848414a50"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""724401c0-ece9-4c9f-a764-1c46c0f561bf"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0b084813-8bdd-4545-8cb0-dbb3b3ab32fc"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e8c8c638-8560-494e-8b51-93fc25c3b279"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b3cc2aea-0aed-4472-90b7-5c13e4a48562"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Sneak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a38c0af5-247e-4890-92dc-a3dc8dfc5aa5"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""106f92e9-5f65-41a9-b770-a1c75913ce9a"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Rush"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77ed70b5-3822-475e-aa22-c11a3184a72a"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""EnableQuestUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d695f77-e46c-4433-a52e-d80c097119d8"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""EnableInventoryUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
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
                }
            ]
        },
        {
            ""name"": ""MovePoint"",
            ""id"": ""a6bbacf3-6b2d-4c79-937a-bbf4affd75a6"",
            ""actions"": [
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""ffc9d8cd-2db4-4dba-94ee-76bc6d4f1ccf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowUpClick"",
                    ""type"": ""Button"",
                    ""id"": ""9862e19d-05df-47be-be51-1ab349958c99"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowDownClick"",
                    ""type"": ""Button"",
                    ""id"": ""e5b88a02-9e34-4ff4-b96b-55956eefbb14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""4b9a24f8-5516-4072-99fc-f367e8b77572"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArrowRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""96ad6dff-3e53-4371-a4a8-ddf1f72a3fdb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectClick"",
                    ""type"": ""Button"",
                    ""id"": ""2ed3cc7b-5140-41cb-a825-9fb5013009a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""eefbdc5d-670f-414b-946d-53c989c66716"",
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
                    ""id"": ""eb15c8c4-7262-4594-aafa-154057343b2f"",
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
                    ""id"": ""635e1f40-1496-4ab3-b494-08c316d7c924"",
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
                    ""id"": ""852f1e61-3032-45e6-9f00-c7783d7b5b76"",
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
                    ""id"": ""3981e1bf-2caf-485d-9801-c994937c468c"",
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
                    ""id"": ""2d97653c-91fe-42d4-8935-3edf3a3325c2"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SelectClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""6acef09f-51be-42af-98ff-1d9ba56da734"",
            ""actions"": [
                {
                    ""name"": ""ZoomIn"",
                    ""type"": ""Button"",
                    ""id"": ""90144bf6-53bc-418a-85f0-bfc6c835e492"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomOut"",
                    ""type"": ""Button"",
                    ""id"": ""ab881bbc-f354-44a3-a1e0-5aaf6e0f7f9e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c78ce989-565c-4888-8940-6adbd6b272c3"",
                    ""path"": ""<Keyboard>/comma"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b4df95a-9356-4f8d-8b34-09bac762c800"",
                    ""path"": ""<Keyboard>/period"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ZoomOut"",
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
        // FMPlayer
        m_FMPlayer = asset.FindActionMap("FMPlayer", throwIfNotFound: true);
        m_FMPlayer_Movement = m_FMPlayer.FindAction("Movement", throwIfNotFound: true);
        m_FMPlayer_Sneak = m_FMPlayer.FindAction("Sneak", throwIfNotFound: true);
        m_FMPlayer_Interact = m_FMPlayer.FindAction("Interact", throwIfNotFound: true);
        m_FMPlayer_Rush = m_FMPlayer.FindAction("Rush", throwIfNotFound: true);
        m_FMPlayer_EnableQuestUI = m_FMPlayer.FindAction("EnableQuestUI", throwIfNotFound: true);
        m_FMPlayer_EnableInventoryUI = m_FMPlayer.FindAction("EnableInventoryUI", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_QButtonClick = m_Player.FindAction("QButtonClick", throwIfNotFound: true);
        m_Player_WButtonClick = m_Player.FindAction("WButtonClick", throwIfNotFound: true);
        m_Player_EButtonClick = m_Player.FindAction("EButtonClick", throwIfNotFound: true);
        m_Player_RButtonClick = m_Player.FindAction("RButtonClick", throwIfNotFound: true);
        m_Player_Back = m_Player.FindAction("Back", throwIfNotFound: true);
        // MovePoint
        m_MovePoint = asset.FindActionMap("MovePoint", throwIfNotFound: true);
        m_MovePoint_LeftMouseClick = m_MovePoint.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_MovePoint_ArrowUpClick = m_MovePoint.FindAction("ArrowUpClick", throwIfNotFound: true);
        m_MovePoint_ArrowDownClick = m_MovePoint.FindAction("ArrowDownClick", throwIfNotFound: true);
        m_MovePoint_ArrowLeftClick = m_MovePoint.FindAction("ArrowLeftClick", throwIfNotFound: true);
        m_MovePoint_ArrowRightClick = m_MovePoint.FindAction("ArrowRightClick", throwIfNotFound: true);
        m_MovePoint_SelectClick = m_MovePoint.FindAction("SelectClick", throwIfNotFound: true);
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_ZoomIn = m_Camera.FindAction("ZoomIn", throwIfNotFound: true);
        m_Camera_ZoomOut = m_Camera.FindAction("ZoomOut", throwIfNotFound: true);
        // QuestUI
        m_QuestUI = asset.FindActionMap("QuestUI", throwIfNotFound: true);
        m_QuestUI_Test = m_QuestUI.FindAction("Test", throwIfNotFound: true);
        m_QuestUI_DisableQuestUI = m_QuestUI.FindAction("DisableQuestUI", throwIfNotFound: true);
        // InventoryUI
        m_InventoryUI = asset.FindActionMap("InventoryUI", throwIfNotFound: true);
        m_InventoryUI_Test = m_InventoryUI.FindAction("Test", throwIfNotFound: true);
        m_InventoryUI_DisableInventoryUI = m_InventoryUI.FindAction("DisableInventoryUI", throwIfNotFound: true);
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

    // FMPlayer
    private readonly InputActionMap m_FMPlayer;
    private IFMPlayerActions m_FMPlayerActionsCallbackInterface;
    private readonly InputAction m_FMPlayer_Movement;
    private readonly InputAction m_FMPlayer_Sneak;
    private readonly InputAction m_FMPlayer_Interact;
    private readonly InputAction m_FMPlayer_Rush;
    private readonly InputAction m_FMPlayer_EnableQuestUI;
    private readonly InputAction m_FMPlayer_EnableInventoryUI;
    public struct FMPlayerActions
    {
        private @InputMaster m_Wrapper;
        public FMPlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_FMPlayer_Movement;
        public InputAction @Sneak => m_Wrapper.m_FMPlayer_Sneak;
        public InputAction @Interact => m_Wrapper.m_FMPlayer_Interact;
        public InputAction @Rush => m_Wrapper.m_FMPlayer_Rush;
        public InputAction @EnableQuestUI => m_Wrapper.m_FMPlayer_EnableQuestUI;
        public InputAction @EnableInventoryUI => m_Wrapper.m_FMPlayer_EnableInventoryUI;
        public InputActionMap Get() { return m_Wrapper.m_FMPlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FMPlayerActions set) { return set.Get(); }
        public void SetCallbacks(IFMPlayerActions instance)
        {
            if (m_Wrapper.m_FMPlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnMovement;
                @Sneak.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnSneak;
                @Sneak.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnSneak;
                @Sneak.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnSneak;
                @Interact.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnInteract;
                @Rush.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnRush;
                @Rush.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnRush;
                @Rush.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnRush;
                @EnableQuestUI.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableQuestUI;
                @EnableQuestUI.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableQuestUI;
                @EnableQuestUI.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableQuestUI;
                @EnableInventoryUI.started -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableInventoryUI;
                @EnableInventoryUI.performed -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableInventoryUI;
                @EnableInventoryUI.canceled -= m_Wrapper.m_FMPlayerActionsCallbackInterface.OnEnableInventoryUI;
            }
            m_Wrapper.m_FMPlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Sneak.started += instance.OnSneak;
                @Sneak.performed += instance.OnSneak;
                @Sneak.canceled += instance.OnSneak;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Rush.started += instance.OnRush;
                @Rush.performed += instance.OnRush;
                @Rush.canceled += instance.OnRush;
                @EnableQuestUI.started += instance.OnEnableQuestUI;
                @EnableQuestUI.performed += instance.OnEnableQuestUI;
                @EnableQuestUI.canceled += instance.OnEnableQuestUI;
                @EnableInventoryUI.started += instance.OnEnableInventoryUI;
                @EnableInventoryUI.performed += instance.OnEnableInventoryUI;
                @EnableInventoryUI.canceled += instance.OnEnableInventoryUI;
            }
        }
    }
    public FMPlayerActions @FMPlayer => new FMPlayerActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_QButtonClick;
    private readonly InputAction m_Player_WButtonClick;
    private readonly InputAction m_Player_EButtonClick;
    private readonly InputAction m_Player_RButtonClick;
    private readonly InputAction m_Player_Back;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @QButtonClick => m_Wrapper.m_Player_QButtonClick;
        public InputAction @WButtonClick => m_Wrapper.m_Player_WButtonClick;
        public InputAction @EButtonClick => m_Wrapper.m_Player_EButtonClick;
        public InputAction @RButtonClick => m_Wrapper.m_Player_RButtonClick;
        public InputAction @Back => m_Wrapper.m_Player_Back;
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
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // MovePoint
    private readonly InputActionMap m_MovePoint;
    private IMovePointActions m_MovePointActionsCallbackInterface;
    private readonly InputAction m_MovePoint_LeftMouseClick;
    private readonly InputAction m_MovePoint_ArrowUpClick;
    private readonly InputAction m_MovePoint_ArrowDownClick;
    private readonly InputAction m_MovePoint_ArrowLeftClick;
    private readonly InputAction m_MovePoint_ArrowRightClick;
    private readonly InputAction m_MovePoint_SelectClick;
    public struct MovePointActions
    {
        private @InputMaster m_Wrapper;
        public MovePointActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMouseClick => m_Wrapper.m_MovePoint_LeftMouseClick;
        public InputAction @ArrowUpClick => m_Wrapper.m_MovePoint_ArrowUpClick;
        public InputAction @ArrowDownClick => m_Wrapper.m_MovePoint_ArrowDownClick;
        public InputAction @ArrowLeftClick => m_Wrapper.m_MovePoint_ArrowLeftClick;
        public InputAction @ArrowRightClick => m_Wrapper.m_MovePoint_ArrowRightClick;
        public InputAction @SelectClick => m_Wrapper.m_MovePoint_SelectClick;
        public InputActionMap Get() { return m_Wrapper.m_MovePoint; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovePointActions set) { return set.Get(); }
        public void SetCallbacks(IMovePointActions instance)
        {
            if (m_Wrapper.m_MovePointActionsCallbackInterface != null)
            {
                @LeftMouseClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnLeftMouseClick;
                @ArrowUpClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowUpClick;
                @ArrowUpClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowUpClick;
                @ArrowUpClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowUpClick;
                @ArrowDownClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowDownClick;
                @ArrowDownClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowDownClick;
                @ArrowDownClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowDownClick;
                @ArrowLeftClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowLeftClick;
                @ArrowLeftClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowLeftClick;
                @ArrowLeftClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowLeftClick;
                @ArrowRightClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowRightClick;
                @ArrowRightClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowRightClick;
                @ArrowRightClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnArrowRightClick;
                @SelectClick.started -= m_Wrapper.m_MovePointActionsCallbackInterface.OnSelectClick;
                @SelectClick.performed -= m_Wrapper.m_MovePointActionsCallbackInterface.OnSelectClick;
                @SelectClick.canceled -= m_Wrapper.m_MovePointActionsCallbackInterface.OnSelectClick;
            }
            m_Wrapper.m_MovePointActionsCallbackInterface = instance;
            if (instance != null)
            {
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
            }
        }
    }
    public MovePointActions @MovePoint => new MovePointActions(this);

    // Camera
    private readonly InputActionMap m_Camera;
    private ICameraActions m_CameraActionsCallbackInterface;
    private readonly InputAction m_Camera_ZoomIn;
    private readonly InputAction m_Camera_ZoomOut;
    public struct CameraActions
    {
        private @InputMaster m_Wrapper;
        public CameraActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ZoomIn => m_Wrapper.m_Camera_ZoomIn;
        public InputAction @ZoomOut => m_Wrapper.m_Camera_ZoomOut;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
            {
                @ZoomIn.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
                @ZoomIn.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
                @ZoomIn.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
                @ZoomOut.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
                @ZoomOut.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
                @ZoomOut.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
            }
            m_Wrapper.m_CameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ZoomIn.started += instance.OnZoomIn;
                @ZoomIn.performed += instance.OnZoomIn;
                @ZoomIn.canceled += instance.OnZoomIn;
                @ZoomOut.started += instance.OnZoomOut;
                @ZoomOut.performed += instance.OnZoomOut;
                @ZoomOut.canceled += instance.OnZoomOut;
            }
        }
    }
    public CameraActions @Camera => new CameraActions(this);

    // QuestUI
    private readonly InputActionMap m_QuestUI;
    private IQuestUIActions m_QuestUIActionsCallbackInterface;
    private readonly InputAction m_QuestUI_Test;
    private readonly InputAction m_QuestUI_DisableQuestUI;
    public struct QuestUIActions
    {
        private @InputMaster m_Wrapper;
        public QuestUIActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Test => m_Wrapper.m_QuestUI_Test;
        public InputAction @DisableQuestUI => m_Wrapper.m_QuestUI_DisableQuestUI;
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
            }
        }
    }
    public QuestUIActions @QuestUI => new QuestUIActions(this);

    // InventoryUI
    private readonly InputActionMap m_InventoryUI;
    private IInventoryUIActions m_InventoryUIActionsCallbackInterface;
    private readonly InputAction m_InventoryUI_Test;
    private readonly InputAction m_InventoryUI_DisableInventoryUI;
    public struct InventoryUIActions
    {
        private @InputMaster m_Wrapper;
        public InventoryUIActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Test => m_Wrapper.m_InventoryUI_Test;
        public InputAction @DisableInventoryUI => m_Wrapper.m_InventoryUI_DisableInventoryUI;
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
    public interface IFMPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnSneak(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnRush(InputAction.CallbackContext context);
        void OnEnableQuestUI(InputAction.CallbackContext context);
        void OnEnableInventoryUI(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnQButtonClick(InputAction.CallbackContext context);
        void OnWButtonClick(InputAction.CallbackContext context);
        void OnEButtonClick(InputAction.CallbackContext context);
        void OnRButtonClick(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
    public interface IMovePointActions
    {
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnArrowUpClick(InputAction.CallbackContext context);
        void OnArrowDownClick(InputAction.CallbackContext context);
        void OnArrowLeftClick(InputAction.CallbackContext context);
        void OnArrowRightClick(InputAction.CallbackContext context);
        void OnSelectClick(InputAction.CallbackContext context);
    }
    public interface ICameraActions
    {
        void OnZoomIn(InputAction.CallbackContext context);
        void OnZoomOut(InputAction.CallbackContext context);
    }
    public interface IQuestUIActions
    {
        void OnTest(InputAction.CallbackContext context);
        void OnDisableQuestUI(InputAction.CallbackContext context);
    }
    public interface IInventoryUIActions
    {
        void OnTest(InputAction.CallbackContext context);
        void OnDisableInventoryUI(InputAction.CallbackContext context);
    }
    public interface IConversationActions
    {
        void OnConversationInteract(InputAction.CallbackContext context);
    }
}
