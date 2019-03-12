using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StickObj : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    Image mBG;
    Image mStick;
    Vector3 mInputVector;

	// Use this for initialization
	void Start () {
        mBG = GetComponent<Image>();
        mStick = transform.GetChild(0).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {	
	}

    public void OnPointerUp(PointerEventData eventData)
    {
        mInputVector = Vector3.zero;
        mStick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mBG.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / mBG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / mBG.rectTransform.sizeDelta.y);

            mInputVector = new Vector3(pos.x, pos.y, 0);
            mInputVector = (mInputVector.magnitude > 1.0f) ? mInputVector.normalized : mInputVector;

            mStick.rectTransform.anchoredPosition = new Vector3(mInputVector.x * (mBG.rectTransform.sizeDelta.x / 3)
                , mInputVector.y * (mBG.rectTransform.sizeDelta.y / 3));

        }
    }

    public float GetHorVal()
    {
        return mInputVector.x;
    }

    public float GetVerVal()
    {
        return mInputVector.y;
    }
}
