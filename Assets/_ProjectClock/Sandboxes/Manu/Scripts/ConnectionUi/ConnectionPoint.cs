using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ConnectionPointType
{
    MousePosition,
    TimeSource,
    TimeUser,
}

[RequireComponent(typeof(CanvasGroup))]
public class ConnectionPoint : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private ConnectionPointType _type;

    private CanvasGroup _canvasGroup;
    private TimeUser _target;

    public static Action<ConnectionPoint> OnStartConnection;
    public static Action<ConnectionPoint> OnEndConnection;
    public static Action<ConnectionPoint> OnRemovingConnection;
    public static Action OnCancelConnection;

    public ConnectionPointType Type => _type;
    public RectTransform RectTransform { get; private set; }
    public ConnectionLine ConnectionLine { get; private set; }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        RectTransform = GetComponent<RectTransform>();
        ConnectionLine = null;
    }

    public void DisableRaycasts()
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetTarget(TimeUser target)
    {
        _target = target;
    }

    public void SetType(ConnectionPointType type)
    {
        _type = type;
    }

    public void Connect(ConnectionLine connectionLine)
    {
        if (Type == ConnectionPointType.TimeUser)
        {
            _target.SetConnected(true);
        }
        ConnectionLine = connectionLine;
    }

    public void Disconnect()
    {
        if (Type == ConnectionPointType.TimeUser)
        {
            _target.SetConnected(false);
        }
        ConnectionLine = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ConnectionLine != null)
        {
            OnRemovingConnection?.Invoke(this);
        }
        else
        {
            OnStartConnection?.Invoke(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnCancelConnection?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Drop");
            OnEndConnection?.Invoke(this);
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
