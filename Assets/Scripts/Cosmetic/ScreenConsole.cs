using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenConsole : MonoBehaviour
{

    [Header("References")]
    [SerializeField] TextMeshProUGUI _textMesh;

    [Header("Settings")]
    [SerializeField] bool _showConsole = false;

    DebugControls controls;


    private void Start()
    {
        controls = new DebugControls();

        controls.Debug.DisplayConsole.performed += ctx =>
        {

            if (_showConsole)
            {
                _showConsole = false;
                HideConsole();
            }
            else
            {
                _showConsole = true;
                ShowConsole();
            }

        };

        controls.Debug.Clear.performed += ctx =>
        {
            Clear();
        };

        controls.Enable();

        _showConsole = true;
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }



    void ShowConsole()
    {
        if (_textMesh != null)
        {
            _textMesh.enabled = true;
        }
    }

    void HideConsole()
    {
        if (_textMesh != null)
        {
            _textMesh.enabled = false;
        }
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (_textMesh != null && _showConsole)
        {

            if (type == LogType.Error)
                _textMesh.text += stackTrace;

            _textMesh.text = _textMesh.text + message + "\n \n";

        }
    }


    [ContextMenu("Clear")]
    void Clear()
    {
        _textMesh.text = "";

    }


}
