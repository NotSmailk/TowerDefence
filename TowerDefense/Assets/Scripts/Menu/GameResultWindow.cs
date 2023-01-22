using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class GameResultWindow : MonoBehaviour
{
    [field: SerializeField] private GameResultIntroAnimation _introAnimation;
    [field: SerializeField] private Button _restartButton;
    [field: SerializeField] private Button _quitButton;

    private Canvas _canvas;
    private Action _onRestart;
    private Action _onQuit;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _restartButton.onClick.AddListener(OnRestartClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    public async void Show(GameResultType result, Action onRestart, Action onQuit)
    {
        _onRestart = onRestart;
        _onQuit = onQuit;

        _restartButton.interactable = false;
        _quitButton.interactable = false;

        _canvas.enabled = true;
        await _introAnimation.Play(result);

        _restartButton.interactable = true;
        _quitButton.interactable = true;
    }

    private void OnRestartClicked()
    {
        _canvas.enabled = false;
        _onRestart?.Invoke();
    }

    private void OnQuitClicked()
    {
        _canvas.enabled = false;
        _onQuit?.Invoke();
    }
}

public enum GameResultType
{
    Victory,
    Defeat
}
