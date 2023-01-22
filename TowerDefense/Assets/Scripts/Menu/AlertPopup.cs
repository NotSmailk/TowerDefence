using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class AlertPopup : MonoBehaviour
{
    [field: SerializeField] private TextMeshProUGUI _text;
    [field: SerializeField] private Button _okButton;
    [field: SerializeField] private Button _cancelButton;
    [field: SerializeField] private Button _closeButton;

    private TaskCompletionSource<bool> _taskCompletion;
    private Canvas _canvas;

    public static AlertPopup Instance { get; private set; }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        Instance = this;
        _okButton?.onClick.AddListener(OnAccept);
        _cancelButton?.onClick.AddListener(OnCancelled);
        _closeButton?.onClick.AddListener(OnCancelled);

        DontDestroyOnLoad(this);
    }

    public async Task<bool> AwaitForDecision(string text)
    {
        _text.text = text;
        _canvas.enabled = true;
        _taskCompletion = new TaskCompletionSource<bool>();
        var result = await _taskCompletion.Task;
        _canvas.enabled = false;
        return result;
    }

    private void OnAccept()
    {
        _taskCompletion.SetResult(true);
    }

    private void OnCancelled()
    {
        _taskCompletion.SetResult(false);
    }
}
