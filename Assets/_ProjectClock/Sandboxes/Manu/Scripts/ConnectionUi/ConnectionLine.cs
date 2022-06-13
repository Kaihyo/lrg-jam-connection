using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class ConnectionLine : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Image _lineImage;
    
    public ConnectionPoint PointA { get; private set; }
    public ConnectionPoint PointB { get; private set; }
    public ConnectionPointType StartingPointType { get; private set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _lineImage = GetComponent<Image>();
    }

    private void Update()
    {
        DrawLine();
    }

    public void SetPoints(ConnectionPoint a, ConnectionPoint b)
    {
        PointA = a;
        PointB = b;

        StartingPointType = a.Type;

        ConnectionPoint temp;
        if (PointA.RectTransform.position.x > PointB.RectTransform.position.x)
        {
            temp = PointA;
            PointA = PointB;
            PointB = temp;
        }

        _lineImage.enabled = true;
    }

    public void UpdatePoint(ConnectionPoint oldPoint, ConnectionPoint newPoint)
    {
        if(oldPoint == PointA)
        {
            PointA = newPoint;
            StartingPointType = PointB.Type;
        }
        else if(oldPoint == PointB)
        {
            PointB = newPoint;
            StartingPointType = PointA.Type;
        }

        ConnectionPoint temp;
        if (PointA.RectTransform.position.x > PointB.RectTransform.position.x)
        {
            temp = PointA;
            PointA = PointB;
            PointB = temp;
        }
    }

    public void ApplyConnection()
    {
        PointA.Connect(this);
        PointB.Connect(this);
    }

    public void ApplyDisconnection()
    {
        PointA.Disconnect();
        PointB.Disconnect();
    }

    public void Erase()
    {
        PointA = null;
        PointB = null;

        _lineImage.enabled = false;
    }

    public void UpdateDisplay()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        if (PointA == null || PointB == null)
        {
            return;
        }

        if(!PointA.gameObject.activeSelf || !PointB.gameObject.activeSelf)
        {
            return;
        }

        _rectTransform.position = (PointA.RectTransform.position + PointB.RectTransform.position) / 2f;
        Vector3 d = PointB.RectTransform.anchoredPosition - PointA.RectTransform.anchoredPosition;
        _rectTransform.sizeDelta = new Vector3(d.magnitude, 5f);
        _rectTransform.rotation = Quaternion.Euler(
            new Vector3(
                0,
                0,
                180 * Mathf.Atan(d.y / d.x) / Mathf.PI
         ));
    }
}
