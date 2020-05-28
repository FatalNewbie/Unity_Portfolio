using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController_Origin : MonoBehaviour {


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

    List<bool> mIsTouchInArea;       // Input.touches에 들어있는 i번째 터치가 카메라의 회전이나 핀치 줌,아웃에 사용될 터치인지 판별. touch phase의 start에서 정해준다.
                                     // 터치 Began이 조이스틱과 공격버튼 바깥이라면 True이고, 아니면 False를 주어 나머지 모든 연산에서 무시함.
    int mValidTouchCount;       // 터치 사이클에서 카메라에 영향을 주는 유효한 터치의 갯수를 측정하는 변수.(조이스틱과 공격 버튼 주변의 터치는 무시할 수 있도록).

    Vector3 mExMovePos;
    Vector3 mMovePos;
    Vector3 mDeltaPos;

    SceneStatus mNowStatus;

    enum SceneStatus
    {
        LOBBY,
        GAME
    };

    // Use this for initialization
    void Start()
    {
        mIsTouchInArea = new List<bool>();

        for (int i = 0; i < 10; i++)
        {
            mIsTouchInArea.Add(false);
        }

        mNowStatus = SceneStatus.LOBBY;

        mPlayer = GameObject.FindGameObjectWithTag("PlayerObj_B");
        if (mPlayer == null)
            Debug.Log("mPlyaer in MainCameraController is null");
    }

    // Update is called once per frame
    void Update()
    {
        float x = mRadius * Mathf.Sin(mSeta * Mathf.Deg2Rad) * Mathf.Cos(mDelta * Mathf.Deg2Rad);
        float z = mRadius * Mathf.Sin(mSeta * Mathf.Deg2Rad) * Mathf.Sin(mDelta * Mathf.Deg2Rad);
        float y = mRadius * Mathf.Cos(mSeta * Mathf.Deg2Rad);

        gameObject.transform.position = new Vector3(x, y, z) + mPlayer.transform.position + new Vector3(0, mCenterAdjustValue, 0);
        gameObject.transform.LookAt(mPlayer.transform.position + new Vector3(0, mCenterAdjustValue, 0));

        // 유효터치카운팅 변수 초기화.
        //mValidTouchCount = 0;

        // 리스트 초기화.
        //mIsTouchInArea.Clear();        

        if (Input.touchCount > 0)
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
                        mIsTouchInArea.Insert(i, true);
                    }
                }
            }

            //Debug.Log(mValidTouchCount);

            // 플래그 초기화.
            mPinchFlag = 0;

            //Debug.Log(mValidTouchCount);

            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (mIsTouchInArea[i])
                {
                    // 영역안의 유효터치가 1개일시
                    if (mValidTouchCount == 1)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            mStartPos = Input.GetTouch(i).position;
                            mExMovePos = mStartPos;
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            mMovePos = Input.GetTouch(i).position;
                            mDeltaPos = mMovePos - mExMovePos;

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


                            mExMovePos = Input.GetTouch(i).position;
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            // 해당 터치가 끝났으므로 해당 터치의 유효터치 여부를 false로 바꿔줌.
                            mIsTouchInArea.Insert(i, false);
                            // 유효터치 카운트 -1;
                            mValidTouchCount--;
                        }
                    }

                    // 영역안의 유효터치가 1개 이상 일시
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

    public float GetmDelta()
    {
        return mDelta;
    }

}
