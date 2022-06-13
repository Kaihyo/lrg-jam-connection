using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ConnectionUi : MonoBehaviour
{
    [SerializeField] private List<TimeUser> _timeUsers;
    [SerializeField] private ConnectionPoint _connectPointPrefab;
    [SerializeField] private RectTransform _connectPointsHolder;
    [SerializeField] private ConnectionLine _connectLinePrefab;
    [SerializeField] private RectTransform _connectLinesHolder;

    private Camera _cam;
    private CanvasGroup _canvasGroup;

    private bool _isDisplayed;
    private Queue<ConnectionPoint> _connectPointsPool;
    private Queue<ConnectionLine> _connectLinesPool;
    private List<ConnectionPoint> _displayedConnectPoints;
    private List<ConnectionPoint> _connectedPoints;
    private ConnectionPoint _mouseConnectPoint;
    private ConnectionLine _activeConnectLine;

    public static event System.Action<bool> OnToggleDisplay;

    private void Awake()
    {
        _cam = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        ConnectionPoint.OnStartConnection += OnStartConnection;
        ConnectionPoint.OnEndConnection += OnEndConnection;
        ConnectionPoint.OnCancelConnection += OnCancelConnection;
        ConnectionPoint.OnRemovingConnection += OnRemovingConnection;
    }

    private void OnDisable()
    {
        ConnectionPoint.OnStartConnection -= OnStartConnection;
        ConnectionPoint.OnEndConnection -= OnEndConnection;
        ConnectionPoint.OnCancelConnection -= OnCancelConnection;
        ConnectionPoint.OnRemovingConnection -= OnRemovingConnection;
    }

    private void Start()
    {
        _isDisplayed = false;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;

        _mouseConnectPoint = Instantiate(_connectPointPrefab, transform);
        _mouseConnectPoint.SetTarget(null, true);
        _mouseConnectPoint.SetType(ConnectionPointType.MousePosition);
        _mouseConnectPoint.gameObject.SetActive(false);

        SetupConnectLines();
        SetupConnectPoints();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleDisplay();
        }

        if(_mouseConnectPoint.gameObject.activeSelf)
        {
            _mouseConnectPoint.RectTransform.position = Input.mousePosition;
        }
    }

    private void ToggleDisplay()
    {
        _isDisplayed = !_isDisplayed;
        OnToggleDisplay?.Invoke(_isDisplayed);

        if (_isDisplayed)
        {
            UpdateConnectPoints();
        }
        else
        {
            ClearConnectPoints();
        }

        _canvasGroup.alpha = (_canvasGroup.alpha == 0) ? 1 : 0;
        _canvasGroup.blocksRaycasts = !_canvasGroup.blocksRaycasts;
    }

    private void SetupConnectLines()
    {
        _connectLinesPool = new Queue<ConnectionLine>();

        for (int i = 0; i < 15; i++)
        {
            ConnectionLine connectLine = Instantiate(_connectLinePrefab, _connectLinesHolder.transform);
            connectLine.Erase();
            _connectLinesPool.Enqueue(connectLine);
        }
    }

    private void SetupConnectPoints()
    {
        _connectPointsPool = new Queue<ConnectionPoint>();
        _displayedConnectPoints = new List<ConnectionPoint>();
        _connectedPoints = new List<ConnectionPoint>();

        for (int i = 0; i < 15; i++)
        {
            ConnectionPoint connectionPoint = Instantiate(_connectPointPrefab, _connectPointsHolder.transform);
            connectionPoint.SetTarget(null);
            connectionPoint.SetType(ConnectionPointType.TimeUser);

            _connectPointsPool.Enqueue(connectionPoint);
        }
    }

    private void UpdateConnectPoints()
    {
        foreach(ConnectionPoint connectPoint in _connectedPoints)
        {
            Vector3 pointScreenPos = GetTimeUserScreenPos(connectPoint.Target);
            connectPoint.RectTransform.position = pointScreenPos;
        }

        foreach(TimeUser user in _timeUsers)
        {
            Vector3 pointScreenPos = GetTimeUserScreenPos(user);

            ConnectionPoint connectPoint = _connectPointsPool.Dequeue();
            connectPoint.RectTransform.position = pointScreenPos;
            connectPoint.SetTarget(user);

            _displayedConnectPoints.Add(connectPoint);
        }
    }

    private Vector3 GetTimeUserScreenPos(TimeUser timeUser)
    {
        Vector3 pointScreenPos = _cam.WorldToViewportPoint(timeUser.ConnectionAnchor.position);
        pointScreenPos.x *= _cam.pixelRect.width;
        pointScreenPos.y *= _cam.pixelRect.height;

        return pointScreenPos;
    }

    private void ClearConnectPoints()
    {
        for(int i = _displayedConnectPoints.Count - 1; i >= 0; i--)
        {
            ConnectionPoint connectPoint = _displayedConnectPoints[i];
            connectPoint.SetTarget(null);
            _connectPointsPool.Enqueue(connectPoint);
            _displayedConnectPoints.RemoveAt(i);
        }
    }

    private void OnStartConnection(ConnectionPoint startPoint)
    {
        _mouseConnectPoint.RectTransform.position = Input.mousePosition;
        _mouseConnectPoint.gameObject.SetActive(true);

        _activeConnectLine = _connectLinesPool.Dequeue();
        _activeConnectLine.SetPoints(startPoint, _mouseConnectPoint);
        _activeConnectLine.gameObject.SetActive(true);
    }

    private void OnEndConnection(ConnectionPoint endPoint)
    {
        if(_activeConnectLine == null)
        {
            return;
        }

        if(endPoint.Type == _activeConnectLine.StartingPointType || endPoint.ConnectLine != null)
        {
            return;
        }

        _activeConnectLine.UpdatePoint(_mouseConnectPoint, endPoint);
        _activeConnectLine.ApplyConnection();

        AddConnectedPoints(_activeConnectLine.PointA);
        AddConnectedPoints(_activeConnectLine.PointB);

        _mouseConnectPoint.Disconnect();
        _mouseConnectPoint.gameObject.SetActive(false);

        _activeConnectLine = null;
    }

    private void OnRemovingConnection(ConnectionPoint point)
    {
        _activeConnectLine = point.ConnectLine;

        RemoveConnectedPoints(_activeConnectLine.PointA);
        RemoveConnectedPoints(_activeConnectLine.PointB);

        _activeConnectLine.ApplyDisconnection();

        _mouseConnectPoint.gameObject.SetActive(true);
        _mouseConnectPoint.RectTransform.position = Input.mousePosition;
        _activeConnectLine.UpdatePoint(point, _mouseConnectPoint);

        point.Disconnect();
    }

    private void OnCancelConnection()
    {
        if(_activeConnectLine == null)
        {
            return;
        }

        _mouseConnectPoint.gameObject.SetActive(false);
        _connectLinesPool.Enqueue(_activeConnectLine);
        _activeConnectLine.Erase();
    }

    private void AddConnectedPoints(ConnectionPoint connectPoint)
    {
        if (connectPoint.Type != ConnectionPointType.TimeUser)
        {
            return;
        }

        _connectedPoints.Add(connectPoint);
        _timeUsers.Remove(connectPoint.Target);
        _displayedConnectPoints.Remove(connectPoint);
    }

    private void RemoveConnectedPoints(ConnectionPoint connectPoint)
    {
        if(connectPoint.Type != ConnectionPointType.TimeUser)
        {
            return;
        }

        _displayedConnectPoints.Add(connectPoint);
        _timeUsers.Add(connectPoint.Target);
        _connectedPoints.Remove(connectPoint);
    }
}
