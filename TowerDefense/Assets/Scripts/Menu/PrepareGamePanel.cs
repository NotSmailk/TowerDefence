using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PrepareGamePanel : MonoBehaviour
{
    [field: SerializeField] private GameObject[] _colors;
    [field: SerializeField] private GameObject _go;
    [field: SerializeField] private Vector3 _defaultScale;
    [field: SerializeField] private Vector3 _bigScale;

    public async Task<bool> Prepare(float seconds, CancellationToken cancellationToken)
    {
        ResetThis();
        gameObject.SetActive(true);

        var elementsCount = _colors.Length + 1;
        var unitTime = seconds / elementsCount;

        for (int i = 0; i < _colors.Length; i++)
        {
            if (i > 0)
                _colors[i - 1].transform.localScale = _defaultScale;

            _colors[i].transform.localScale = _bigScale;
            await Task.Delay(TimeSpan.FromSeconds(unitTime), cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }

        foreach (var c in _colors)
        {
            c.gameObject.SetActive(false);
        }

        _go.SetActive(false);

        await Task.Delay(TimeSpan.FromSeconds(unitTime), cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        if (gameObject)
            gameObject.SetActive(false);

        return true;
    }

    private void ResetThis()
    {
        foreach (var c in _colors)
        {
            c.transform.localScale = _defaultScale;
            c.gameObject.SetActive(true);
        }

        _go.SetActive(true);
    }
}
