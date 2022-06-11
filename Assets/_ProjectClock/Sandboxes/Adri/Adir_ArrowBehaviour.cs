using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Adir_ArrowBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform arrow;
    public Adir_ClockTicking clockTicking;
    
    public void OnDrag(PointerEventData eventData)
    {
        clockTicking.ticking = false;
        Vector3 difference = new Vector3(eventData.position.x, eventData.position.y, 0.0f) - arrow.position;
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, rotation_z - 90.0f);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        clockTicking.ticking = true;
    }
}
