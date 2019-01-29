using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using VR3DUI_BaseDisplay;

public class cs3DVRUI_DigitalDisplay : cs3DVRUI_DisplayBase
{


    public float TimeInterval = 1.0f;

    public bool EnableNumberEvent = false;

    [Range(0, 9)]
    public int EventNumber = 0;

    private bool AutoCountRun = false;


    public UnityEventController NumberEvent = new UnityEventController();
    public UnityEventController StartAutoCountEvent = new UnityEventController();
    


    private Transform Numbers;
    private GameObject ActiveNumber;

    private int CurrentNumber = -1;
    public int GetCurNumber() { return CurrentNumber; }

	// Use this for initialization
	void Start ()
    {

        Numbers = transform.Find("body").Find("Num");
        ActiveNumber = null;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CoAutoCount()
    {
        while (true)
        {
            if (!AutoCountRun)
                break;

            yield return new WaitForSeconds(TimeInterval);
            SetNumber(++CurrentNumber);

               
          
        }

    }



    public void SetNumber(int num)
    {

        if(num >= 0 && num <=9)
        {
            if (ActiveNumber != null)
                ActiveNumber.SetActive(false);


            ActiveNumber = Numbers.Find(num.ToString()).gameObject;
            ActiveNumber.SetActive(true);
            CurrentNumber = num;

            //특정 수자에 대한 이벤트 처리
            if (EnableNumberEvent && CurrentNumber == EventNumber)
            {
                StoptAutoCount();
                NumberEvent.Invoke(this);
            }
        }


    }


    public void InitCount()
    {
        CurrentNumber = -1;

        if (ActiveNumber != null)
            ActiveNumber.SetActive(false);
    }


    public void StartAutoCount()
    {
        if (AutoCountRun)
            return;
        else
            InitCount();

        AutoCountRun = true;
        StartCoroutine("CoAutoCount");
        StartAutoCountEvent.Invoke(this);



    }

    public void StoptAutoCount()
    {

        AutoCountRun = false;
        StopCoroutine("CoAutoCount");
    
    }

}
