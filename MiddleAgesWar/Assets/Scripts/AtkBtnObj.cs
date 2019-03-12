using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtkBtnObj : MonoBehaviour {

    // 클릭된 상태인지 확인하는 변수.
    bool mIsClick;
    // 클릭된 후에 콤보를 이어가기위해 정해진 구간안에서 한번더 눌렸는지 확인하는 변수.
    bool mIsClickAgain;


    public bool IsClick
    {
        set
        {
            mIsClick = value;
        }
        get
        {
            return mIsClick;
        }
    }

    public bool IsClickAgain
    {
        set
        {
            mIsClickAgain = value;
        }
        get
        {
            return mIsClickAgain;
        }
    }

	// Use this for initialization
	void Start () {
        mIsClick = false;
        mIsClickAgain = false;
	}
	
	// Update is called once per frame
	void Update () {
	
        

	}

    public void OnBtnClick()
    {
        if (mIsClick)
            mIsClickAgain = true;

        mIsClick = true;
    }

    
}
