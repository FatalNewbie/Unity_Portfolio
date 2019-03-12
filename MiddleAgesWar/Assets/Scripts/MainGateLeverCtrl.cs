using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGateLeverCtrl : MonoBehaviour {

    bool mMainGateOpened;
    [SerializeField] GameObject mMainGateObj;

	// Use this for initialization
	void Start () {
        mMainGateOpened = false;
        gameObject.transform.position = new Vector3(-17.4f, 1.49f, 2.67f);
        gameObject.transform.rotation = Quaternion.Euler(-45, -90, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OpenMainGate()
    {
        if (!mMainGateOpened)
        {
            gameObject.transform.position = new Vector3(-14.41f, 1.478343f, 2.67f);
            gameObject.transform.rotation = Quaternion.Euler(-135, -90, 0);
            mMainGateOpened = true;
        }
        mMainGateObj.SetActive(false);
    }


}
