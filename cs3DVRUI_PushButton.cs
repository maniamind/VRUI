using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.Events;

using VR3DUI_BaseButton;
public class cs3DVRUI_PushButton : cs3DVRUI_ButtonBase
{

    
    public UnityEventController Button_Down = new UnityEventController();
    public UnityEventController Button_Up = new UnityEventController();
    public UnityEventController Button_Press = new UnityEventController();



    // Use this for initialization
    protected override void Start()
    {

        OriginPosition = buttonObject.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if (PowerOn)
            Button_Press.Invoke(this);
    }


    public override void OnColliderEventPressUp(ColliderButtonEventData eventData)
    {
        PowerOn = false;

        if (Button_Up != null)
        {
            Button_Up.Invoke(this);
        }
    }

    public override void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
        if (eventData.IsViveButton(m_activeButton) && pressingEvents.Add(eventData) && pressingEvents.Count == 1)
        {

            if (PowerOn)
                return;

            buttonObject.transform.localPosition = OriginPosition + buttonDownDisplacement;

            if (Button_Down != null)
            {
                Button_Down.Invoke(this);
            }

            PowerOn = true;

        }
    }

    public override void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {
        if (pressingEvents.Count == 0)
        {
            PowerOn = false;

        }

        if (pressingEvents.Remove(eventData) && pressingEvents.Count == 0)
        {
            buttonObject.transform.localPosition = OriginPosition;

        }


    }
}
