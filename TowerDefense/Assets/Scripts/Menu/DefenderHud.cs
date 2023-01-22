using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenderHud : MonoBehaviour
{
    [field: SerializeField] private TextMeshProUGUI _wavesText;
    [field: SerializeField] private TextMeshProUGUI _playerHealthText;
    [field: SerializeField] private ToggleWithSpriteSwap _pauseToggle;
    [field: SerializeField] private Button _quitButton;

    public event Action<bool> PauseClicked;
    public event Action QuitGame;

    private void Awake()
    {
        _pauseToggle.ValueChanged += OnPauseClicked;
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    public void UpdatePlayerHealth(float currentHp, float maxHp)
    {
        _playerHealthText.text = $"{(int)(currentHp / maxHp * 100)}%";
    }
    public void UpdateScenarioWaves(int currentWave, int wavesCount)
    {
        _wavesText.text = $"{currentWave}/{wavesCount}";
    }
    private async void OnQuitButtonClicked()
    {
        OnPauseClicked(true);
        var isConfirmed = await AlertPopup.Instance.AwaitForDecision("Do you want to quit?");
        OnPauseClicked(false);
        if (isConfirmed)
            QuitGame?.Invoke();
    }

    private void OnPauseClicked(bool isPaused)
    {
        PauseClicked?.Invoke(isPaused);
    }

    private void OnDestroy()
    {
        _pauseToggle.ValueChanged -= OnPauseClicked;
    }
}
