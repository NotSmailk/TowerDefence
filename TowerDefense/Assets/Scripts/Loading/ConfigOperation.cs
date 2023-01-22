using System;
using System.Threading.Tasks;

public class ConfigOperation : ILoadingOperation
{
    public string Description => "Configuration is loading...";

    public ConfigOperation(AppInfoContainer appInfoContainer)
    {
        // NONE
    }

    public async Task Load(Action<float> onProgress)
    {
        var loadTime = UnityEngine.Random.Range(1.5f, 2.5f);
        const int steps = 4;

        for (int i = 1; i <= steps; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(loadTime/steps));
            onProgress?.Invoke(i / loadTime);
        }

        onProgress?.Invoke(1f);
    }
}
