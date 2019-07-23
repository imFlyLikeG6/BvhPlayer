using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file MotionController.cs
 * @brief bvhController와 CsvController의 base class.
 */
public class MotionController : MonoBehaviour {

    public JointController m_JointController;

    public Quaternion[] m_CalibrationData;
    public List<Quaternion[]> m_GlobalQuatData = new List<Quaternion[]>();
    public List<Quaternion[]> m_LocalQuatData = new List<Quaternion[]>();
    public List<Vector3[]> m_RotationDataV = new List<Vector3[]>();

    public List<Vector3> m_PositionData = new List<Vector3>();
    
    public int m_TotalFrames = 0;
    public float m_FrameTime = 0;
    private IEnumerator _PlayCoroutine = null;
    public int m_CurrentFrame = 0;
    public bool m_IsPlay = false;
    public float m_FrameRatio = 1;
    public bool m_DataInit;

    public bool m_IsBvhController = true;

    /**
    * @brief 초기화 함수.
    */
    public virtual void Clear()
    {
        if (m_GlobalQuatData != null)
            m_GlobalQuatData.Clear();
        if (m_LocalQuatData != null)
            m_LocalQuatData.Clear();
        if (m_RotationDataV != null)
            m_RotationDataV.Clear();
        if (m_PositionData != null)
            m_PositionData.Clear();
        m_DataInit = false;
        m_CalibrationData = null;
    }

    /**
    * @brief New_3DViewer_Replay에서 사용되는 재생함수.
    */
    IEnumerator PlayStart()
    {
        float startTime = Time.unscaledTime;
        float tick = startTime;
        int startFrame = m_CurrentFrame;
        while (m_CurrentFrame < m_TotalFrames - 1)
        {
            tick = Time.unscaledTime - startTime;
            if (m_FrameTime != 0)
            {
                int tickFrame = (int)(tick * m_FrameRatio / m_FrameTime);
                m_CurrentFrame = startFrame + tickFrame;
                if (m_CurrentFrame >= m_TotalFrames) m_CurrentFrame = m_TotalFrames - 1;

                if(m_IsBvhController)
                    m_JointController.SetRotation(m_LocalQuatData[m_CurrentFrame], m_PositionData[m_CurrentFrame]);
                else
                    m_JointController.SetWorldRotation(m_GlobalQuatData[m_CurrentFrame], new Vector3(0,100,0));
                
                yield return new WaitForSeconds(0.001f);
            }
        }
        SendMessage("SetPlayBtn", "play");
        Stop();
    }

    /**
    * @brief 해당 인덱스의 Euler 회전 데이터 값을 리턴합니다.
    */
    public Vector3[] GetBoneDataV(int index)
    {
        Vector3[] re = new Vector3[m_RotationDataV.Count];

        for (int i = 0; i < m_RotationDataV.Count; i++)
            re[i] = m_RotationDataV[i][index];

        return re;
    }
    
    /**
    * @brief 해당 인덱스의 quaternion 데이터 값을 리턴합니다.
    */
    public Quaternion[] GetBoneData(int index)
    {
        Quaternion[] re = new Quaternion[m_LocalQuatData.Count];

        for (int i = 0; i < m_LocalQuatData.Count; i++)
            re[i] = m_LocalQuatData[i][index];

        return re;
    }

    /**
    * @brief 모델에게 해당 프레임의 회전값을 적용합니다.
    */
    public void SetFrame(int frame)
    {
        if (frame < m_LocalQuatData.Count)
        {
            m_CurrentFrame = frame;
            if (m_IsBvhController)
                m_JointController.SetRotation(m_LocalQuatData[m_CurrentFrame], m_PositionData[m_CurrentFrame]);
            else
                m_JointController.SetWorldRotation(m_GlobalQuatData[m_CurrentFrame], new Vector3(0, 100, 0));
        }
    }

    /**
    * @brief 로딩 된 모션파일을 재생합니다.
    */
    public void Play(bool setIsplay = false)
    {
        if (_PlayCoroutine != null)
            StopCoroutine(_PlayCoroutine);

        if (setIsplay) m_IsPlay = true;
        _PlayCoroutine = PlayStart();
        StartCoroutine(_PlayCoroutine);
    }

    /**
    * @brief 로딩 된 모션파일의 재생을 일시정지합니다.
    */
    public void Pause(bool setIsplay = false)
    {
        if (_PlayCoroutine != null)
        {
            StopCoroutine(_PlayCoroutine);
            _PlayCoroutine = null;
            if (setIsplay) m_IsPlay = false;
        }
    }

    /**
    * @brief 로딩된 모션파일의 재생,정지 토글 함수.
    */
    public void PlayAndPause()
    {
        if (_PlayCoroutine != null)
        {
            m_IsPlay = false;
            StopCoroutine(_PlayCoroutine);
            _PlayCoroutine = null;
        }
        else
        {
            m_IsPlay = true;
            _PlayCoroutine = PlayStart();
            StartCoroutine(_PlayCoroutine);
        }
    }

    /**
    * @brief 로딩된 모션파일의 재생을 정지하고 프레임을 0으로 돌립니다.
    */
    public void Stop()
    {
        if (_PlayCoroutine != null)
        {
            m_IsPlay = false;
            StopCoroutine(_PlayCoroutine);
            _PlayCoroutine = null;
            SetFrame(0);
        }
    }

    /**
    * @brief 현재 프레임을 반환합니다.
    */
    public float GetCurrentTime()
    {
        return m_CurrentFrame * m_FrameTime;
    }

    /**
    * @brief 재생 빠르기를 설정합니다. 
    */
    public void SetFrameRatio(bool fast)
    {
        // 1, 2, 4, 8, 16
        if (fast && m_FrameRatio < 16.0f)
        {
            m_FrameRatio *= 2.0f;
        }
        else if (!fast && m_FrameRatio > 1 / 16.0f)
        {
            m_FrameRatio /= 2.0f;
        }

        if (m_IsPlay)
            Play();
    }

    /**
    * @brief 현재프레임을 해당 프레임만큼 앞,뒤로 움직입니다.
    */
    public void Moveframe(bool forward, int frame)
    {
        int newframe = m_CurrentFrame + (forward ? frame : -frame);
        if (newframe < 0 || m_TotalFrames == 0) newframe = 0;
        else if (newframe >= m_TotalFrames) newframe = m_TotalFrames - 1;
        SetFrame(newframe);
        if (m_IsPlay)
            Play();
    }
}
