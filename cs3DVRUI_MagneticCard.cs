using System;
using UnityEngine;
using UnityEngine.Events;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;
public class cs3DVRUI_MagneticCard : cs3DVRUI_DragBase
{



    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;


    public Vector3 Card_Pivot;
    

    private float yPosCheck = 0.0f;


    private Vector3 ButtonSavePos;

    private bool CheckGrip = false;
    private GameObject CheckGrip_Obj;

    public int KeyValue = 0;



    protected override void Start()
    {

        base.Start();
        ButtonSavePos = HandleObject.transform.localEulerAngles;

    }

    // Update is called once per frame
    void Update()
    {

    }


    protected override void OnDisable()
    {


        eventList.Clear();

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }


    public override void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {

        if (!eventData.IsViveButton(m_grabButton)) { return; }



        var casterPose = GetEventPose(eventData);
        var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

        if (alignPosition) { offsetPose.pos = Vector3.zero; }
        if (alignRotation) { offsetPose.rot = Quaternion.identity; }

        eventList.AddUniqueKey(eventData, offsetPose);



        if(!CheckGrip)
        {
            for (int i = 0; i < eventData.pressEnteredObjects.Count; i++)
            {
                if (eventData.pressEnteredObjects[i].tag == "MagneticCard")
                {
                    CheckGrip_Obj = eventData.pressEnteredObjects[i].gameObject;
                    CheckGrip = true;
                    break;
                }
            }

        }



        Debug.Log("cs3DVRUI_Base::OnColliderEventDragStart");
    }

    public override void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
    {

        if (eventData != grabbedEvent) { return; }


        var rigid = CheckGrip_Obj.transform.GetComponent<Rigidbody>();
        if (rigid != null)
        {
            // if rigidbody exists, follow eventData caster using physics
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;


            Vector3 TargetPos = new Vector3(targetPose.pos.x + Card_Pivot.x, targetPose.pos.y + Card_Pivot.y, targetPose.pos.z + Card_Pivot.z);

            // applying velocity
            var diffPos = TargetPos - rigid.position;
            if (Mathf.Approximately(diffPos.sqrMagnitude, 0f))
            {
                rigid.velocity = Vector3.zero;
            }
            else
            {
                rigid.velocity = diffPos / Mathf.Clamp(followingDuration, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION);
            }

            // applying angular velocity
            float angle;
            Vector3 axis;

            (targetPose.rot * Quaternion.Inverse(rigid.rotation)).ToAngleAxis(out angle, out axis);
            while (angle > 360f) { angle -= 360f; }

            if (Mathf.Approximately(angle, 0f) || float.IsNaN(axis.x))
            {
                rigid.angularVelocity = Vector3.zero;
            }
            else
            {
                angle *= Mathf.Deg2Rad / Mathf.Clamp(followingDuration, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION); // convert to radius speed
                if (overrideMaxAngularVelocity && rigid.maxAngularVelocity < angle) { rigid.maxAngularVelocity = angle; }
                rigid.angularVelocity = axis * angle;
            }
        }


        Debug.Log("cs3DVRUI_Base::OnColliderEventDragFixedUpdate");
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


            if(CheckGrip)
            {
           //     CheckGrip_Obj.transform.rotation = targetPose.rot;
                CheckGrip_Obj.transform.position = new Vector3(targetPose.pos.x + Card_Pivot.x, targetPose.pos.y + Card_Pivot.y, targetPose.pos.z + Card_Pivot.z);

                CheckGrip_Obj.transform.localRotation = Quaternion.Euler(0, 0, targetPose.rot.eulerAngles.x * -1.0f);
            }
            //else
            //{
            //    for (int i = 0; i < eventData.pressEnteredObjects.Count; i++)
            //    {
            //        if (eventData.pressEnteredObjects[i].tag == "MagneticCard")
            //        {
            //          //  eventData.pressEnteredObjects[i].transform.rotation = targetPose.rot;
            //            eventData.pressEnteredObjects[i].transform.position = new Vector3(targetPose.pos.x + Card_Pivot.x, targetPose.pos.y + Card_Pivot.y, targetPose.pos.z + Card_Pivot.z);

            //            eventData.pressEnteredObjects[i].transform.localRotation = Quaternion.Euler(0, 0, targetPose.rot.eulerAngles.x *-1.0f);
            //            CheckGrip_Obj = eventData.pressEnteredObjects[i].gameObject;
            //            CheckGrip = true;
            //            break;
            //        }
            //    }

            //}


        }
    }


    public override void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;


        eventList.Remove(eventData);


        if (released)
        {

            HandleObject.localRotation = Quaternion.Euler(ButtonSavePos.x, ButtonSavePos.y, ButtonSavePos.z);
            CheckGrip = false;

        }
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

        }

    }

}
