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
                    ""name"": ""ChangeCamera"",
                    ""type"": ""Button"",
                    ""id"": ""d454c09d-e692-4c80-8fe3-5b11af46c1a2"",
                    ""expectedControlType"": ""Button"",
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
                    ""name"": ""1D Axis"",
                    ""id"": ""8508e737-9d2e-4510-8e4e-5f0359e4639a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""a39d9472-463c-40f6-bbfb-253a8f29f092"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""2dd1994b-ae91-4ddd-bd5a-d290a5e01506"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""55a16eda-dd27-46e0-8c87-9cce05ecb1f5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""36ba602c-a73b-46a2-9ba2-c35add9e41ab"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""ca464173-ce59-42c6-880f-cfa63b731f66"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Car"",
                    ""action"": ""ChangeCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
        m_PC_ChangeCamera = m_PC.FindAction("ChangeCamera", throwIfNotFound: true);
        m_PC_Brake = m_PC.FindAction("Brake", throwIfNotFound: true);
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
    private readonly InputAction m_PC_ChangeCamera;
    private readonly InputAction m_PC_Brake;
    public struct PCActions
    {
        private @BasicPlayer m_Wrapper;
        public PCActions(@BasicPlayer wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PC_Move;
        public InputAction @ChangeCamera => m_Wrapper.m_PC_ChangeCamera;
        public InputAction @Brake => m_Wrapper.m_PC_Brake;
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
                @ChangeCamera.started -= m_Wrapper.m_PCActionsCallbackInterface.OnChangeCamera;
                @ChangeCamera.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnChangeCamera;
                @ChangeCamera.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnChangeCamera;
                @Brake.started -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
                @Brake.performed -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
                @Brake.canceled -= m_Wrapper.m_PCActionsCallbackInterface.OnBrake;
            }
            m_Wrapper.m_PCActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @ChangeCamera.started += instance.OnChangeCamera;
                @ChangeCamera.performed += instance.OnChangeCamera;
                @ChangeCamera.canceled += instance.OnChangeCamera;
                @Brake.started += instance.OnBrake;
                @Brake.performed += instance.OnBrake;
                @Brake.canceled += instance.OnBrake;
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
        void OnChangeCamera(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
    }
}
