using System;
using UnityEngine;
using UnityEngine.Events;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_Micro : cs3DVRUI_DragBase
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



    protected override void Start()
    {

        base.Start();
        ButtonSavePos = HandleObject.transform.localEulerAngles;

    }

    // Update is called once per frame
    void Update () {
		
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


            if(!CheckConnet)
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
            float Add_Angle = HandleObject.localEulerAngles.x + (HandlefollowSpeed /4.0f);
            HandleObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

        }

    }


}
