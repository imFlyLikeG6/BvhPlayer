using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file SensorDataUI.cs
 * @brief New_3DViewer_Replay씬에서 쓰이는 sensor data표 UI관리 클래스. 
 */
public class SensorDataUI : MonoBehaviour {

    public UILabel m_SelectedSensorUI;
    public UILabel[] m_QuatUI;
    public UILabel[] m_EulerUI;

    public MotionController m_MotionController;

    private Vector3[] m_BoneDataV;
    private Quaternion[] m_BoneData;

    private int m_CurrentBoneIndex = -1;
    private int m_CurrentFrame = -1;
    private string m_DefaultLB = "Sensor ID";

    public void Awake()
    {
        m_DefaultLB = m_SelectedSensorUI.text;
    }

    public void Clear()
    {
        foreach (UILabel lb in m_QuatUI)
        {
            lb.text = string.Empty;
        }
        foreach (UILabel lb in m_EulerUI)
        {
            lb.text = string.Empty;
        }
        m_BoneDataV = null;
        m_BoneData = null;
        m_CurrentBoneIndex = -1;
        m_CurrentFrame = -1;
        m_SelectedSensorUI.text = m_DefaultLB;
    }

    public void SetBoneIndexObj(object idx)
    {
        SetBoneData((int)idx);
    }

    public void SetBoneData(int idx)
    {
        m_CurrentBoneIndex = idx;

        m_BoneDataV = null;
        m_BoneData = null;
        m_BoneDataV = m_MotionController.GetBoneDataV(idx);
        m_BoneData = m_MotionController.GetBoneData(idx);
        UpdateData();
    }

    public void SetCurrentFrame(int frame)
    {
        m_CurrentFrame = frame;
        UpdateData();
    }

    public void UpdateData()
    {
        if (m_BoneData != null && m_CurrentFrame >= 0)
        {
            if (m_BoneData.Length > m_CurrentFrame)
            {
                if(m_QuatUI != null)
                {
                    m_QuatUI[0].text = string.Format("X\n{0:F3}", m_BoneData[m_CurrentFrame].x);
                    m_QuatUI[1].text = string.Format("Y\n{0:F3}", m_BoneData[m_CurrentFrame].y);
                    m_QuatUI[2].text = string.Format("Z\n{0:F3}", m_BoneData[m_CurrentFrame].z);
                    m_QuatUI[3].text = string.Format("W\n{0:F3}", m_BoneData[m_CurrentFrame].w);
                }
                if (m_EulerUI != null)
                {
                    m_EulerUI[0].text = string.Format("X\n{0:F2}", m_BoneDataV[m_CurrentFrame].x);
                    m_EulerUI[1].text = string.Format("Y\n{0:F2}", m_BoneDataV[m_CurrentFrame].y);
                    m_EulerUI[2].text = string.Format("Z\n{0:F2}", m_BoneDataV[m_CurrentFrame].z);
                }
            }
        }
    }
}

