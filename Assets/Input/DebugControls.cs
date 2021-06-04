// GENERATED AUTOMATICALLY FROM 'Assets/Input/DebugControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DebugControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DebugControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DebugControls"",
    ""maps"": [
        {
            ""name"": ""Debug"",
            ""id"": ""696e061d-4c41-4797-a456-b6d5a8f8a99b"",
            ""actions"": [
                {
                    ""name"": ""DisplayConsole"",
                    ""type"": ""Button"",
                    ""id"": ""c69185ed-d8c4-49b3-a37a-07b047b7c95c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Clear"",
                    ""type"": ""Button"",
                    ""id"": ""daa2e9b3-8a04-460b-91ab-899872e7459d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""34a816d1-89d1-46a5-a2f2-f85303688136"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayConsole"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0acad899-18b9-44e0-a016-fed3ca1518d7"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Clear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_DisplayConsole = m_Debug.FindAction("DisplayConsole", throwIfNotFound: true);
        m_Debug_Clear = m_Debug.FindAction("Clear", throwIfNotFound: true);
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

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_DisplayConsole;
    private readonly InputAction m_Debug_Clear;
    public struct DebugActions
    {
        private @DebugControls m_Wrapper;
        public DebugActions(@DebugControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @DisplayConsole => m_Wrapper.m_Debug_DisplayConsole;
        public InputAction @Clear => m_Wrapper.m_Debug_Clear;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @DisplayConsole.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnDisplayConsole;
                @DisplayConsole.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnDisplayConsole;
                @DisplayConsole.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnDisplayConsole;
                @Clear.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnClear;
                @Clear.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnClear;
                @Clear.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnClear;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DisplayConsole.started += instance.OnDisplayConsole;
                @DisplayConsole.performed += instance.OnDisplayConsole;
                @DisplayConsole.canceled += instance.OnDisplayConsole;
                @Clear.started += instance.OnClear;
                @Clear.performed += instance.OnClear;
                @Clear.canceled += instance.OnClear;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);
    public interface IDebugActions
    {
        void OnDisplayConsole(InputAction.CallbackContext context);
        void OnClear(InputAction.CallbackContext context);
    }
}
