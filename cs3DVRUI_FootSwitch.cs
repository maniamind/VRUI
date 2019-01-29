using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_FootSwitch : cs3DVRUI_DragBase
{



    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;




    public UnityEventController HandleGrabRelease = new UnityEventController();
    public UnityEventController Event_Connet = new UnityEventController();
    public UnityEventController Event_Disconnet = new UnityEventController();


    private float yPosCheck = 0.0f;


    private Vector3 ButtonSavePos;

    private bool CheckConnet = false;
    private Text PercentDis;

    private float CurProgress = 0.0f;
    public float GetProgress() { return CurProgress; }


    protected override void Start()
    {

        base.Start();
        PercentDis = HandleObject.transform.Find("TitleCanvas").GetChild(0).GetComponent<Text>();
        PercentDis.text = (0 * 100.0f).ToString("N0") + "%";
        CurProgress = 0.0f;
        ButtonSavePos = HandleObject.transform.localEulerAngles;

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


            if (!CheckConnet)
                MoveStick(targetPose.pos.y);

            yPosCheck = targetPose.pos.y;




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
            HandleObject.localRotation = Quaternion.Euler(ButtonSavePos.x, ButtonSavePos.y, ButtonSavePos.z);
            PercentDis.text = (0 * 100.0f).ToString("N0") + "%";
            CurProgress = 0;
        }
    }

    public void TriggerEnter()
    {
        CheckConnet = true;
        Event_Connet.Invoke(this);
    }



    public void RestoreHandle()
    {
        CheckConnet = false;
        Event_Disconnet.Invoke(this);
    }

    public void MoveStick(float moveValue)
    {



        if ((StickMoveSensitivity / 1000f) > Mathf.Abs(yPosCheck - moveValue))
            return;


        //float Add_Angle = (HandleObject.localEulerAngles.x + HandlefollowSpeed);
        if (yPosCheck > moveValue)
        {
            float Add_Angle = HandleObject.localEulerAngles.x + (HandlefollowSpeed / 4.0f);
            HandleObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

            PercentDis.text = ((Add_Angle/6.0f) * 100.0f).ToString("N0") + "%";
            CurProgress = (Add_Angle / 6.0f) * 100.0f;
        }

    }



}
