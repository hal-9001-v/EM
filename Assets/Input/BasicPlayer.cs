// GENERATED AUTOMATICALLY FROM 'Assets/Input/BasicPlayer.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BasicPlayer : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @BasicPlayer()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BasicPlayer"",
    ""maps"": [
        {
            ""name"": ""PC"",
            ""id"": ""2fc1ccd9-2db8-48c1-8ee7-3a76fc52658b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""31250c71-d03a-489c-bf5c-283cacf876a0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Button"",
                    ""id"": ""770f1879-3cfe-4890-ab88-a7b9ae647940"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""e2b55f53-d436-4acd-b0e5-0b65b41f05f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Send_Msg"",
                    ""type"": ""Button"",
                    ""id"": ""d235c8a1-781f-4a0c-9fe6-6e2088091821"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""Button"",
                    ""id"": ""e237f2a0-5b67-4c75-bf4c-7f6ff5520735"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""dd8b9391-5223-48ac-912c-ded5b068d8fe"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a804b583-e96b-4925-ac12-345c37e26e91"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d7952d7a-6864-4015-abc1-a9b9b0674e7e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a5c33ec8-fc33-4179-950e-1aaf8aba158c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6d5c14b6-2eb8-417e-b23f-58fb9b14e75c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""34a5c73a-c3fd-48f5-9b44-a8096f307ed9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""80fdcd2f-a4df-4d47-8344-54b7fb78ee7f"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e672f96b-60d9-4ef4-9367-f1ecbc122f84"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""412e5cbb-aaa4-4989-bb26-0799b0fb5da0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d40ca57e-4179-4466-bbd3-bf363b868673"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""15a45654-ac9d-4414-a9ec-209ffb0f6f23"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bea45604-2f4c-46f1-b4d5-807d9ae9ad89"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e368439-5bf4-415c-b642-b12c20636151"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Send_Msg"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf73365a-9578-43b9-bdbd-0482fca64e75"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Car"",
            ""bindingGroup"": ""Car"",
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
        // PC
        m_PC = asset.FindActionMap("PC", throwIfNotFound: true);
        m_PC_Move = m_PC.FindAction("Move", throwIfNotFound: true);
        m_PC_Brake = m_PC.FindAction("Brake", throwIfNotFound: true);
        m_PC_Pause = m_PC.FindAction("Pause", throwIfNotFound: true);
        m_PC_Send_Msg = m_PC.FindAction("Send_Msg", throwIfNotFound: true);
        m_PC_Camera = m_PC.FindAction("Camera", throwIfNotFound: true);
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

    // PC
    private readonly InputActionMap m_PC;
    private IPCActions m_PCActionsCallbackInterface;
    private readonly InputAction m_PC_Move;
    private readonly InputAction m_PC_Brake;
    private readonly InputAction m_PC_Pause;
    private readonly InputAction m_PC_Send_Msg;
    private readonly InputAction m_PC_Camera;
    public struct PCActions
    {
        private @BasicPlayer m_Wrapper;
        public PCActions(@BasicPlayer wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PC_Move;
        public InputAction @Brake => m_Wrapper.m_PC_Brake;
        public InputAction @Pause => m_Wrapper.m_PC_Pause;
        public InputAction @Send_Msg => m_Wrapper.m_PC_Send_Msg;
        public InputAction @Camera => m_Wrapper.m_PC_Camera;
        public InputActionMap Get() { return m_Wrapper.m_PC; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PCActions set) { return set.Get(); }
        public void SetCallbacks(IPCActions instance)
        {
            if (m_Wrapper.m_PCActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PCActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnMove;
                @Brake.started -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
                @Brake.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
                @Brake.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
                @Pause.started -= m_Wrapper.m_PCActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnPause;
                @Send_Msg.started -= m_Wrapper.m_PCActionsCallbackInterface.OnSend_Msg;
                @Send_Msg.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnSend_Msg;
                @Send_Msg.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnSend_Msg;
                @Camera.started -= m_Wrapper.m_PCActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnCamera;
            }
            m_Wrapper.m_PCActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Brake.started += instance.OnBrake;
                @Brake.performed += instance.OnBrake;
                @Brake.canceled += instance.OnBrake;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Send_Msg.started += instance.OnSend_Msg;
                @Send_Msg.performed += instance.OnSend_Msg;
                @Send_Msg.canceled += instance.OnSend_Msg;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
            }
        }
    }
    public PCActions @PC => new PCActions(this);
    private int m_CarSchemeIndex = -1;
    public InputControlScheme CarScheme
    {
        get
        {
            if (m_CarSchemeIndex == -1) m_CarSchemeIndex = asset.FindControlSchemeIndex("Car");
            return asset.controlSchemes[m_CarSchemeIndex];
        }
    }
    public interface IPCActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnSend_Msg(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
    }
}
