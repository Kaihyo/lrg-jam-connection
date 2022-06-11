using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionUi : MonoBehaviour
{
    [SerializeField] private List<TimeUser> _timeUsers;
    [SerializeField] private RectTransform _connectionPointPrefab;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Start()
    {
        foreach(TimeUser user in _timeUsers)
        {
            Vector3 pointScreenPos =  _cam.WorldToViewportPoint(user.ConnectionPoint.position);
            pointScreenPos.x *= _cam.pixelRect.width;
            pointScreenPos.y *= _cam.pixelRect.height;
            RectTransform img = Instantiate(_connectionPointPrefab, this.transform);
            img.position = pointScreenPos;

            var connectionPoint = img.GetComponent<ConnectionPoint>();
            connectionPoint.Initiate(user.ConnectionPoint.position);
        }
    }
}
