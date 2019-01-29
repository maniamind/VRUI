using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class cs3DVRUI_TapeCtrl : MonoBehaviour
{


    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;
    private SteamVR_TrackedController _controller = null;
    


    public GameObject ControllerObj = null;

    cs3DVRUI_TapeCtrl TapeControl;
    GameObject Pin_Prefab;
    GameObject TotalLength;
    cs3DVRUI_TapeMeasure TapeMeasure;


    private bool IsPress = false;

    private bool Init_Check = false;

    int ClickCount = 0;

    

    Vector3 beforPos;

    Vector3 CurrPos;


    float TotalLen = 0.0f;
    GameObject PinGroup;

    private void OnEnable()
    {
        
        Init();

    }

    public void SetMeasure(cs3DVRUI_TapeMeasure Measure)
    {
        TapeMeasure = Measure;
    }



    public void Init()
    {
        
        if (_controller == null)
        {
            _controller = GetComponent<SteamVR_TrackedController>();
            _controller.TriggerClicked += HandleTriggerClicked;
            _controller.TriggerUnclicked += HandleTriggerUnclicked;
            _controller.PadClicked += HandlePadClicked;

            Pin_Prefab = transform.Find("Pin").gameObject;
            TotalLength = transform.Find("Length").gameObject;
            TotalLength.SetActive(true);

            PinGroup = GameObject.Find("Pin_Group").gameObject;
            

            Init_Check = true;
        }

    }


    public void InitValue()
    {

        ClickCount = 0;
        TotalLen = 0.0f;

        //깃발 삭제 코드 넣자.
        DestroyChild(PinGroup.transform);
    }


    private void DestroyChild(Transform root)
    {

        int nChildCount = root.GetChildCount();

        for (int j = 0; j < nChildCount; j++)
        {

            
            Destroy(root.transform.GetChild(j).gameObject);
        }
    }





    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerUnclicked;
        _controller.PadClicked -= HandlePadClicked;

    }


    // Update is called once per frame
    //void Update()
    //{
    //    if (IsPress && Init_Check)
    //    {

    //        currLine.SetPosition(1, transform.position);
    //    }

    //}


    #region Primitive Spawning
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (!Init_Check)
            return;

        GameObject NewPin = Add_Pin(PinGroup, Pin_Prefab);


        NewPin.transform.position = transform.position;
        NewPin.transform.rotation = transform.rotation;

      //  NewPin.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);

        NewPin.name = "Pin";
        NewPin.tag = "Measure_Pin";
        NewPin.SetActive(true);

        //currLine = NewPin.transform.GetComponent<LineRenderer>();

        GameObject Show = NewPin.transform.Find("Pin_Show").transform.gameObject;
     //   GameObject LenObj = Show.transform.Find("TextObj").LookAt(Camera.main.tra//nsform);
        
        if (ClickCount == 0)
        {
            Show.SetActive(true);
          //  currLine.SetPosition(0, transform.position);
            CurrPos = transform.position;
        }
        else
        {
            Show.SetActive(false);
            //currLine.SetPosition(0, beforPos);

            CurrPos = beforPos;
        }
        

        IsPress = true;

        TapeMeasure.TapeMeasureClick(NewPin.transform);
    }

    public void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        if (!Init_Check||!IsPress)
            return;

        GameObject NewPin = Add_Pin(PinGroup, Pin_Prefab);


        NewPin.transform.position = transform.position;
        NewPin.transform.rotation = transform.rotation;

      //  NewPin.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);


        NewPin.SetActive(true);



        NewPin.name = "Pin";
        NewPin.tag = "Measure_Pin";

        beforPos = transform.position;
        IsPress = false;

        float distance = Vector3.Distance(CurrPos, transform.position);

        distance = distance * TapeMeasure.MarkingUnit;


        Text mLength = NewPin.transform.GetComponentInChildren<Text>();

        mLength.text = string.Format("{0:00.0}",  distance) ;

        TotalLen = TotalLen + distance;

        Text mTotalLength = TotalLength.transform.GetComponentInChildren<Text>();
        mTotalLength.text = string.Format("Total Length\n{0:00.0}", TotalLen);


        ClickCount++;

        TapeMeasure.TapeMeasureUnClick(transform);
    }


    public int GetClickCount() { return ClickCount; }


    #endregion

    #region Primitive Selection
    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        InitValue();
        TapeMeasure.TapeMeasurePadClick();
    }


    #endregion


    public GameObject Add_Pin(GameObject parent, GameObject prefab)
    {
        Vector3 scaile = prefab.transform.localScale;
        GameObject go = Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = scaile;
        }
        return go;
    }


}
