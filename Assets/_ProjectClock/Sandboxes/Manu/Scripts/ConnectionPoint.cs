using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionPoint : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    public static Action<ConnectionPoint> OnStartConnection;
    public static Action OnEndConnection;
    public static Action OnDropConnection;

    public Vector3 WorldPosition { get; private set; }

    public void Initiate(Vector3 worldPosition)
    {
        WorldPosition = worldPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnStartConnection?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer down");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
    }
}
