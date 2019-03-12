using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMng : MonoBehaviour {

    [SerializeField] GameObject mHitObj1;
    [SerializeField] GameObject mHitObj2;
    [SerializeField] GameObject mHitObj3;

    // Use this for initialization
    void Start () {

        mHitObj1.SetActive(false);

        if (mHitObj2 != null)
        {
            mHitObj2.SetActive(false);
         
               if (mHitObj3 != null)
                mHitObj3.SetActive(false);
        }
    }

  
    // 설정된 히트 박스를 킴.
    void HitOn(int num)
    {
        if (num == 1)
            mHitObj1.SetActive(true);
        else if (num == 2)
            mHitObj2.SetActive(true);
        else if(num==3)
            mHitObj3.SetActive(true);

        
    }

    // 설정된 히트 박스를 끔.
    void HitOff(int num)
    {
        if(num==1)
            mHitObj1.SetActive(false);
        else if (num == 2)
            mHitObj2.SetActive(false);
        else if (num == 3)
            mHitObj3.SetActive(false);
    }
    
    

}
