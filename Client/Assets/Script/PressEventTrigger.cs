
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class PressEventTrigger : MonoBehaviour, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    //public event CallBack PointDown, PointUp;
    private int index = 0, num = 15;
    public bool IsOn = false, IsOn_Old = false;
    void OnDisable()
    {
        if (IsOn)
        {
            //Debug.Log("kkkkkkkkkkk:");
            IsOn = false;
            IsOn_Old = false;
            //if (PointUp != null)
            //{
            //    PointUp();
            //}
        }
    }
    public void ClearEvent()
    {
        //PointUp = null;
        //PointDown = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsOn = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsOn = true;
    }

    void Update()
    {
        if (IsOn)
        {
            if (index == 0)
            {
                //if (PointDown != null)
                //{
                //    PointDown();
                //}
            }
            else
            {
                index++;
                if (index >= num)
                {
                    index = 0;
                }
            }
        }
        else
        {
            if (IsOn_Old)
            {
                //if (PointUp != null)
                //{
                //    PointUp();
                //}
            }
        }
        IsOn_Old = IsOn;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsOn = false;
    }
}
