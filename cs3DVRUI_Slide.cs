using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;

public class cs3DVRUI_Slide : MonoBehaviour
    , IColliderEventDragStartHandler
    , IColliderEventDragFixedUpdateHandler
    , IColliderEventDragUpdateHandler
    , IColliderEventDragEndHandler
{



    [Serializable]
    public class UnityEventSlide : UnityEvent<cs3DVRUI_Slide> { }





    [Space(5, order = 0)]

    public const float MIN_FOLLOWING_DURATION = 0.02f;
    public const float DEFAULT_FOLLOWING_DURATION = 0.04f;
    public const float MAX_FOLLOWING_DURATION = 0.5f;


    [Space(5, order = 0)]
    [Header("------------ Button Setting ", order = 1)]
    [Space(1, order = 2)]

    public Transform ButtonObject;
    public float ButtonOnPos;
    public float ButtonOffPos;

    private Vector3 ButtonSavePos;


    private OrderedIndexedTable<ColliderButtonEventData, Pose> eventList = new OrderedIndexedTable<ColliderButtonEventData, Pose>();

    private bool alignPosition = true;
    private bool alignRotation = true;

    

    [Space(5, order = 0)]

    [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
    private float followingDuration = DEFAULT_FOLLOWING_DURATION;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;


    private bool overrideMaxAngularVelocity = true;


    [Space(5, order = 0)]

    [SerializeField]
    private ControllerButton m_grabButton = ControllerButton.Trigger;


    [Space(5, order = 0)]

    public UnityEventSlide Slide_On_Event = new UnityEventSlide();
    public UnityEventSlide Slide_Off_Event = new UnityEventSlide();


    private bool bSwitchOn = false;
    public bool IsSwitchOn() { return bSwitchOn; }


    public ControllerButton grabButton
    {
        get
        {
            return m_grabButton;
        }
        set
        {
            m_grabButton = value;
            // set all child MaterialChanger heighlightButton to value;
            var matChangers = ListPool<MaterialChanger>.Get();
            GetComponentsInChildren(matChangers);
            for (int i = matChangers.Count - 1; i >= 0; --i) { matChangers[i].heighlightButton = value; }
            ListPool<MaterialChanger>.Release(matChangers);
        }
    }

    public bool isGrabbed { get { return eventList.Count > 0; } }


    private float xPosCheck = 0.0f;

    public ColliderButtonEventData grabbedEvent { get { return isGrabbed ? eventList.GetLastKey() : null; } }

    private Pose GetEventPose(ColliderButtonEventData eventData)
    {
        var grabberTransform = eventData.eventCaster.transform;
        return new Pose(grabberTransform);
    }




#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        grabButton = m_grabButton;
    }
#endif
    protected virtual void Start()
    {
        alignPosition = true;
        alignRotation = true;

        grabButton = m_grabButton;

        ButtonSavePos = ButtonObject.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }



    protected virtual void OnDisable()
    {

        eventList.Clear();

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    public virtual void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {
        if (!eventData.IsViveButton(m_grabButton)) { return; }

        var casterPose = GetEventPose(eventData);
        var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

        if (alignPosition) { offsetPose.pos = Vector3.zero; }
        if (alignRotation) { offsetPose.rot = Quaternion.identity; }

        eventList.AddUniqueKey(eventData, offsetPose);

        //if (afterGrabbed != null)
        //{
        //    afterGrabbed.Invoke(this);
        //}
    }

    public virtual void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            // if rigidbody exists, follow eventData caster using physics
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;

            // applying velocity
            var diffPos = targetPose.pos - rigid.position;
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
    }

    public virtual void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        if (GetComponent<Rigidbody>() == null)
        {
            // if rigidbody doesn't exist, just move to eventData caster's pose
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;


            if (ButtonObject.localPosition.x >= ButtonOnPos && ButtonObject.localPosition.x <= ButtonOffPos)
                MoveStick(targetPose.pos.x);


            xPosCheck = targetPose.pos.x;

        }
    }

    private void MoveStick(float moveValue)
    {

        float Add_Pos;

        if ((StickMoveSensitivity/10000f) > Mathf.Abs(xPosCheck - moveValue))
            return;


        if (xPosCheck > moveValue)
        {

            Add_Pos = ButtonObject.localPosition.x - (StickfollowSpeed / 1000f);

            if (Add_Pos <= ButtonOnPos + 0.0002f)
                Add_Pos = ButtonOnPos + 0.0002f;

            ButtonObject.localPosition = new Vector3(Add_Pos, ButtonSavePos.y, ButtonSavePos.z);



            //손잡이를 잡고 내릴때 
            //                if (Add_Pos >= ButtonOffPos - 0.02f)
            if (ButtonOnPos <= ButtonObject.localPosition.x && ButtonOnPos + 0.01f >= ButtonObject.localPosition.x)
                SwitchOn();


        }
        else if (xPosCheck < moveValue)
        {

            Add_Pos = ButtonObject.localPosition.x + (StickfollowSpeed / 1000f);

            if (Add_Pos >= (ButtonOffPos - 0.0002f))
                Add_Pos = ButtonOffPos - 0.0002f;

            ButtonObject.localPosition = new Vector3(Add_Pos, ButtonSavePos.y, ButtonSavePos.z);

            if (ButtonOffPos >= ButtonObject.localPosition.x && ButtonOffPos - 0.01f <= ButtonObject.localPosition.x)
                SwitchOff();

        }
    }




    public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;


        eventList.Remove(eventData);


        if (released)
        {

            if (ButtonOffPos + 0.09f >= ButtonObject.localPosition.x && ButtonOffPos - 0.002f <= ButtonObject.localPosition.x)
            {
                ButtonObject.localPosition = new Vector3(ButtonOffPos - 0.0002f, ButtonSavePos.y, ButtonSavePos.z); 
                SwitchOff();
            }

            if(ButtonOnPos - 0.09f <= ButtonObject.localPosition.x && ButtonOnPos + 0.002f >= ButtonObject.localPosition.x)
            {
                ButtonObject.localPosition = new Vector3(ButtonOnPos + 0.0002f, ButtonSavePos.y, ButtonSavePos.z);
                SwitchOn();
            }
        }
    }



    public void SwitchOn()
    {
        if (IsSwitchOn())
            return;

        bSwitchOn = true;


        if (Slide_On_Event != null)
        {
            Slide_On_Event.Invoke(this);
        }
    }

    public void SwitchOff()
    {
        if (!IsSwitchOn())
            return;

        bSwitchOn = false;

        if (Slide_Off_Event != null)
        {
            Slide_Off_Event.Invoke(this);
        }
    }
}
