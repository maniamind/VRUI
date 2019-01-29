
using System;
using UnityEngine;
using UnityEngine.Events;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_HandWheel : cs3DVRUI_DragBase
{


    private float MinActionAngle;
    private float CenterActionAngle;
    private float rotateDegree_Save;


    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]

    //GrabRelease

    public UnityEventController Event_LeftTurn = new UnityEventController();
    public UnityEventController Event_RightTurn = new UnityEventController();
    public UnityEventController HandleGrabRelease = new UnityEventController();



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



    public override void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {
        if (!eventData.IsViveButton(m_grabButton)) { return; }



        var casterPose = GetEventPose(eventData);
        var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

        if (alignPosition) { offsetPose.pos = Vector3.zero; }
        if (alignRotation) { offsetPose.rot = Quaternion.identity; }

        eventList.AddUniqueKey(eventData, offsetPose);


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

            //yPosCheck = targetPose.pos.y;
            //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
            Vector3 mPosition = targetPose.pos; //마우스 좌표 저장
            Vector3 oPosition = HandleObject.transform.position; //게임 오브젝트 좌표 저장

            //카메라가 앞면에서 뒤로 보고 있기 때문에, 마우스 position의 z축 정보에
            //게임 오브젝트와 카메라와의 z축의 차이를 입력시켜줘야 합니다.
            mPosition.z = oPosition.z - targetPose.pos.z;

            Vector3 target = mPosition;// Camera.main.ScreenToWorldPoint(mPosition);

            float dy = target.y - oPosition.y;
            float dx = target.x - oPosition.x;

            float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            rotateDegree = (rotateDegree - 120f);

            if (rotateDegree > rotateDegree_Save)
            {
                Debug.Log("Left");
                Event_LeftTurn.Invoke(this);
            }
            else
            {
                Debug.Log("Right");
                Event_RightTurn.Invoke(this);
            }





  

            

            //if (rotateDegree > HandleObject.transform.localRotation.eulerAngles.z)
            //{
            //    Debug.Log("Left");
            //    Event_LeftTurn.Invoke(this);
            //}
            //else
            //{
            //    Debug.Log("Right");
            //    Event_RightTurn.Invoke(this);
            //}



            HandleObject.transform.localRotation = Quaternion.Euler(0f, 0f, rotateDegree);



            rotateDegree_Save = rotateDegree;




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



}
