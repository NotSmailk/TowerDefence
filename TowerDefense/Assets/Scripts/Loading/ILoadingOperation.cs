using System;
using System.Threading.Tasks;

public interface ILoadingOperation
{
    public string Description { get; }

    Task Load(Action<float> onProgress);
}
