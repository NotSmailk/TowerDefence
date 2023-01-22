using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class MenuLoadingOperation : ILoadingOperation
{
    public string Description => "Main menu is loading...";

    public MenuLoadingOperation()
    {
        // NONE
    }

    public async Task Load(Action<float> onProgress)
    {
        onProgress?.Invoke(0.5f);

        var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.MAIN_MENU, LoadSceneMode.Additive);
        while (!loadOp.isDone)
        {
            await Task.Delay(1);
        }

        onProgress?.Invoke(1f);
    }
}
