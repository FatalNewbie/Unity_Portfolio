using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1HpBarMng : MonoBehaviour {

    Camera mCamera;
    GameObject mPlayer;
    Vector3 mDirection;

	// Use this for initialization
	void Start () {
        mCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mPlayer = GameObject.Find("PlayerObj_B");
        
	}
	    
	// Update is called once per frame
	void Update () {
        mDirection = mCamera.transform.position - mPlayer.transform.position;
        //transform.rotation = Quaternion.LookRotation(gameObject.transform.position - mCamera.transform.position);
        //gameObject.transform.LookAt(mDirection);
        transform.rotation = Quaternion.LookRotation(-mDirection);
            
    }
}
