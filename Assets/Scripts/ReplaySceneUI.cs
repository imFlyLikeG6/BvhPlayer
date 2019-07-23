using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Crosstales.FB;

/**
 * @file ReplaySceneUI.cs
 * @brief bvhPlayer씬의 전체 UI관리 클래스. UI Root에 붙어있습니다.
 */
public class ReplaySceneUI : MonoBehaviour
{
    public Camera m_UICamera;
    [Header("MotionFile")]
    public UILabel m_MotionPathLB;
    public UILabel m_FrameLB;
    public UILabel m_FpsLB;
    public UILabel m_MotionTimeLB;
    public string m_MotionFilePath { get; set; }
    public string m_SavedFilePath { get; set; }
    private string m_TmpFilePath;
    public BvhController m_BvhController;
    public MotionController m_MotionController;
    private int m_TotalFrame;
    private int m_CurrentFrame;
    private int m_Fps;
    private float m_TotalMotionTime;
    private int m_OldFrame;
    private bool m_NoChangeMotionFile;

    public UILabel m_MotionFileTitle;
    public bool m_MainFileIsBVH;
    
    [Header("SensorData")]
    public UIPopupList[] m_SensorList;
    public UILabel[] m_SensorDataLB;
    public SensorDataUI[] m_SensorDataUI;
    public UILabel m_CogDataLB;
    
    [Header("Graph")]
    public UIPopupList[] m_GraphList;
    public UILabel[] m_SelectedGraphLB;
    public GraphController[] m_GraphController;

    [Header("TimeLine")]
    public UIScrollBar m_TimeLineScrollBar;
    public UIScrollView m_TimeLineScrollView;
    public UIPanel m_TimeLineScrollPanel;
    public UILabel m_CurrentTimeLB;
    public UIWidget m_TimeLineRoot;
    public UIButton m_PlayBtn;
    public Transform m_CurrentBar;
    public UILabel m_RatioLB;
    public TimelineUnitActive m_TimeLineUnit;
    public UIWidget m_TimeSelectedBar;
    private int m_TimeLineDragStart;
    private int m_TimeLineDragEnd;
    private float m_TimeLineScale = 1;
    private float m_ScrollOffsetX = 15;
    private int m_SkipFrame = 5;

    private bool m_SelectDragOnLeft = false;
    private bool m_SelectDragOnRight = false;

    private Vector2 m_WindowSize;
    //public UILabel m_DebugTest;

    public int Fps
    {
        get { return m_Fps; }
        set
        {
            m_Fps = value;
            m_FpsLB.text = m_Fps.ToString();
        }
    }

    public float TotalMotionTime
    {
        get { return m_TotalMotionTime; }
        set
        {
            m_TotalMotionTime = value;
            m_MotionTimeLB.text = TimeToString(m_TotalMotionTime);
        }
    }

    void Start()
    {
        m_WindowSize = new Vector2(Screen.width, Screen.height);
        m_OldFrame = m_CurrentFrame;
        m_MotionController = m_BvhController;
        m_TmpFilePath = Application.temporaryCachePath + "/ETRI_TempFile";
        m_MotionFilePath = "";

        TimeLineScale(-1);
        m_TimeLineScrollView.ResetPosition();
    }

    void Update()
    {
        //화면 크기관련
        if (m_WindowSize.x != Screen.width || m_WindowSize.y != Screen.height)
        {
            m_WindowSize.x = Screen.width;
            m_WindowSize.y = Screen.height;
            TimeLineScale(m_TimeLineScale);
        }

        //프레임 업데이트
        m_CurrentFrame = m_MotionController.m_CurrentFrame;
        if (m_OldFrame != m_CurrentFrame)
        {
            m_OldFrame = m_CurrentFrame;
            UpdateMotionFrame();
        }

        if (m_SelectDragOnLeft) DragingSelect(true);
        else if (m_SelectDragOnRight) DragingSelect(false);

        //단축키
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.R))
                RemoveMotion();
            else if (Input.GetKeyDown(KeyCode.S))
                SaveMotion();

            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0)
            {
                if (delta > 0) TimeLineZoomIn();
                else TimeLineZoomOut();
            }
        }

    }

    /**
     * @brief MotionController에 currentframe으로 ui를 셋팅합니다.
     */
    public void UpdateMotionFrame()
    {
        if (m_TotalFrame > 0)
        {
            m_FrameLB.text = string.Format("{0} / {1}", m_CurrentFrame + 1, m_TotalFrame);
            int xPos = m_CurrentFrame * m_TimeLineRoot.width / m_TotalFrame;
            m_CurrentBar.localPosition = new Vector3(xPos, 0, 0);
            m_CurrentTimeLB.text = TimeToString(m_CurrentFrame * m_MotionController.m_FrameTime);
            foreach (GraphController gc in m_GraphController)
            {
                if (gc.m_Init) gc.SetFrame(m_CurrentFrame, xPos);
            }
            foreach (SensorDataUI sd in m_SensorDataUI)
            {
                sd.SetCurrentFrame(m_CurrentFrame);
            }
        }
    }

    private void LateUpdate()
    {
        m_TimeLineScrollView.UpdateScrollbars();
    }

    /**
     * @brief 초기화 함수.
     */
    public void Clear()
    {
        m_TimeSelectedBar.transform.localPosition = Vector3.zero;
        m_TimeSelectedBar.width = 10;
        m_TimeSelectedBar.gameObject.SetActive(false);
        m_CurrentBar.transform.localPosition = Vector3.zero;

        TimeLineScale(-1);
        foreach (GraphController gc in m_GraphController)
        {
            gc.Clear();
        }
        foreach (SensorDataUI sd in m_SensorDataUI)
        {
            sd.Clear();
        }
        foreach (UIPopupList list in m_SensorList)
        {
            list.Clear();
        }
        foreach (UIPopupList list in m_GraphList)
        {
            list.Clear();
        }
        m_BvhController.Clear();
        m_MotionFileTitle.text = "";
    }

    private void OnDestroy()
    {
        Clear();
        if (System.IO.File.Exists(m_TmpFilePath + ".bvh"))
            System.IO.File.Delete(m_TmpFilePath + ".bvh");
    }

    /**
     * @brief 시간을 string형태로 변환합니다.
     */
    public string TimeToString(float seconds)
    {
        int ms = (int)(seconds * 1000) % 1000;
        int hour = (int)seconds / 3600;
        seconds %= 3600;
        int min = (int)seconds / 60;
        int sec = (int)seconds % 60;

        return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", hour, min, sec, ms);
        //return string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
    }

    /**
     * @brief 타임라인을 확대합니다.
     */
    public void TimeLineZoomIn()
    {
        //if (m_TimeLineRoot.width >= m_TotalFrame) return;
        if (m_TimeLineRoot.width >= 90000) return;
        TimeLineScale(m_TimeLineScale * 4 / 3);
    }

    /**
     * @brief 타임라인을 축소합니다.
     */
    public void TimeLineZoomOut()
    {
        if (m_TimeLineScrollPanel.width >= m_TimeLineRoot.width) return;
        TimeLineScale(m_TimeLineScale * 2 / 3);
    }

    /**
     * @brief 타임라인 확대/축소를 관리합니다.
     * @param scale     셋팅할 스케일. -1: 화면에 꽉차게 스케일링.
     */
    public void TimeLineScale(float scale)
    {
        int width = (int)(scale * m_TotalFrame);
        if (width <= m_TimeLineScrollPanel.width)
            width = (int)m_TimeLineScrollPanel.width;
        if (width > 90000) width = 90000;

        //else if (scale > 2)
        //    width = m_TotalFrame * 2;

        //m_TimeLineScale = scale;

        if (m_TotalFrame > 0)
        {
            m_TimeLineScale = (float)width / m_TotalFrame;
            float oldPos = m_TimeLineScrollPanel.transform.localPosition.x;
            float oldBarPos = m_CurrentBar.localPosition.x;
            float nowBarPos = m_CurrentFrame * width / m_TotalFrame;
            float newPos = oldPos - (nowBarPos - oldBarPos);

            //print("width" + width + ",newPos" + newPos + ", m_TimeLineScrollPanel.width" + m_TimeLineScrollPanel.width + "<" +m_ScrollOffsetX);
            //m_ScrollOffsetX = m_TimeLineScrollView.transform.localPosition.x;
            //뒤쪽이 남을때
            if (width + newPos < m_TimeLineScrollPanel.width)
                newPos += (m_TimeLineScrollPanel.width - (width + newPos)) + m_ScrollOffsetX;
            else if (newPos > m_ScrollOffsetX) newPos = m_ScrollOffsetX; //앞이 남을때

            //print(newPos);
            m_TimeLineScrollPanel.transform.localPosition = new Vector3(newPos, 0, 0);
            m_TimeLineScrollPanel.clipOffset = new Vector2(-newPos, 0);

        }
        else
        {
            m_TimeLineScrollPanel.transform.localPosition = new Vector3(15, 0, 0);
            m_TimeLineScrollPanel.clipOffset = new Vector2(-15, 0);
        }
        print("width : " + width + ", m_TimeLineScale : " + m_TimeLineScale + ", scale :" + (float)m_TimeLineScrollPanel.width / m_TotalFrame);
        m_TimeLineRoot.width = width;
        //m_TimeLineScrollView.ResetPosition();
        foreach (GraphController gc in m_GraphController)
        {
            gc.UpdateGraph();
        }

        if (m_TimeSelectedBar.gameObject.activeSelf && m_TotalFrame != 0)
        {
            int xPos = m_TimeLineDragEnd * m_TimeLineRoot.width / m_TotalFrame;
            int startPos = m_TimeLineDragStart * m_TimeLineRoot.width / m_TotalFrame;
            m_TimeSelectedBar.transform.localPosition = new Vector3(startPos, 0, 0);
            m_TimeSelectedBar.width = xPos - (int)m_TimeSelectedBar.transform.localPosition.x;
        }

        UpdateMotionFrame();
    }
    
    /**
     * @brief 오픈할 모션파일에 대한 선택 브라우저를 띄우고, 선택한 파일을 표시합니다.
     */
    public void OpenMotionFile()
    {
        ExtensionFilter[] extensions = new ExtensionFilter[1];
        extensions[0] = new ExtensionFilter(string.Empty, "bvh");
        //m_MotionFilePath = FileBrowser.OpenSingleFile("Open File", "", extensions);
        m_MotionPathLB.text = FileBrowser.OpenSingleFile("Open File", "", extensions);
    }

    /**
     * @brief 타임라인에 마우스드래그 이벤트 발생시 관리함수.
     * @param leftdown  true:현재프레임 설정. false:선택영역 설정.
     */
    public void TimelineMouseDrag(bool leftdown)
    {
        int frame = SetMouseFrame();
        if (m_TotalFrame > 0 && !leftdown)
        {
            int xPos = frame * m_TimeLineRoot.width / m_TotalFrame;
            int startPos = m_TimeLineDragStart * m_TimeLineRoot.width / m_TotalFrame;
            if (frame > m_TimeLineDragStart)
            {
                m_TimeSelectedBar.transform.localPosition = new Vector3(startPos, 0, 0);
                m_TimeSelectedBar.width = xPos - (int)m_TimeSelectedBar.transform.localPosition.x;
            }
            else
            {
                m_TimeSelectedBar.transform.localPosition = new Vector3(xPos, 0, 0);
                m_TimeSelectedBar.width = startPos - (int)m_TimeSelectedBar.transform.localPosition.x;
            }
            m_TimeLineDragEnd = frame;
            if (!m_TimeSelectedBar.gameObject.activeSelf) m_TimeSelectedBar.gameObject.SetActive(true);
        }
    }

    /**
     * @brief 타임라인에 마우스다운 이벤트 발생시 관리함수.
     * @param leftdown  true:현재프레임 설정. false:선택영역 취소.
     */
    public void TimelineMouseDown(bool leftdown)
    {
        int frame = SetMouseFrame();
        if (!leftdown)
        {
            m_TimeSelectedBar.gameObject.SetActive(false);
            m_TimeLineDragStart = frame;
        }
    }

    /**
     * @brief 타임라인 위의 마우스 위치에 따른 현재 프레임 계산함수. frame값을 리턴합니다.
     */
    int SetMouseFrame()
    {
        if (m_MotionController.m_IsPlay) m_MotionController.Pause(false);
        Vector2 mouseScreenPosition = m_UICamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = m_TimeLineRoot.transform.InverseTransformPoint(mouseScreenPosition);
        int frame = (int)pos.x * m_TotalFrame / m_TimeLineRoot.width;
        if (frame < 0) frame = 0;
        else if (frame >= m_TotalFrame) frame = m_TotalFrame - 1;
        if (m_TotalFrame > 0)
        {
            m_MotionController.SetFrame(frame);
        }
        return frame;
    }

    /**
     * @brief 타임라인에 마우스업 이벤트 발생시 관리함수.
     * @param leftdown  true:재생 중이였으면 계속해서 재생합니다. false:선택영역 설정.
     */
    public void TimelineMouseUp(bool leftdown)
    {
        if (m_MotionController.m_IsPlay) m_MotionController.Play();
        if(m_TimeSelectedBar.gameObject.activeSelf && !leftdown)
        {
            m_TimeLineDragEnd = m_CurrentFrame;
            if (m_TimeLineDragEnd < m_TimeLineDragStart)
            {
                int tmp = m_TimeLineDragStart;
                m_TimeLineDragStart = m_TimeLineDragEnd;
                m_TimeLineDragEnd = tmp;
            }
        }
    }
    
    /**
     * @brief 모션파일을 로딩합니다. 같은 이름의 csv,bvh를 모두 로딩합니다.
     * @param path  모션파일 경로.
     */
    public void LoadMotionFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        Debug.Log("Load Motion File => " + path);
        Clear();

        m_MotionController.Stop();

        string bvhPath = path.Substring(0, path.LastIndexOf('.'));
        bvhPath = bvhPath + ".bvh";

        m_MainFileIsBVH = System.IO.File.Exists(bvhPath);

        if (!m_NoChangeMotionFile)
        {
            m_MotionFilePath = path;
            if (System.IO.File.Exists(m_TmpFilePath + ".bvh"))
                System.IO.File.Delete(m_TmpFilePath + ".bvh");
            if (System.IO.File.Exists(m_TmpFilePath + ".csv"))
                System.IO.File.Delete(m_TmpFilePath + ".csv");
        }
        int slash = Mathf.Max(m_MotionFilePath.LastIndexOf('/'), m_MotionFilePath.LastIndexOf('\\'));
        m_MotionFileTitle.text = m_MotionFilePath.Substring(slash + 1);
        m_SavedFilePath = path;
        m_BvhController.LoadBvhData(bvhPath);

        m_MotionController = m_BvhController;                
        m_MotionController.m_IsBvhController = m_MainFileIsBVH;

        m_TotalFrame = m_MotionController.m_TotalFrames;
        TotalMotionTime = m_MotionController.m_TotalFrames * m_MotionController.m_FrameTime;
        Fps = (int)(1 / m_MotionController.m_FrameTime);
        
        AddBoneList();

        m_CurrentBar.localPosition = new Vector3(0, 0, 0);
        m_TimeLineRoot.width = (int)m_TimeLineScrollPanel.width;
        m_TimeLineRoot.transform.localPosition = Vector3.zero;
        m_TimeLineScale = m_TimeLineScrollPanel.width / m_TotalFrame;
        TimeLineScale(m_TimeLineScale);

        m_TimeLineUnit.MakeTimeLineUnitText(m_TotalFrame);
        m_TimeLineScrollBar.barSize = 1;
        m_TimeLineScrollView.ResetPosition();

        foreach (SensorDataUI sd in m_SensorDataUI)
        {
            sd.m_MotionController = m_MotionController;
        }

        foreach (GraphController gc in m_GraphController)
        {
            gc.m_MotionController = m_MotionController;
        }

        m_MotionController.SetFrame(0);
        m_NoChangeMotionFile = false;
    }
    
    /**
     * @brief 로딩된 모션파일을 재생합니다.
     */
    public void Play()
    {
        m_MotionController.PlayAndPause();
        if (m_MotionController.m_IsPlay)
            SetPlayBtn("pause");
        else
            SetPlayBtn("play");
    }

    /**
     * @brief play버튼의 sprite를 설정합니다. 
     */
    public void SetPlayBtn(string spriteName)
    {
        m_PlayBtn.normalSprite = spriteName;
    }

    /**
     * @brief 로딩된 모션파일을 정지합니다.
     */
    public void Stop()
    {
        m_MotionController.Stop();
        m_PlayBtn.normalSprite = "play";
    }

    /**
     * @brief 재생의 빠르기를 현재보다 빠르게 설정합니다.
     */
    public void SetFrameRatioFast()
    {
        m_MotionController.SetFrameRatio(true);
        
        m_RatioLB.text = string.Format("{0:F1}x", m_MotionController.m_FrameRatio);
    }

    /**
     * @brief 재생의 빠르기를 현재보다 느리게 설정합니다.
     */
    public void SetFrameRatioSlow()
    {
        m_MotionController.SetFrameRatio(false);
        m_RatioLB.text = m_MotionController.m_FrameRatio + "x";
    }

    /**
     * @brief 현재 프레임을 m_SkipFrame만큼 앞으로 이동합니다.
     */
    public void MoveForward()
    {
        m_MotionController.Moveframe(true, m_SkipFrame);
    }

    /**
     * @brief 현재 프레임을 m_SkipFrame만큼 뒤로 이동합니다.
     */
    public void MoveBackward()
    {
        m_MotionController.Moveframe(false, m_SkipFrame);
    }

    /**
     * @brief 각 popuplist에 로딩한 모션파일의 센서,관절 리스트를 추가합니다.
     */
    public void AddBoneList()
    {
        foreach (UIPopupList list in m_SensorList)
        {
            list.Clear();
        }
        foreach (UIPopupList list in m_GraphList)
        {
            list.Clear();
        }
        int count = 0;
        if (m_BvhController.m_DataInit)
        {
            for (int i = 0; i < m_BvhController.m_OffsetDatas.Count; i++)
            {
                string name = m_BvhController.m_OffsetDatas[i].name;
                if (name.Contains("Site")) continue;
                
                Color color = Color.white;

                foreach (UIPopupList list in m_SensorList)
                {
                    list.AddItem(name, count, color);
                }
                foreach (UIPopupList list in m_GraphList)
                {
                    list.AddItem(name, count, color);
                }
                count++;
            }
        }
    }

    /**
     * @brief 선택영역의 위 flag를 잡고 드래그했을때 선택영역을 설정합니다.
     */
    public void DragingSelect(bool left)
    {
        int frame = SetMouseFrame();
        if (m_TotalFrame > 0)
        {
            if (left && frame < m_TimeLineDragEnd)
            {
                int startPos = frame * m_TimeLineRoot.width / m_TotalFrame;
                int endPos = m_TimeLineDragEnd * m_TimeLineRoot.width / m_TotalFrame;
                m_TimeSelectedBar.transform.localPosition = new Vector3(startPos, 0, 0);
                m_TimeSelectedBar.width = endPos - (int)startPos;
                m_TimeLineDragStart = frame;
            }
            else if (!left && frame > m_TimeLineDragStart)
            {
                int startPos = m_TimeLineDragStart * m_TimeLineRoot.width / m_TotalFrame;
                int endPos = frame * m_TimeLineRoot.width / m_TotalFrame;
                m_TimeSelectedBar.width = endPos - (int)startPos;
                m_TimeLineDragEnd = frame;
            }            
        }
    }

    /**
     * @brief 선택영역의 위 flag를 mouse down했을 때 발생하는 함수.
     */
    public void SelectedBarDragOn(string left)
    {
        if (left.Contains("left"))
            m_SelectDragOnLeft = true;
        else
            m_SelectDragOnRight = true;
    }

    /**
     * @brief 선택영역의 위 flag를 mouse up했을 때 발생하는 함수.
     */
    public void SelectedBarDragOff(string left)
    {
        if (left.Contains("left"))
        {
            m_SelectDragOnLeft = false;
            DragingSelect(true);
        }
        else
        {
            m_SelectDragOnRight = false;
            DragingSelect(false);
        }
    }

    /**
     * @brief MoveForward,MoveBackward에서 스킵할 프레임을 설정합니다.
     */
    public void SetSkipFrame(string num)
    {
        int.TryParse(num, out m_SkipFrame);
    }

    /**
     * @brief 선택한 영역의 모션 데이터를 삭제합니다.
     */
    public void RemoveMotion()
    {
        if (m_TimeSelectedBar.gameObject.activeSelf)
        {
            if (m_TimeLineDragStart == 0 && m_TimeLineDragEnd == m_TotalFrame - 1) return;
            //string savePath = FileBrowser.SaveFile("Save File", "", "untitled", "bvh");
            m_NoChangeMotionFile = true;
            string ext = m_SavedFilePath.Contains(".bvh") ? ".bvh" : ".csv";
            m_SavedFilePath = m_TmpFilePath + ext;
            if (m_SavedFilePath == string.Empty) return;

            //int i = 1;
            string tmp = m_SavedFilePath.Substring(0, m_SavedFilePath.LastIndexOf('.') + 1);

            bool reSave = false;
            if (System.IO.File.Exists(tmp + "bvh"))
            {
                tmp += "_1";
                reSave = true;
            }
            m_BvhController.SaveBvhFile(tmp + "bvh", m_TimeLineDragStart, m_TimeLineDragEnd, true);

            if(reSave)
            {
                string orignal = m_SavedFilePath.Substring(0, m_SavedFilePath.LastIndexOf('.') + 1);
                if(System.IO.File.Exists(orignal + "bvh"))
                {
                    System.IO.File.Delete(orignal + "bvh");
                    System.IO.File.Copy(tmp + "bvh", orignal + "bvh");
                    System.IO.File.Delete(tmp + "bvh");
                }
            }


            LoadMotionFile(m_SavedFilePath);
        }
        else
            Debug.Log("Please select a range to save.");
    }

    /**
     * @brief 모션데이터를 저장합니다. 선택영역이 있을 때, 그 영역만 저장합니다.
     */
    public void SaveMotion()
    {
        if (m_MotionFilePath == string.Empty) return;
        int i = 1;
        string path = m_MotionFilePath.Substring(0, m_MotionFilePath.LastIndexOf('.'));
        if (path == string.Empty) return;
        while (true)
        {
            if(!System.IO.File.Exists(path + "_" + i.ToString() + ".bvh"))
            {
                path = path + "_" + i.ToString();
                break;
            }
            i++;
        }
        int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
        string dir = path.Substring(0, slash);
        string name = path.Substring(slash + 1);
        string savePath = FileBrowser.SaveFile("Save File", dir, name, "bvh");
        if (savePath == string.Empty) return;
        string tmp = savePath.Substring(0, savePath.LastIndexOf('.') + 1);

        if (m_TimeSelectedBar.gameObject.activeSelf)
        {
            m_BvhController.SaveBvhFile(tmp + "bvh", m_TimeLineDragStart, m_TimeLineDragEnd, false);
        }
        else
        {
            m_BvhController.SaveBvhFile(tmp + "bvh", 0, m_TotalFrame - 1, false);
        }

        //m_TimeLineScrollView.ResetPosition();
    }

    /**
     * @brief UIInput에 있는 프레임으로 선택영역을 설정합니다.
     */
    public void SetSelectSection(UIInput start, UIInput end)
    {
        if (m_TotalFrame <= 0) return;

        int startFrame, endFrame;
        int.TryParse(start.value, out startFrame);
        int.TryParse(end.value, out endFrame);

        if (startFrame <= 0) startFrame = 1;
        if (endFrame <= 0) endFrame = 1;
        if (startFrame > m_TotalFrame) startFrame = m_TotalFrame;
        if (endFrame > m_TotalFrame) endFrame = m_TotalFrame;
        if (endFrame < startFrame) endFrame = startFrame;
        start.value = string.Format("{0}", startFrame);
        end.value = string.Format("{0}", endFrame);

        m_TimeLineDragStart = startFrame - 1;
        m_TimeLineDragEnd = endFrame - 1;

        int startPos = m_TimeLineDragStart * m_TimeLineRoot.width / m_TotalFrame;
        int endPos = m_TimeLineDragEnd * m_TimeLineRoot.width / m_TotalFrame;
        m_TimeSelectedBar.transform.localPosition = new Vector3(startPos, 0, 0);
        m_TimeSelectedBar.width = endPos - (int)startPos;

        if (!m_TimeSelectedBar.gameObject.activeSelf) m_TimeSelectedBar.gameObject.SetActive(true);

    }

    /**
     * @brief vStart의 vEnd에 대한 각도 값을 float으로 반환합니다.
     */
    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        //Vector3 v = vEnd - vStart;
        //float re = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + 90;
        float re = Mathf.Acos(Vector3.Dot(vStart, vEnd)) * Mathf.Rad2Deg;
        if (re > 180) re = 360 - re;
        return re;
    }

    public void Menu_MotionFileLoad()
    {
        OpenMotionFile();
        LoadMotionFile(m_MotionPathLB.text);
    }

}
