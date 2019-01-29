using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;


using VR3DUI_BaseDisplay;
public class cs3DVRUI_TapeMeasure : cs3DVRUI_DisplayBase
{




    //단위를 넣자..


    public float MarkingUnit = 100f;



    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]
    public UnityEventController Event_TriggerClicked = new UnityEventController();
    public UnityEventController Event_PadClicked = new UnityEventController();


    private GameObject Obj_Pin;
    private GameObject Obj_TotalLength;

    Transform TextObj;


    private LineRenderer currLine;

    bool IsPress = false;
    bool IsInit = false;

    private bool IsExit = false;

    public GameObject ControllerObj = null;




    cs3DVRUI_TapeCtrl TapeControl = null;

    void Start()
    {

        Transform trans = GameObject.Find("VROrigin").transform.Find("[CameraRig]");
        if (ControllerObj == null)
        {
            ControllerObj = trans.Find("Controller (left)").gameObject;

            //if (!ControllerObj.activeSelf)
            //{
            //    ControllerObj = trans.Find("Controller (right)").gameObject;

            //}

        }


        Obj_Pin = transform.Find("Pin").gameObject;
        Obj_TotalLength = transform.Find("Length").gameObject;
        currLine = GetComponent<LineRenderer>();

    }


    // Update is called once per frame
    void Update()
    {
        if(IsPress)
        {
            currLine.SetPosition(TapeControl.GetClickCount() + 1, ControllerObj.transform.position);
        }

    }

    public override void OnColliderEventPressDown(ColliderButtonEventData eventData)
    {
        for (int i = 0; i < eventData.pressEnteredObjects.Count; i++)
        {
            if (eventData.pressEnteredObjects[i].tag == "TapeMeasure")
            {

                if (ControllerObj.activeSelf && TapeControl == null)
                {
                    Obj_Pin.transform.parent = ControllerObj.transform;

                    Obj_TotalLength.transform.parent = ControllerObj.transform;
                

                    TapeControl = ControllerObj.AddComponent<cs3DVRUI_TapeCtrl>();
                    TapeControl.Init();
                    TapeControl.SetMeasure(this);
                    
                }
                    
                break;
            }
        }

    }

    public void TapeMeasureClick(Transform trans)
    {
        if (IsExit)
            IsExit = false;

        if (TapeControl.GetClickCount() == 0)
        {
            currLine.SetPosition(0, trans.position);
            currLine.SetPosition(1, trans.position);
        }


        currLine.SetVertexCount(TapeControl.GetClickCount() + 2);
        currLine.SetPosition(TapeControl.GetClickCount()+1, trans.position);
        IsPress = true;

        Event_TriggerClicked.Invoke(this);

}

    public void TapeMeasureUnClick(Transform trans)
    {

        if (!IsPress)
            return;

        IsPress = false;


    
    }


    public void TapeMeasurePadClick()
    {
        

        currLine.SetVertexCount(0);
        currLine.SetVertexCount(2);

        //모드를 빠져 나간다
        if(IsExit)
        {
            Obj_Pin.transform.parent = transform;
            Obj_Pin.transform.localPosition = new Vector3(0f, 12f, 0f);
            Obj_Pin.transform.localRotation = Quaternion.identity;
            Obj_Pin.SetActive(false);




            Obj_TotalLength.transform.parent = transform;
            Obj_TotalLength.SetActive(false);
            Obj_TotalLength.transform.localPosition = Vector3.zero;
            Obj_TotalLength.transform.localRotation = Quaternion.identity;

            Destroy(TapeControl);

                
        }

        IsExit = true;

        Event_PadClicked.Invoke(this);

    }


}
