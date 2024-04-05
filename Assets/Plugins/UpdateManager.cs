using System;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public event Action OnUpdate = null;

    private void Update()
    {
        OnUpdate?.Invoke();
    }
}
