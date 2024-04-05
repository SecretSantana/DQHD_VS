using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventHandler : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public event Action HandlePointerEnter;
    public event Action HandlePointerExit;

    private bool _isPointerOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Enter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Exit();
    }

    private void OnDestroy()
    {
        if (_isPointerOver)
        {
            Exit();
        }
    }

    private void OnDisable()
    {
        if (_isPointerOver)
        {
            Exit();
        }
    }

    private void Enter()
    {
        _isPointerOver = true;
        HandlePointerEnter?.Invoke();
    }

    private void Exit()
    {
        _isPointerOver = false;
        HandlePointerExit?.Invoke();
    }
}
