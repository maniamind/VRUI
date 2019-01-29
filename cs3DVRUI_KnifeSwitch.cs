using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_KnifeSwitch : cs3DVRUI_DragBase
{



    [Space(5, order = 0)]
    [Header("--- Controller Setting", order = 1)]
    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;

    [Space(5, order = 0)]




    public bool ElectricPuger_Left = true;
    public bool ElectricPuger_Right = true;


    private GameObject Puger_Left;
    private GameObject Puger_Right;

    private bool CheckConnet = false;
    public bool GetConnet() { return CheckConnet; } 

    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]
    public UnityEventController HandleGrabRelease = new UnityEventController();
    public UnityEventController Event_Connet = new UnityEventController();
    public UnityEventController Event_Disconnet = new UnityEventController();


    private float xPosCheck = 0.0f;



    private Vector3 ButtonSaveAngles;


    protected override void Start()
    {

        base.Start();
        ButtonSaveAngles = HandleObject.transform.localEulerAngles;

        Puger_Left = transform.Find("Puger_Left").gameObject;
        Puger_Right = transform.Find("Puger_Left").gameObject;

        Puger_Left.SetActive(ElectricPuger_Left);
        Puger_Right.SetActive(ElectricPuger_Right);


    }



    // Update is called once per frame
    void Update()
    {


    }





    protected override void OnDisable()
    {

        if (IsGrabbed && HandleGrabRelease != null)
        {
            HandleGrabRelease.Invoke(this);
        }

        eventList.Clear();

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }




    public override void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        if (GetComponent<Rigidbody>() == null)
        {
            // if rigidbody doesn't exist, just move to eventData caster's pose
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;



            MoveStick(targetPose.pos.x);


            xPosCheck = targetPose.pos.x;




        }
    }




    public override void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;

        if (released && HandleGrabRelease != null)
        {
            HandleGrabRelease.Invoke(this);
        }

        eventList.Remove(eventData);


        if (released)
        {

        }


    }





    public void MoveStick(float moveValue)
    {


        if ((StickMoveSensitivity / 1000f) > Mathf.Abs(xPosCheck - moveValue))
            return;



        if (xPosCheck > moveValue)
        {
            float Add_Angle = HandleObject.localEulerAngles.x - (HandlefollowSpeed / 3.0f);

            if (HandleObject.localEulerAngles.x > 0)
                Add_Angle = Add_Angle - 360;

            if (Add_Angle <= -90)
                Add_Angle = -90;

            HandleObject.localRotation = Quaternion.Euler(Add_Angle, ButtonSaveAngles.y, ButtonSaveAngles.z);
        }
        else if (xPosCheck < moveValue)
        {
            float Add_Angle = HandleObject.localEulerAngles.x + (HandlefollowSpeed / 3.0f);

            if(HandleObject.localEulerAngles.x > 0)
                Add_Angle = Add_Angle - 360;


            if (Add_Angle >= 0)
                Add_Angle = 0;

           HandleObject.localRotation = Quaternion.Euler(Add_Angle, ButtonSaveAngles.y, ButtonSaveAngles.z);
        }

    }


    public void TriggerEnter()
    {

        if(ElectricPuger_Left && ElectricPuger_Right)
        {
            CheckConnet = true;
            Event_Connet.Invoke(this);

        }

    }



    public void RestoreHandle()
    {
        CheckConnet = false;
        Event_Disconnet.Invoke(this);
    }


}
