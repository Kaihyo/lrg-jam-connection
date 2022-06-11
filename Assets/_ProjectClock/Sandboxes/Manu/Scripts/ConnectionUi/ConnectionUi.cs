using System.Collections.Generic;
using UnityEngine;

public class ConnectionUi : MonoBehaviour
{
    [SerializeField] private List<TimeUser> _timeUsers;
    [SerializeField] private ConnectionPoint _connectionPointPrefab;
    [SerializeField] private ConnectionLine _connectionLinePrefab;

    private Camera _cam;
    private ConnectionPoint _mouseConnectPoint;
    private ConnectionLine _activeConnectionLine;

    private void Awake()
    {
        _cam = Camera.main;
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
        _mouseConnectPoint = Instantiate(_connectionPointPrefab, transform);
        _mouseConnectPoint.DisableRaycasts();
        _mouseConnectPoint.SetType(ConnectionPointType.MousePosition);
        _mouseConnectPoint.gameObject.SetActive(false);

        foreach(TimeUser user in _timeUsers)
        {
            Vector3 pointScreenPos =  _cam.WorldToViewportPoint(user.ConnectionPoint.position);
            pointScreenPos.x *= _cam.pixelRect.width;
            pointScreenPos.y *= _cam.pixelRect.height;
            ConnectionPoint connectionPoint = Instantiate(_connectionPointPrefab, transform);
            connectionPoint.RectTransform.position = pointScreenPos;
            connectionPoint.SetTarget(user);
            connectionPoint.SetType(ConnectionPointType.TimeUser);
        }
    }

    private void Update()
    {
        if(_mouseConnectPoint.gameObject.activeSelf)
        {
            _mouseConnectPoint.RectTransform.position = Input.mousePosition;
        }
    }

    private void OnStartConnection(ConnectionPoint startPoint)
    {
        _mouseConnectPoint.gameObject.SetActive(true);
        _mouseConnectPoint.RectTransform.position = Input.mousePosition;

        _activeConnectionLine = Instantiate(_connectionLinePrefab, transform);
        _activeConnectionLine.SetPoints(startPoint, _mouseConnectPoint);
    }

    private void OnEndConnection(ConnectionPoint endPoint)
    {
        if(_activeConnectionLine == null)
        {
            return;
        }

        if(endPoint.Type == _activeConnectionLine.StartingPointType)
        {
            return;
        }

        _activeConnectionLine.UpdatePoint(_mouseConnectPoint, endPoint);
        _activeConnectionLine.ApplyConnection();

        _mouseConnectPoint.Disconnect();
        _mouseConnectPoint.gameObject.SetActive(false);

        _activeConnectionLine = null;

    }

    private void OnRemovingConnection(ConnectionPoint point)
    {
        _activeConnectionLine = point.ConnectionLine;
        _activeConnectionLine.ApplyDisconnection();

        _mouseConnectPoint.gameObject.SetActive(true);
        _mouseConnectPoint.RectTransform.position = Input.mousePosition;
        _activeConnectionLine.UpdatePoint(point, _mouseConnectPoint);

        point.Disconnect();
    }

    private void OnCancelConnection()
    {
        if(_activeConnectionLine == null)
        {
            return;
        }

        _mouseConnectPoint.gameObject.SetActive(false);
        _activeConnectionLine.Erase();
    }
}
