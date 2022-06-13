using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [Header("UI")]
    [SerializeField] private Sprite _disconnectedSprite;
    [SerializeField] private Sprite _connectedSprite;
    [SerializeField] private Color _disconnectedColor, _connectedColor;

    private CanvasGroup _canvasGroup;
    private Image _image;
    

    public static Action<ConnectionPoint> OnStartConnection;
    public static Action<ConnectionPoint> OnEndConnection;
    public static Action<ConnectionPoint> OnRemovingConnection;
    public static Action OnCancelConnection;

    public ConnectionPointType Type => _type;
    public RectTransform RectTransform { get; private set; }
    public ConnectionLine ConnectLine { get; private set; }
    public TimeUser Target { get; private set; }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
        RectTransform = transform as RectTransform;

        ConnectLine = null;
    }

    public void SetTarget(TimeUser target, bool forceVisibility = false)
    {
        Target = target;

        _canvasGroup.alpha = (!forceVisibility && target == null) ? 0 : 1;
        _canvasGroup.blocksRaycasts = (target == null) ? false : true;
    }

    public void SetType(ConnectionPointType type)
    {
        _type = type;
    }

    public void Connect(ConnectionLine connectionLine)
    {
        if (Type == ConnectionPointType.TimeUser)
        {
            Target.SetConnected(true);
        }
        ConnectLine = connectionLine;
        _image.sprite = _connectedSprite;
        _image.color = _connectedColor;
    }

    public void Disconnect()
    {
        if (Type == ConnectionPointType.TimeUser)
        {
            Target.SetConnected(false);
        }
        ConnectLine = null;
        _image.sprite = _disconnectedSprite;
        _image.color = _disconnectedColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ConnectLine != null)
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
