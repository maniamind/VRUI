using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

namespace VR3DUI_BaseDisplay
{

    public class cs3DVRUI_DisplayBase : MonoBehaviour
                , IColliderEventPressDownHandler
    {


        [Serializable]
        public class UnityEventController : UnityEvent<cs3DVRUI_DisplayBase> { }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {

        }


    }

}