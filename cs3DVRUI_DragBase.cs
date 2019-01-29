using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;


namespace VR3DUI_BaseDrag
{
    public class cs3DVRUI_DragBase : MonoBehaviour
        , IColliderEventPressDownHandler
        , IColliderEventDragStartHandler
        , IColliderEventDragFixedUpdateHandler
        , IColliderEventDragUpdateHandler
        , IColliderEventDragEndHandler
        , IColliderEventPressExitHandler

    {

        [Space(5, order = 0)]
        [Header("--- Controller Property", order = 1)]        
        [Space(5, order = 0)]
        public Transform HandleObject;
        [Space(15, order = 0)]

        public const float MIN_FOLLOWING_DURATION = 0.02f;
        public const float DEFAULT_FOLLOWING_DURATION = 0.04f;
        public const float MAX_FOLLOWING_DURATION = 0.5f;

        [Space(20, order = 0)]

        [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
        protected float followingDuration = DEFAULT_FOLLOWING_DURATION;

        //핸덜 민감도
        [Space(10, order = 0)]
        [Range(1, 10)]
        public int HandleMoveSensitivity = 2;


        //헨덜의 이동 속도...
        [Space(10, order = 0)]
        [Range(1, 5)]
        public float HandlefollowSpeed = 3;


        //컨트롤러 버튼 선택.
        [Space(10, order = 0)]
        [SerializeField]
        protected ControllerButton m_grabButton = ControllerButton.Trigger;
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

        //유니티 이벤트 변수
        [Serializable]
        public class UnityEventController : UnityEvent<cs3DVRUI_DragBase> { }


        //컨트롤로 이벤트 리스트
        protected OrderedIndexedTable<ColliderButtonEventData, Pose> eventList = new OrderedIndexedTable<ColliderButtonEventData, Pose>();




        protected bool PowerOn = false;
        bool IsPowerOn() { return PowerOn; }

        protected Vector3 OriginPosition;

        public bool IsGrabbed { get { return eventList.Count > 0; } }

        protected bool alignPosition = true;
        protected bool alignRotation = true;
        protected bool overrideMaxAngularVelocity = true;

        protected Pose GetEventPose(ColliderButtonEventData eventData)
        {
            var grabberTransform = eventData.eventCaster.transform;
            return new Pose(grabberTransform);
        }

        public ColliderButtonEventData grabbedEvent { get { return IsGrabbed ? eventList.GetLastKey() : null; } }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            grabButton = m_grabButton;
        }
#endif
        // Use this for initialization
        protected virtual void Start()
        {
            alignPosition = true;
            alignRotation = true;

            grabButton = m_grabButton;
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



        public virtual void OnColliderEventPressExit(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressExit");
        }



        #region Drag Event ------------------------------------------------------


        public virtual void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {
        
        }



        public virtual void OnColliderEventDragStart(ColliderButtonEventData eventData)
        {

            if (!eventData.IsViveButton(m_grabButton)) { return; }



            var casterPose = GetEventPose(eventData);
            var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

            if (alignPosition) { offsetPose.pos = Vector3.zero; }
            if (alignRotation) { offsetPose.rot = Quaternion.identity; }

            eventList.AddUniqueKey(eventData, offsetPose);


            Debug.Log("cs3DVRUI_Base::OnColliderEventDragStart");
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

            Debug.Log("cs3DVRUI_Base::OnColliderEventDragFixedUpdate");
        }

        public virtual void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragUpdate");
        }

        public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragEnd");
        }
        #endregion

    }
}