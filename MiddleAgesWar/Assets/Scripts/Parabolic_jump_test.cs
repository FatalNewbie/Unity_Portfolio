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
        

        
	}

    void Fire()
    {
        Vector3 mVec = new Vector3(-1.02f, 24, 0.82f);

        mRigid.velocity = mVec;

        //mRigid.AddForce(new Vector3(0, 10, 0));
        mSwitch = false;
    }

}
