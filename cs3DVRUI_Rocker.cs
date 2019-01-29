using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class cs3DVRUI_Rocker : MonoBehaviour
    , IColliderEventPressEnterHandler
    , IColliderEventPressExitHandler


{

    [Serializable]
    public class UnityEventRocker : UnityEvent<cs3DVRUI_Rocker> { }


    [Space(5, order = 0)]
    [Header("------------ Button Setting ", order = 1)]
    [Space(1, order = 2)]




    [Space(5, order = 0)]


    public Transform OnObject;
    public Transform OffObject;

    public float Action_Angle = 40f;

    [Space(10, order = 0)]
    [SerializeField]
    private ControllerButton m_activeButton = ControllerButton.Trigger;


    public UnityEventRocker Switch_On = new UnityEventRocker();
    public UnityEventRocker Switch_Off = new UnityEventRocker();



    
    private bool bOnOff = false;
    public bool IsSwitchOn() { return bOnOff; }


    private HashSet<ColliderButtonEventData> presses = new HashSet<ColliderButtonEventData>();


    public ControllerButton activeButton
    {
        get
        {
            return m_activeButton;
        }
        set
        {
            m_activeButton = value;
            // set all child MaterialChanger heighlightButton to value;
            var changers = ListPool<MaterialChanger>.Get();
            GetComponentsInChildren(changers);
            for (int i = changers.Count - 1; i >= 0; --i) { changers[i].heighlightButton = value; }
            ListPool<MaterialChanger>.Release(changers);
        }
    }



    // Use this for initialization
    void Start ()
    {
        bOnOff = false;

        SetSwitchOff();
        
    }

    public void SetSwitchOn()
    {

        //OnObject.Rotate(Vector3.zero);
        OnObject.localRotation = Quaternion.Euler(0, 0, 0);
        OffObject.localRotation = Quaternion.Euler(0, 0, Action_Angle);

        if (Switch_On != null)
            Switch_On.Invoke(this);

        bOnOff = true;
    }

    public void SetSwitchOff()
    {

        OnObject.localRotation = Quaternion.Euler(0, 0, Action_Angle * -1.0f);
        OffObject.localRotation = Quaternion.Euler(0, 0, 0);

        if (Switch_Off != null)
            Switch_Off.Invoke(this);

        bOnOff = false;
    }


#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        activeButton = m_activeButton;
    }
#endif




    public void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {


        if (eventData.IsViveButton(m_activeButton) && presses.Add(eventData) && presses.Count == 1)
        {

            for (int i = 0; i < eventData.pressEnteredObjects.Count; i++)
            {
                if (eventData.pressEnteredObjects[i] == OnObject.gameObject)
                {
                    if (IsSwitchOn())
                        break;
                    else
                    {
                        SetSwitchOn();
                        break;
                    }
                }
                else if (eventData.pressEnteredObjects[i] == OffObject.gameObject)
                {
                    if (!IsSwitchOn())
                        break;
                    else
                    {
                        SetSwitchOff();
                        break;
                    }
                }


            }

        }
    }

    public void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {

        if (presses.Remove(eventData) && presses.Count == 0)
        {


        }
    }




}
