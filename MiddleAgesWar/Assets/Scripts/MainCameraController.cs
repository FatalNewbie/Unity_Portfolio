﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {

    [SerializeField] float mSeta = 45;                      // 카메라 상하 이동 각
    [SerializeField] float mDelta = -90;                    // 카메라 좌우 이동 각
    [SerializeField] float mRadius = 5;
    [SerializeField] GameObject mPlayer;
    [SerializeField] float mCenterAdjustValue = 1.7f;       // 이 값은, 이 값을 더하지 않으면 카메라가 캐릭터 몸통 중앙쪽을 향하기 때문에 카메라가 캐릭터 머리쪽을 향하게 하기 위해 더하는 값임. 구면좌표계 원점을 이 값만큼 위로 올림.
    // [SerializeField] float mCameraMoveSensti = 2.5f;     // 처음에 이 값을 카메라 각도에 더해줘서 카메라 움직여줫는데(카메라 민감도), 현재는 터치 변화량을 그대로 카메라 각도에 더해주고 있기 때문에 일단 사용안함.

    int mPinchCount;            // 핀치 줌과 핀치 아웃을 하기 위해 화면의 조이스틱 부분을 제외한 부분에 몇개의 터치가 이루어졌는지 세는 카운터.

    Vector3 mStartPos;
    float mNowDistance;       // 현재 프레임에서 핀치 줌과 아웃 시 두 터치 지점 사이의 거리
    float mExDistance;        // 이전 프레임에서 핀치 줌과 아웃 시 두 터치 지점 사이의 거리
    float mDeltaPinchDistance;          // 이전 프레임과 현재 프레임의 두 터치 지점 사이의 변화량
    Vector3 mFirstTouch;        // 핀치에 사용할 첫번째 터치
    Vector3 mSecondTouch;       // 핀치에 사용할 두번째 터치
    int mPinchFlag;             // 첫번째, 두번째 터치 구분용 플레그.

    List<int> mValidTouchFingerID;       // Input.touches에 들어있는 i번째 터치가 카메라의 회전이나 핀치 줌,아웃에 사용될 터치인지 판별. touch phase의 start에서 정해준다.
                                     // 터치 Began이 조이스틱과 공격버튼 바깥이라면 True이고, 아니면 False를 주어 나머지 모든 연산에서 무시함.
    int mValidTouchCount;       // 터치 사이클에서 카메라에 영향을 주는 유효한 터치의 갯수를 측정하는 변수.(조이스틱과 공격 버튼 주변의 터치는 무시할 수 있도록).

    Vector3 mExMovePos;
    Vector3 mMovePos;
    Vector3 mDeltaPos;

    int mTestTouchCnt=0;

    SceneStatus mNowScenceStatus;

    enum SceneStatus
    {
        LOBBY,
        GAME
    };

    TouchStatus mNowTouchStatus; 

    enum TouchStatus
    {
        NONE,
        MOVE,
        PINCH
    };

    // Use this for initialization
    void Start() {
        mValidTouchFingerID = new List<int>();



        mNowScenceStatus = SceneStatus.LOBBY;

        mPlayer = GameObject.FindGameObjectWithTag("PlayerObj_B");
        if (mPlayer == null)
            Debug.Log("mPlyaer in MainCameraController is null");
    }

    // Update is called once per frame
    void Update() {

        // 이 안을 깔끔하게 만들고 싶다 이거지.
        // 
        /*
         일단 카메라의 위치와 방향을 잡아주는 MainCameraPositionSetting메소드는 매 업데이트 마다 체크 되어야 함.
         화면상의 유효터치 갯수를 체크하는 메소드도 항상 돌아야 함.
         유효터치 갯수를 체크하는 메소드가 먼저 돌아서 갯수를 정하고 그 갯수에 맞는 메소드만 돌아갈 수 있도록.
         
         */


        // 카메라의 위치를 잡고, 방향을 캐릭터를 향하게 함.
        MainCameraPositionSetting();



        // 유효터치카운팅 변수 초기화.
        //mValidTouchCount = 0;

        // 리스트 초기화.
        //mIsTouchInArea.Clear();        

        

        if (Input.touchCount > 0)
        {


            //카메라 조작을 위한 터치와  조이스틱과 다른 버튼들의 터치를 구분해내는 메소드.
            ValidTouchCheck();

         




            // 플래그 초기화.
            mPinchFlag = 0;

            // 유효터치 갯수가 1개이 상일때 실행. 이 안에서 카메라 회전과 핀치 줌 작동.
            if (mValidTouchCount>0)
            {
                // 카메라 회전
                CameraMove();

                // 유효터치 갯수가 2개 이상일때 실행. 핀치 줌 작동.
                if (mValidTouchCount > 1)
                {

                }
            }


            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (mIsTouchInArea[i])
                {
                    // 영역안의 유효터치가 1개일시
                    // 1개의 터치로 카메라 돌리기
                    if (mValidTouchCount == 1)
                    {
                        CameraMove();
                    }

                    // 영역안의 유효터치가 1개 이상 일시
                    // 2개의 터치로 핀치 줌
                    else
                    {
                        // Began 페이즈에만 불러지는건데 이때는 mPinchFlag를 -1로 설정하여 특수한 상황으로 설정.
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {

                            if (mPinchFlag == 0)
                            {
                                mFirstTouch = Input.GetTouch(i).position;
                                mPinchFlag++;
                            }
                            else if (mPinchFlag == 1)
                            {
                                mSecondTouch = Input.GetTouch(i).position;
                                mPinchFlag = -1;
                            }
                        }
                        // Move 페이즈에서는 Flag를 이용하여 첫번째 터치와 두번째 터치를 저장.
                        else if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            if (mPinchFlag == 0)
                            {
                                mFirstTouch = Input.GetTouch(i).position;
                                mPinchFlag++;
                            }
                            else if (mPinchFlag == 1)
                            {
                                mSecondTouch = Input.GetTouch(i).position;
                                mPinchFlag++;
                            }
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            // 해당 터치가 끝났으므로 해당 터치의 유효터치 여부를 false로 바꿔줌.
                            mIsTouchInArea.Insert(i, false);
                            // 유효터치 카운트 -1;
                            mValidTouchCount--;
                        }

                        //if (Input.GetTouch(i).phase == TouchPhase.Began)
                        //{
                        //    mStartPos = Input.GetTouch(i).position;
                        //    mExMovePos = mStartPos;
                        //}
                        //else if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        //{
                        //    mMovePos = Input.GetTouch(i).position;
                        //    mDeltaPos = mMovePos - mExMovePos;

                        //    if (mDeltaPos.x != 0)
                        //    {
                        //        if (mDeltaPos.x > 0)
                        //            mDelta -= mDeltaPos.x;
                        //        else
                        //            mDelta -= mDeltaPos.x;
                        //    }

                        //    if (mDeltaPos.y != 0)
                        //    {
                        //        if (mDeltaPos.y > 0 && mSeta <= 90.0f)
                        //            mSeta += mDeltaPos.y;
                        //        else if (mDeltaPos.y < 0 && mSeta >= 20)
                        //            mSeta += mDeltaPos.y;
                        //    }

                        //    Debug.Log(mDeltaPos);
                        //    mExMovePos = Input.GetTouch(i).position;
                        //}
                        //else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        //{

                        //}
                    }

                    // Began페이즈시 발생한 특수한 상황으로 이전거리를 저장함.
                    if (mPinchFlag == -1)
                    {
                        mExDistance = Vector3.Distance(mFirstTouch, mSecondTouch);
                    }

                    // 핀치를 위한 두개의 터치가 입력됐을때만 실행되는데 이전 프레임과의 거리 변화량을 측정하여 카메라 위치를 조정함.
                    if (mPinchFlag == 2)
                    {
                        mNowDistance = Vector3.Distance(mFirstTouch, mSecondTouch);
                        mDeltaPinchDistance = (mNowDistance - mExDistance) / 20;
                        if (mDeltaPinchDistance > 2)
                            mDeltaPinchDistance = 2;
                        else if (mDeltaPinchDistance < -2)
                            mDeltaPinchDistance = -2;
                        //Debug.Log(mDeltaPinchDistance);
                        if (mDeltaPinchDistance > 0 && mRadius <= 10)
                            mRadius -= mDeltaPinchDistance;
                        else if (mDeltaPinchDistance < 0 && mRadius >= 1)
                            mRadius -= mDeltaPinchDistance;
                        //mRadius +=  mDeltaPinchDistance;
                        mExDistance = mNowDistance;
                    }
                }

            }

            // input.touch를 1바퀴 돌면서 Ended상태의 터치를 ValidTouchFingerID 리스트에서 지워줌.
            mValidTouchFingerIDUpdater();

        }


        


        //if (Input.GetMouseButtonDown(0) == true)
        //{
        //    mStartPos = Input.mousePosition;
        //    mExMovePos = mStartPos;
        //}
        //else if (Input.GetMouseButtonUp(0) == true)
        //{

        //}
        //else if (Input.GetMouseButton(0) == true)
        //{
        //    mMovePos = Input.mousePosition;
        //    mDeltaPos = mMovePos - mExMovePos;

        //    if (mDeltaPos.x != 0)
        //    {
        //        if (mDeltaPos.x > 0)
        //            mDelta -= mCameraMoveSensti;
        //        else
        //            mDelta += mCameraMoveSensti;
        //    }

        //    if (mDeltaPos.y != 0 )
        //    {
        //        if (mDeltaPos.y > 0 && mSeta <= 90.0f)
        //            mSeta += mCameraMoveSensti;
        //        else if(mDeltaPos.y<0 && mSeta >= 20 )
        //            mSeta -= mCameraMoveSensti;
        //    }

        //    Debug.Log(Input.mousePosition);
        //    mExMovePos = Input.mousePosition;
        //}


    }

    public void MainCameraPositionSetting()
    {
        float x = mRadius * Mathf.Sin(mSeta * Mathf.Deg2Rad) * Mathf.Cos(mDelta * Mathf.Deg2Rad);
        float z = mRadius * Mathf.Sin(mSeta * Mathf.Deg2Rad) * Mathf.Sin(mDelta * Mathf.Deg2Rad);
        float y = mRadius * Mathf.Cos(mSeta * Mathf.Deg2Rad);

        gameObject.transform.position = new Vector3(x, y, z) + mPlayer.transform.position + new Vector3(0, mCenterAdjustValue, 0);
        gameObject.transform.LookAt(mPlayer.transform.position + new Vector3(0, mCenterAdjustValue, 0));
    }


    public float GetmDelta()
    {
        return mDelta;
    }

    public void CameraMove()
    {
        // 첫번째 validtouch의 FinerID를 사용해 input.touch의 인덱스 구함. 그 인덱스를 target에 저장.
        int target = FindTouchFromFingerID(mValidTouchFingerID[0]);

        if (Input.GetTouch(target).phase == TouchPhase.Moved)
        {
            mMovePos = Input.GetTouch(target).position;
            mDeltaPos = mMovePos - mExMovePos;
            mExMovePos = Input.GetTouch(target).position;

            if (mDeltaPos.x != 0)
            {
                if (mDeltaPos.x > 0)
                    mDelta -= mDeltaPos.x;
                else
                    mDelta -= mDeltaPos.x;
            }

            if (mDeltaPos.y != 0)
            {
                if (mDeltaPos.y > 0 && mSeta <= 90.0f)
                    mSeta += mDeltaPos.y;
                else if (mDeltaPos.y < 0 && mSeta >= 20)
                    mSeta += mDeltaPos.y;
            }


        }
        //else if (Input.GetTouch(target).phase == TouchPhase.Ended)
        //{
        //    // 해당 유효터치가 끝났으므로 해당 터치를 유효터치 리스트에서 빼줌.
        //    mValidTouchFingerID.Remove(0);
            
        //    // 유효터치 카운트 -1;
        //    mValidTouchCount--;
        //}
    }

    void ValidTouchCheck()
    {
        // Input에 들어온 터치들을 한번씩 체크하여 조이스틱과 공격버튼 밖의 터치들의 갯수를 mValidTouchCount를 통하여 카운트 하고, 
        // 해당되는 i번째와 같은 순서의 mIsTouchInArea의 값을 true로 바꿔준다.
        for (int i = 0; i < Input.touchCount; ++i)
        {

            // Began페이즈의 터치만 체크함.
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                // 해당범위 안의 터치를 골라넴.
                if ((Input.GetTouch(i).position.x > 120 && Input.GetTouch(i).position.x < 400) || Input.GetTouch(i).position.y > 110)
                {
                    mValidTouchCount++;

                    mValidTouchFingerID.Add(Input.GetTouch(i).fingerId);

                    // 여기서 차후에 터치 이동 변화량을 계산하기 위해 시작점을 미리 저장한다.
                    mExMovePos = Input.GetTouch(i).position;
                }
            }
        }
    }

    // 주어진 fingerID를 사용하여 해다 fingerID를 가지고 있는 Input.touches의 인덱스를 반환
    public int FindTouchFromFingerID(int FingerID)
    {
        int target = 0;

        for (int i=0; i<Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == FingerID)
                target = i;
        }

        return target;

     }

    // 매 Update시 실행. input.touch를 1바퀴 돌면서 Ended상태의 터치를 ValidTouchFingerID 리스트에서 지워줌.
    public void mValidTouchFingerIDUpdater()
    {
        // 터치들 중에서
        for (int i = 0; i < Input.touchCount; i++)
        {
            // Ended phase인 터치가 있다면
            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                // mValidTouchFinderID리스트에 해당 터치가 있다면 삭제하고 mValidTouchCount를 1 줄여줌.
                for (int j = 0; j < mValidTouchFingerID.Count; j++)
                {
                    if (Input.GetTouch(i).fingerId == mValidTouchFingerID[j])
                    {
                        Debug.Log("Before:)");
                        mValidTouchFingerID.Remove(j);
                        mValidTouchCount--;
                        Debug.Log("After :)");
                    
                    }
                }
            }

        }
        
    }




}
