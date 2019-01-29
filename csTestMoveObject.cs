using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestMoveObject : MonoBehaviour
{
    public float OneMoveInterval = 0.01f;
    public float limit = 0.27f;

    Transform ArrowObj;

	// Use this for initialization
	void Start ()
    {

        ArrowObj = transform.Find("CrossArrow");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpMove()
    {

        if (ArrowObj && limit >= ArrowObj.transform.localPosition.y)
            ArrowObj.transform.localPosition = new Vector3(ArrowObj.transform.localPosition.x, ArrowObj.transform.localPosition.y + OneMoveInterval, ArrowObj.transform.localPosition.z);
    }

    public void DownMove()
    {
        if (ArrowObj && (limit * -1.0f) <= ArrowObj.transform.localPosition.y)
            ArrowObj.transform.localPosition = new Vector3(ArrowObj.transform.localPosition.x, ArrowObj.transform.localPosition.y - OneMoveInterval, ArrowObj.transform.localPosition.z);
    }

    public void LeftMove()
    {
        if (ArrowObj && (limit * -1.0f) <= ArrowObj.transform.localPosition.x)
            ArrowObj.transform.localPosition = new Vector3(ArrowObj.transform.localPosition.x - OneMoveInterval, ArrowObj.transform.localPosition.y , ArrowObj.transform.localPosition.z);
    }

    public void RightMove()
    {
        if (ArrowObj && limit >= ArrowObj.transform.localPosition.y)
            ArrowObj.transform.localPosition = new Vector3(ArrowObj.transform.localPosition.x + OneMoveInterval, ArrowObj.transform.localPosition.y, ArrowObj.transform.localPosition.z);
    }

}
