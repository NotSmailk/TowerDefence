using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameLoadingOperation : ILoadingOperation
{
    public string Description => "Game is loading...";

    public async Task Load(Action<float> onProgress)
    {
        onProgress?.Invoke(0.5f);
        var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.QUICK_GAME, LoadSceneMode.Single);
        while (!loadOp.isDone)
        {
            await Task.Delay(1);
        }

        Scene scene = SceneManager.GetSceneByName(Constants.Scenes.QUICK_GAME);
        SceneManager.SetActiveScene(scene);
        onProgress?.Invoke(1f);
    }
}
