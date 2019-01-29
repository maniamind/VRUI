using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.Events;

using VR3DUI_BaseButton;

public class cs3DVRUI_ToggleOnOff : cs3DVRUI_ButtonBase
{

    private GameObject DisplayLight_on;
    private GameObject DisplayLight_off;
    bool bToggleOnOff = false;
    public bool GetOnOff() { return bToggleOnOff; }

    public UnityEventController Button_Down = new UnityEventController();
    public UnityEventController Button_Up = new UnityEventController();
    public UnityEventController Button_Press = new UnityEventController();

    public UnityEventController Toggle_On_Event = new UnityEventController();
    public UnityEventController Toggle_Off_Event = new UnityEventController();



    // Use this for initialization
    protected override void Start()
    {

        OriginPosition = buttonObject.localPosition;
        bToggleOnOff = false;
        DisplayLight_on = buttonObject.transform.Find("display_on").gameObject;
        DisplayLight_off = buttonObject.transform.Find("display_off").gameObject;
        DisplayLight_on.SetActive(false);
        DisplayLight_off.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void OnColliderEventPressUp(ColliderButtonEventData eventData)
    {
        PowerOn = false;

        if (Button_Up != null)
        {
            Button_Up.Invoke(this);

            if (!bToggleOnOff)
            {
                bToggleOnOff = true;
                DisplayLight_on.SetActive(true);
                DisplayLight_off.SetActive(false);
                Toggle_On_Event.Invoke(this);
            }
            else
            {
                bToggleOnOff = false;
                DisplayLight_on.SetActive(false);
                DisplayLight_off.SetActive(true);
                Toggle_Off_Event.Invoke(this);
            }
        }
    }

    public override void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
        if (eventData.IsViveButton(m_activeButton) && pressingEvents.Add(eventData) && pressingEvents.Count == 1)
        {

            //if (PowerOn)
            //    return;

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
