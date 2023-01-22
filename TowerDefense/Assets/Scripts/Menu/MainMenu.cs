using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField] private Button _quickGameBtn;

    private void Start()
    {
        _quickGameBtn.onClick.AddListener(OnQuickGameBtnClicked);
    }

    private void OnQuickGameBtnClicked()
    {
        var loadingOperation = new Queue<ILoadingOperation>();
        loadingOperation.Enqueue(new GameLoadingOperation());
        LoadingScreen.Instance.Load(loadingOperation);
    }
}
