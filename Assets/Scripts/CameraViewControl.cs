using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file CameraViewControl.cs
 * @brief 3D뷰어 크기조절 및 카메라이동 컨트롤러. 각 씬의 main Camera에 붙어있습니다. 
 */
public class CameraViewControl : MonoBehaviour {

    public Camera m_MainCamera;
    public UITexture m_ViewRect;

    public GameObject m_Model;
    public Vector3 m_CenterOffset = new Vector3(0, 1.472f, 0);
    public Vector3 m_HoldCenterOffset = new Vector3(0, 2.32f, 4.31f);
    public Vector3 m_MoveCenterOffset = new Vector3(0, 2.32f, 4.31f);

    public float sensitivityX = 8F;
    public float sensitivityY = 8F;
    public float sensitivityW = 8F;
    private float speed = 0.5f;

    private float deltaX;
    private float deltaY;
    private float deltaW;

    private float mouseX;
    private float mouseY;

    private int mouseDown = -1;
    public bool moveMode = true;
    public bool m_AndroidVer = false;

    // Use this for initialization
    void Start () {
        m_ViewRect.enabled = true;
        if(m_Model)
            LookAtCenter();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //화면크기 관련
        if (m_MainCamera.targetTexture != null) m_MainCamera.targetTexture.Release();
        m_MainCamera.targetTexture = new RenderTexture(m_ViewRect.width , m_ViewRect.height, 24);
        m_ViewRect.mainTexture = m_MainCamera.targetTexture;

        if (Input.GetMouseButtonDown(0)) mouseDown = 0; //left
        else if (Input.GetMouseButtonDown(1)) mouseDown = 1;    //right
        else if (Input.GetMouseButtonDown(2)) mouseDown = 2;    //middle

    }

    void LookAtCenter()
    {
        m_MainCamera.transform.LookAt(m_Model.transform.position + m_CenterOffset);
    }
    
    private void OnDrag()
    {
        if(moveMode)
        {
            if (mouseDown == 0)
            {
                deltaX = Input.GetAxis("Mouse X") * sensitivityX;
                deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
                m_MainCamera.transform.RotateAround(m_Model.transform.position + m_CenterOffset, m_Model.transform.up, deltaX);

                //Quaternion quat_rot = m_MainCamera.transform.rotation;
                //if ((quat_rot.eulerAngles.x == 90) && (deltaY < 0)) { }       // limit +Y axis
                //else if ((quat_rot.eulerAngles.x == 270) && (deltaY > 0)) { } // limit -Y axis
                //else
                {
                    m_MainCamera.transform.RotateAround(m_Model.transform.position + m_CenterOffset, m_MainCamera.transform.right, -deltaY);
                }
            }
            else if (mouseDown == 1)
            {
                if (m_AndroidVer)
                {

                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
                    deltaW = (currentMagnitude - prevMagnitude) * 0.01f;
                    //Debug.Log("x : " + Input.GetAxis("Mouse X") + ", y : " + Input.GetAxis("Mouse Y"));
                    //deltaW = Input.GetAxis("Mouse X") * Input.GetAxis("Mouse Y") * sensitivityW / 2;
                    if ((deltaW > 0) || (deltaW < 0) && moveMode)
                    {
                        m_MainCamera.transform.position += m_MainCamera.transform.forward * deltaW;
                    }
                }
                else
                {
                    deltaX = Input.GetAxis("Mouse X") / 2f;
                    deltaY = Input.GetAxis("Mouse Y") / 2f;
                    m_MainCamera.transform.Translate(-deltaX, -deltaY, 0);
                }
            }
        }
    }

    private void OnScroll()
    {
        deltaW = Input.GetAxis("Mouse ScrollWheel") * sensitivityW;
        if ((deltaW > 0) || (deltaW < 0) && moveMode)
        {
            m_MainCamera.transform.position += m_MainCamera.transform.forward * deltaW;
        }
    }

    public void SetMoveMode(bool mode)
    {
        if(moveMode != mode)
        {
            if(moveMode)
            {
                m_MoveCenterOffset = m_MainCamera.transform.position;
                m_MainCamera.transform.position = m_HoldCenterOffset;
            }
            else
            {
                m_MainCamera.transform.position = m_MoveCenterOffset;
            }
            LookAtCenter();
        }

        moveMode = mode;
    }
}
