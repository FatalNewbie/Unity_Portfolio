using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabolic_jump_test : MonoBehaviour {

    bool mSwitch;
    Rigidbody mRigid;

	// Use this for initialization
	void Start () {
        mSwitch = true;
        mRigid = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (mSwitch)
            Fire();
        if (!mSwitch)
        {
            if (mRigid.velocity == Vector3.zero)
                Debug.Log("Done!!");
        }
	}

    void Fire()
    {
        mRigid.velocity = new Vector3(20, 10, 0);
        //mRigid.AddForce(new Vector3(0, 10, 0));
        mSwitch = false;
    }

}
