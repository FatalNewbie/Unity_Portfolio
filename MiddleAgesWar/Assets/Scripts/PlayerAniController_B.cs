using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniController_B : MonoBehaviour
{

    PlayerController_B mPlayerCtrl;

    void Start()
    {
        GameObject go = GameObject.Find("PlayerObj_B");
        if (go == null)
        {
            Debug.Log("PlayerObj is Missing");
        }
        mPlayerCtrl = go.GetComponent<PlayerController_B>();
        if (mPlayerCtrl == null)
        {
            Debug.Log("PlayerController in PlayerObj is Missing");
        }

    }


    // 슬래쉬 끝날때쯤에 호출되는 함수.
    void SlashEnd()
    {
        mPlayerCtrl.SendMessage("SlashState");
    }

    void FallEnd()
    {
        mPlayerCtrl.SendMessage("PlayerStandUp");
    }
}
