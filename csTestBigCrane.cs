using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csTestBigCrane : MonoBehaviour
{


    Animation AnCrtrl_Holder;
    Animation AnCrtrl_Body;
    public float fMoveTime = 0.0f;
    public float fLRMoveTime = 0.0f;

    private bool ItemGetReady_Down= false;
    private bool ItemGetReady_Right = true;
    private bool IsCatch = false;
    private bool IsGage = false;


    public cs3DVRUI_PressureGauge Gauge;

    public GameObject CatchObj;
    public GameObject CatchPoint;

    public Text State_Text;

    float SavePosZ;

    // Use this for initialization
    void Start()
    {
        SavePosZ = transform.position.z;

        AnCrtrl_Body = transform.Find("Animation").GetComponent<Animation>();

        AnCrtrl_Holder = AnCrtrl_Body.transform.Find("Holder").GetComponent<Animation>();

        //AnCrtrl.Play("CraneUp");
        //AnCrtrl.Stop();


        // Set up some state; 

        

        DownValue();
        LeftValue();

        ItemGetReady_Right = true;

        State_Text.text = "";
    }



	
	// Update is called once per frame
	void Update () {
		
	}



    public void UpValue()
    {
        if (fMoveTime <= 0.0f)
        {           
            return;
        }
        else
            ItemGetReady_Down = false;



        fMoveTime = fMoveTime - 0.005f;
        AnCrtrl_Holder["HolderDown"].time = fMoveTime;
        AnCrtrl_Holder["HolderDown"].enabled = true;
        // Sample animations now. 
        AnCrtrl_Holder.Sample();
        AnCrtrl_Holder["HolderDown"].enabled = false;
        AnCrtrl_Holder["HolderDown"].speed = 0;
        AnCrtrl_Holder.Play("HolderDown", PlayMode.StopAll);

        State_Text.text = "Holder\nUp";

        if(IsCatch)
            Gauge.Event_Push();

        //if(IsCatch && !IsGage)
        //{
        //    for(int i =0; i <20; i++)
        //    {
        //        Gauge.Event_Push();
        //    }
        //    IsGage = true;
        //}

    }

    public void DownValue()
    {
        if (fMoveTime >= 0.93f)
        {
            ItemGetReady_Down = true;
            return;
        }
        else
            ItemGetReady_Down = false;

        fMoveTime = fMoveTime + 0.005f;


        AnCrtrl_Holder["HolderDown"].time = fMoveTime;
        AnCrtrl_Holder["HolderDown"].enabled = true;
        // Sample animations now. 
        AnCrtrl_Holder.Sample();
        AnCrtrl_Holder["HolderDown"].enabled = false;
        AnCrtrl_Holder["HolderDown"].speed = 0;
        AnCrtrl_Holder.Play("HolderDown", PlayMode.StopAll);
        State_Text.text = "Holder\nDown";

    }



    public void LeftValue()
    {


        if (fLRMoveTime >= 0.92f)
        {
            ItemGetReady_Right = true;
            return;
        }
        else
            ItemGetReady_Right = false;

        fLRMoveTime = fLRMoveTime + 0.005f;


        AnCrtrl_Body["CraneBodyLeft"].time = fLRMoveTime;
        AnCrtrl_Body["CraneBodyLeft"].enabled = true;
        // Sample animations now. 
        AnCrtrl_Body.Sample();
        AnCrtrl_Body["CraneBodyLeft"].enabled = false;
        AnCrtrl_Body["CraneBodyLeft"].speed = 0;
        AnCrtrl_Body.Play("CraneBodyLeft", PlayMode.StopAll);

        State_Text.text = "Holder\nLeft";
    }

    public void RightValue()
    {
        if (fLRMoveTime <= 0.0f)
        {
            fLRMoveTime = 0.0f;

            ItemGetReady_Right = true;
            return;
        }
        else
            ItemGetReady_Right = false;



        fLRMoveTime = fLRMoveTime - 0.005f;
        AnCrtrl_Body["CraneBodyLeft"].time = fLRMoveTime;
        AnCrtrl_Body["CraneBodyLeft"].enabled = true;
        // Sample animations now. 
        AnCrtrl_Body.Sample();
        AnCrtrl_Body["CraneBodyLeft"].enabled = false;
        AnCrtrl_Body["CraneBodyLeft"].speed = 0;
        AnCrtrl_Body.Play("CraneBodyLeft", PlayMode.StopAll);

        State_Text.text = "Holder\nRight";

    }


    public void Catch()
    {
        if(ItemGetReady_Down && ItemGetReady_Right && !IsCatch)
        {

            CatchObj.transform.parent = CatchPoint.transform;
            CatchObj.transform.localPosition = Vector3.zero;
            CatchObj.transform.localRotation = Quaternion.identity;

            State_Text.text = "Hold";
            IsCatch = true;
        }
        else
            State_Text.text = "Error";
    }


    public void ReleaseCatch()
    {

        if (ItemGetReady_Down && ItemGetReady_Right && IsCatch)
        {
            CatchObj.transform.parent = null;

            State_Text.text = "Release";
            Gauge.Event_Release();
            IsCatch = false;
        }
        else
            State_Text.text = "Error";

    }

    public void MoveForward()
    {
        if (50f <= transform.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f);
            State_Text.text = "Move\nForward";
        }

  

    }

    public void MoveBackward()
    {
        if(SavePosZ >= transform.position.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
        State_Text.text = "Move\nBackward";


    }
}

