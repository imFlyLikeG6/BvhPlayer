using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file TimelineUnitActive.cs
 * @brief  New_3DViewer_Replay씬에서 쓰이는 타임라인 UI관리 클래스.
 */
public class TimelineUnitActive : MonoBehaviour {

    public UIWidget m_Root;
    public ReplaySceneUI m_ReplaySceneUI;
    public UIFont m_Font;
    public int m_TotalFrame = 0;

    public GameObject[] m_LOD;

    private int m_LODCount = 0;

    private int m_MouseDown = -1;

    // Update is called once per frame
    void Update() {

        if(m_TotalFrame > 0)
        {
            if ((float)m_Root.width / m_TotalFrame <= 0.2f)
            {
                if (m_LODCount != 2)
                {
                    SetLOD(2);
                }
            }
            else if ((float)m_Root.width / m_TotalFrame <= 0.4f)
            {
                if (m_LODCount != 1)
                {
                    SetLOD(1);
                }
            }
            else
            {
                if (m_LODCount != 0)
                {
                    SetLOD(0);
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) m_MouseDown = 0; //left
        else if (Input.GetMouseButtonDown(1)) m_MouseDown = 1;    //right
        else if (Input.GetMouseButtonDown(2)) m_MouseDown = 2;    //middle

    }

    /**
    * @brief 타임라인의 프레임숫자에 LOD를 적용합니다. (lod : 0~2)
    */
    public void SetLOD(int lod)
    {
        m_LODCount = lod;
        for (int i = 0; i < m_LOD.Length; i++)
        {
            m_LOD[i].SetActive(i >= m_LODCount);
        }
    }

    /**
    * @brief 전체프레임을 받아 타임라인에 프레임숫자ux를 배치합니다. 100프레임 단위.
    */
    public void MakeTimeLineUnitText(int totalFrame)
    {
        for(int i =0; i< m_LOD.Length; i++)
        {
            foreach (Transform child in m_LOD[i].transform)
            {
                if (m_Root.transform != child)
                    Destroy(child.gameObject);
            }
        }

        int width = m_Root.width;
        m_TotalFrame = totalFrame;
        int count = totalFrame / 100;
        float anchors = 100.0f / totalFrame;
        int lodcount = 0;
        for (int i = 1; i <= count; i++)
        {
            if (i % 10 == 0) lodcount = 2;
            else if (i % 5 == 0) lodcount = 1;
            else lodcount = 0;
            UILabel lb = NGUITools.AddChild<UILabel>(m_LOD[lodcount].gameObject);
            float pos = i * anchors;
            float rightAnchors = 0;
            if ( pos + (25.0f / width) > 1) rightAnchors = 1;
            //lb.SetAnchor(m_Root.gameObject);
            if(rightAnchors == 1)
            {
                lb.SetAnchor(m_Root.gameObject, (i - 0.5f) * anchors, 0.6f, rightAnchors, 0.8f);
            }
            else
            {
                lb.SetAnchor(m_Root.gameObject,
                    pos, -25,
                    0.6f, 0,
                    pos, 25,
                    0.8f, 0);
            }
            lb.bitmapFont = m_Font;
            lb.text = string.Format("{0}", i * 100);
            lb.fontSize = 14;
            lb.effectStyle = UILabel.Effect.Shadow;
            lb.UpdateAnchors();
        }
    }

    public void OnPress(bool isDown)
    {
        if (Input.GetMouseButtonDown(0)) m_MouseDown = 0;
        else if (Input.GetMouseButtonDown(1)) m_MouseDown = 1;

        if (isDown)
            m_ReplaySceneUI.TimelineMouseDown(m_MouseDown == 0);
        if(!isDown)
            m_ReplaySceneUI.TimelineMouseUp(m_MouseDown == 0);
    }

    public void OnDrag()
    {
        m_ReplaySceneUI.TimelineMouseDrag(m_MouseDown == 0);
    }
}
