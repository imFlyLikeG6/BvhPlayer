using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file GraphController.cs
 * @brief 그래프 컨트롤러. 각 씬의 GraphView-Graph에 붙어있습니다.
 */
public class GraphController : MonoBehaviour {

    public UINgraph m_Graph;
    public bool m_isLiveGraph = false;
    public bool m_MainFileIsBVH = true;

    public MotionController m_MotionController;
    public UILabel[] m_RangeLabel;
    public Transform m_SelectedBoneAngle;
    public Transform m_CurrentBar;
    public UIWidget m_FrameWidget;
    public UISprite[] m_ColorUI;

    //0:quat, 1:Euler
    public int m_CurrentGraphIndex;
    public int m_CurrentBoneIndex;

    public Color[] m_GraphColor = { Color.red, Color.yellow, Color.blue, Color.green };
    private string[] m_GraphName = { "x", "y", "z", "w" };

    NGraphDataSeriesXy[] mSeries;

    NGraphDataSeriesXyLiveTransient[] m_LiveSeries;

    public bool m_Init;

    // Use this for initialization
    void Start () {
        m_CurrentBoneIndex = -1;
        m_CurrentGraphIndex = -1;
    }
    
    public void UpdateGraph()
    {
        m_Graph.Update();
    }

    /**
    * @brief 그래프 시크바 위치조절 함수.
    * @param frame  프레임수.
    * @param xPos 시크바 x좌표 로컬포지션.
    */
    public void SetFrame(int frame, float xPos)
    {
        if (m_Init)
        {
            /* 타임라인과 크기가 같기때문에 따로 계산할필요가 없어졌음.
            int totalframe = m_MotionController.m_TotalFrames;
            if (frame > totalframe && frame < 0) return;

            int totalwidth = m_FrameWidget.width;
            int xPos = frame * totalwidth / totalframe;            */
            m_CurrentBar.localPosition = new Vector3(xPos, 0, 0);
            Quaternion CurrentRot = m_MotionController.GetBoneData(m_CurrentBoneIndex)[frame];
            m_SelectedBoneAngle.localRotation = CurrentRot;

        }
    }

    /**
     * @brief 초기화함수.
     */
    public void Clear()
    {
        if(mSeries != null)
        {
            for (int i = 0; i < mSeries.Length; i++)
                m_Graph.removeDataSeries(mSeries[i]);
        }

        if (m_LiveSeries != null)
        {
            for (int i = 0; i < m_LiveSeries.Length; i++)
                m_Graph.removeDataSeries(m_LiveSeries[i]);
        }
        if(m_CurrentBar) m_CurrentBar.localPosition = new Vector3(0, 0, 0);
        m_Graph.deleteContainer();
        m_LiveSeries = null;
        mSeries = null;
        m_Init = false;
    }

    /**
     * @brief 그래프의 센서 값 셋팅
     * @param idx    ngui popuplist의 data를 object로 받습니다.
     */
    public void SetBoneIndexObj(object idx)
    {
        SetBoneIndex((int)idx);
    }

    /**
     * @brief 그래프의 센서 값 셋팅
     * @param idx   센서 인덱스값.
     */
    public void SetBoneIndex(int idx)
    {
        m_CurrentBoneIndex = idx;
        if (m_isLiveGraph) SetLiveGraph();
        else SetGraph();
    }

    /**
     * @brief 그래프의 종류 값 셋팅.
     * @param sidx   인덱스의 이름 Quat, Euler.
     * @param on   true:선택 false:선택취소.
     */
    public void SetGraphIndexString(string sidx, bool on)
    {
        if (!on) return;

        switch(sidx)
        {
            case "Quat":
                SetGraphIndex(0);
                break;
            case "Euler":
                SetGraphIndex(1);
                break;
        }
    }

    /**
     * @brief 그래프의 종류 값 셋팅.
     * @param idx   그래프 종류 인덱스 값.
     */
    public void SetGraphIndex(int idx)
    {
        m_CurrentGraphIndex = idx;
        if (m_isLiveGraph) SetLiveGraph();
        else SetGraph();
    }

    /**
     * @brief m_CurrentGraphIndex와 m_CurrentBoneIndex를 토대로 그래프를 그립니다.
     */
    public void SetGraph()
    {
        Clear();
        if(m_MotionController != null)
        {
            if (m_MotionController.m_GlobalQuatData != null
                && m_CurrentBoneIndex >= 0
                && m_CurrentGraphIndex >= 0)
            {
                if (m_CurrentGraphIndex == 0)
                    SetQuatData(m_MotionController.GetBoneData(m_CurrentBoneIndex));
                else if (m_CurrentGraphIndex == 1)
                    SetEulerData(m_MotionController.GetBoneDataV(m_CurrentBoneIndex), 180, -180);

            }
        }
    }

    /**
     * @brief vector값에 대한 그래프를 그립니다.
     * @param VecList    벡터데이터.
     * @param max    y축 최대값.
     * @param min    y축 최소값.
     */
    void SetEulerData(Vector3[] VecList, float max, float min)
    {
        if (VecList == null)
            return;

        if (!m_Graph.enabled) m_Graph.enabled = true;
        m_RangeLabel[0].text = string.Format("{0:0}", max);
        m_RangeLabel[1].text = "0";
        m_RangeLabel[2].text = string.Format("{0:0}", min);
        m_Graph.setRanges(0, VecList.Length, min, max);

        int unit = 10000;
        int xCount = VecList.Length / unit + 1;
        int yCount = 3;
        mSeries = null;
        mSeries = new NGraphDataSeriesXy[xCount * yCount];

        int length = VecList.Length;
        int j = 0;
        while (length > 0)
        {
            int count = length < unit ? length : unit;

            List<Vector2>[] datas = new List<Vector2>[yCount];
            for (int i = 0; i < yCount; i++)
            {
                datas[i] = new List<Vector2>();
            }
            for (int i = 0; i < count; i++)
            {
                Vector3 vec = VecList[j * unit + i];
                datas[0].Add(new Vector2(j * unit + i, CheckRange(vec.x, min, max)));
                datas[1].Add(new Vector2(j * unit + i, CheckRange(vec.y, min, max)));
                datas[2].Add(new Vector2(j * unit + i, CheckRange(vec.z, min, max)));
            }

            for (int i = 0; i < yCount; i++)
            {
                mSeries[j * yCount + i] = m_Graph.addDataSeries<NGraphDataSeriesXy>(m_GraphName[i], m_GraphColor[i]);
                mSeries[j * yCount + i].Data = datas[i];
                mSeries[j * yCount + i].Reveal = 2.0f;
                mSeries[j * yCount + i].PlotStyle = NGraphDataSeriesXy.Style.Line;
                mSeries[j * yCount + i].PlotThickness = 1f;
            }
            length -= unit;
            j++;
        }

        m_Init = true;
    }

    /**
     * @brief Quaternion값에 대한 그래프를 그립니다. max, min은 자동으로 1,-1로 설정.
     * @param quatList    쿼터니언 데이터.
     */
    void SetQuatData(Quaternion[] quatList)
    {
        if (!m_Graph.enabled) m_Graph.enabled = true;

        m_Graph.setRanges(0, quatList.Length, -1, 1);
        m_RangeLabel[0].text = "1";
        m_RangeLabel[1].text = "0";
        m_RangeLabel[2].text = "-1";

        int unit = 10000;
        int xCount = quatList.Length / unit + 1;
        int yCount = 4;
        mSeries = null;
        mSeries = new NGraphDataSeriesXy[xCount * yCount];

        int length = quatList.Length;
        int j = 0;
        while (length > 0)
        {
            int count = length < unit ? length : unit;

            List<Vector2>[] datas = new List<Vector2>[yCount];
            for (int i = 0; i < yCount; i++)
            {
                datas[i] = new List<Vector2>();
            }
            for (int i = 0; i < count; i++)
            {
                Quaternion vec = quatList[j * unit + i];
                datas[0].Add(new Vector2(j * unit + i, CheckRange(vec.x, -1f, 1f)));
                datas[1].Add(new Vector2(j * unit + i, CheckRange(vec.y, -1f, 1f)));
                datas[2].Add(new Vector2(j * unit + i, CheckRange(vec.z, -1f, 1f)));
                datas[3].Add(new Vector2(j * unit + i, CheckRange(vec.w, -1f, 1f)));
            }

            for (int i = 0; i < yCount; i++)
            {
                mSeries[j * yCount + i] = m_Graph.addDataSeries<NGraphDataSeriesXy>(m_GraphName[i], m_GraphColor[i]);
                mSeries[j * yCount + i].Data = datas[i];
                mSeries[j * yCount + i].Reveal = 2.0f;
                mSeries[j * yCount + i].PlotStyle = NGraphDataSeriesXy.Style.Line;
                mSeries[j * yCount + i].PlotThickness = 1f;
            }
            length -= unit;
            j++;
        }

        m_Init = true;
    }

    /**
     * @brief m_CurrentGraphIndex와 m_CurrentBoneIndex를 토대로 실시간 그래프를 셋팅합니다.
     */
    void SetLiveGraph()
    {
        Clear();
        if (m_CurrentGraphIndex >= 0)
        {
            if (m_CurrentGraphIndex == 0)
            {
                m_Graph.setRanges(0, 10, -1, 1);
                m_RangeLabel[0].text = "1";
                m_RangeLabel[1].text = "0";
                m_RangeLabel[2].text = "-1";
                m_ColorUI[3].gameObject.SetActive(true);
                m_LiveSeries = new NGraphDataSeriesXyLiveTransient[4];
                for (int i = 0; i < m_LiveSeries.Length; i++)
                {
                    m_LiveSeries[i] = m_Graph.addDataSeries<NGraphDataSeriesXyLiveTransient>(m_GraphName[i], m_GraphColor[i]);
                    m_LiveSeries[i].UpdateRate = 0.01f;
                }
                m_Init = true;
            }
            else if (m_CurrentGraphIndex > 0)
            {
                if(m_CurrentGraphIndex == 1)
                {
                    m_Graph.setRanges(0, 10, -180, 180);
                    m_RangeLabel[0].text = "180";
                    m_RangeLabel[1].text = "0";
                    m_RangeLabel[2].text = "-180";
                }
                else if (m_CurrentGraphIndex == 2)
                {
                    m_Graph.setRanges(0, 10, -500, 500);
                    m_RangeLabel[0].text = "500";
                    m_RangeLabel[1].text = "0";
                    m_RangeLabel[2].text = "-500";
                }
                else if (m_CurrentGraphIndex == 3)
                {
                    m_Graph.setRanges(0, 10, -6, 6);
                    m_RangeLabel[0].text = "6";
                    m_RangeLabel[1].text = "0";
                    m_RangeLabel[2].text = "-6";
                }
                else if (m_CurrentGraphIndex == 4)
                {
                    m_Graph.setRanges(0, 10, -80, 80);
                    m_RangeLabel[0].text = "80";
                    m_RangeLabel[1].text = "0";
                    m_RangeLabel[2].text = "-80";
                }
                m_ColorUI[3].gameObject.SetActive(false);
                m_LiveSeries = new NGraphDataSeriesXyLiveTransient[3];
                for (int i = 0; i < m_LiveSeries.Length; i++)
                {
                    m_LiveSeries[i] = m_Graph.addDataSeries<NGraphDataSeriesXyLiveTransient>(m_GraphName[i], m_GraphColor[i]);
                    m_LiveSeries[i].UpdateRate = 0.01f;
                }
                m_Init = true;
            }
        }

    }

    /**
     * @brief 실시간 그래프를 받은 data로 업데이트합니다.
     * @param data    vector혹은 Quaternion을 object로 받습니다.
     */
    public void UpdateLiveData(object data)
    {
        if(m_Init)
        {
            if (m_CurrentGraphIndex == 0)
            {
                Quaternion quat = (Quaternion)data;
                m_LiveSeries[0].UpdateValue = CheckRange(quat.x, m_Graph.YRange.x, m_Graph.YRange.y);
                m_LiveSeries[1].UpdateValue = CheckRange(quat.y, m_Graph.YRange.x, m_Graph.YRange.y);
                m_LiveSeries[2].UpdateValue = CheckRange(quat.z, m_Graph.YRange.x, m_Graph.YRange.y);
                m_LiveSeries[3].UpdateValue = CheckRange(quat.w, m_Graph.YRange.x, m_Graph.YRange.y);
                m_SelectedBoneAngle.rotation = quat;
            }
            else if (m_CurrentGraphIndex > 0)
            {
                Vector3 vec = (Vector3)data;
                m_LiveSeries[0].UpdateValue = CheckRange(vec.x, m_Graph.YRange.x, m_Graph.YRange.y);
                m_LiveSeries[1].UpdateValue = CheckRange(vec.y, m_Graph.YRange.x, m_Graph.YRange.y);
                m_LiveSeries[2].UpdateValue = CheckRange(vec.z, m_Graph.YRange.x, m_Graph.YRange.y);
                m_SelectedBoneAngle.rotation = Quaternion.Euler(vec);
            }
        }
    }

    /**
     * @brief 해당 범위를 넘지않는 값을 반환합니다.
     * @param data    변환할 데이터.
     * @param min    최대값.
     * @param max    최소값.
     * @return 계산된 float값.
     */
    public float CheckRange(float data, float min, float max)
    {
        if (data < min)
            data = min;
        else if (data > max)
            data = max;

        return data;
    }
}
