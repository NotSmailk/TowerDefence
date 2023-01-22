using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [field: SerializeField] private Canvas _canvas;
    [field: SerializeField] private Slider _progressFill;
    [field: SerializeField] private TextMeshProUGUI _loadingInfo;
    [field: SerializeField] private float _barSpeed;

    private float _targetProgress;
    private bool _isProgress;

    public static LoadingScreen Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public async void Load(Queue<ILoadingOperation> loadingOperations)
    {
        _canvas.enabled = true;
        StartCoroutine(UpdateProgressBar());

        foreach (var operation in loadingOperations)
        {
            ResetFill();
            _loadingInfo.text = operation.Description;

            await operation.Load(OnProgress);
            await WaitForBarFill();
        }
    }

    private void ResetFill()
    {
        _progressFill.value = 0f;
        _targetProgress = 0f;
    }

    private void OnProgress(float progress)
    {
        _targetProgress = progress;
    }

    private async Task WaitForBarFill()
    {
        while (_progressFill.value < _targetProgress)
        {
            await Task.Delay(1);
        }

        await Task.Delay(TimeSpan.FromSeconds(0.15f));
    }

    private  IEnumerator UpdateProgressBar()
    {
        while (_canvas.enabled)
        {
            if (_progressFill.value < _targetProgress)
                _progressFill.value += Time.deltaTime * _barSpeed;

            yield return null;
        }
    }
}
