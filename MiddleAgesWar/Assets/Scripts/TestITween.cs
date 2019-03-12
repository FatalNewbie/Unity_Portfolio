using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestITween : MonoBehaviour {

    public float height = 5.5f;
    [SerializeField] Transform target;
    public float speed = 1.0f;

    Vector3 m_mid;
    Vector3 m_cosV;
    float angle = 0f;

    private void Start()
    {
        m_mid = (target.position + transform.position) / 2f;
        m_cosV = transform.position - m_mid;
    }

    void Update()
    {
        if (transform.position.y > -0.1f)
        {
            transform.position = m_cosV * Mathf.Cos(angle) + height * Vector3.up * Mathf.Sin(angle) + m_mid;
            //transform.position = m_cosV * Mathf.Cos(angle) + m_mid;
            //transform.position = height * Vector3.up * Mathf.Sin(angle);
            angle += speed * Time.deltaTime;
        }

        
    }

    


}



