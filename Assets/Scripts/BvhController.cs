using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * @file BvhController.cs
 * @brief New_3DViewer_Replay씬에서 쓰이는 bvh컨트롤러. UI Root에 붙어있습니다. 
 */

public class BvhController : MotionController
{

    public struct OffsetData
    {
        public string name;
        public Vector3 offset;
        public int flag; // 0:root 1:head 2:Lhand 3:Rhand 4:Llag 5:Rleg
    }

    public int m_IndexX;
    public int m_IndexY;
    public int m_IndexZ;
    public List<int> m_FlagCount = new List<int>();
    public List<OffsetData> m_OffsetDatas = new List<OffsetData>();
    public int m_ChanaelCount;
    private string m_BvhPath;


    /**
     * @brief 초기화함수.
     */
    public override void Clear()
    {
        base.Clear();

        if (m_OffsetDatas != null)
            m_OffsetDatas.Clear();
        if (m_FlagCount != null)
            m_FlagCount.Clear();
        m_BvhPath = "";
    }

    /**
     * @brief bvh 로딩 함수.
     * @param string file bvh파일 경로.
     * @return 성공시 true, 실패시 false 반환.
     */
    public bool LoadBvhData(string file)
    {
        Clear();
        if (!File.Exists(file)) return false;

        string[] arrLine = File.ReadAllLines(file);
        m_BvhPath = file;

        int i = 0;
        int flag = 0;
        OffsetData tmpData = new OffsetData();
        bool initChannel = false;
        int flagCount = 0;
        m_ChanaelCount = 0;

        //offset Data
        while (i < arrLine.Length)
        {
            string line = arrLine[i];
            string[] lines = line.Split(new[] { " ", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (line.Contains("JOINT"))
            {
                tmpData.name = lines[1];
            }
            else if (line.Contains("End") || line.Contains("ROOT"))
            {
                tmpData.name = lines[1];
            }
            else if (line.Contains("OFFSET"))
            {
                int j = 0;
                for (j = 0; j < lines.Length; j++)
                {
                    if (lines[j] == "OFFSET")
                        break;
                }
                float x = (float)System.Convert.ToDouble(lines[j + 1]);
                float y = (float)System.Convert.ToDouble(lines[j + 2]);
                float z = (float)System.Convert.ToDouble(lines[j + 3]);
                tmpData.offset = new Vector3(x, y, z);
                tmpData.flag = flag;
                m_OffsetDatas.Add(tmpData);
                flagCount++;
                if (tmpData.name.Contains("Site") || tmpData.name.Contains("Hips"))
                {
                    flag++;
                    //int tmpFlag = new int();
                    //tmpFlag = flagCount;
                    //print(tmpFlag);
                    m_FlagCount.Add(flagCount);
                    flagCount = 0;
                }
            }
            else if (line.Contains("CHANNELS"))
            {
                m_ChanaelCount++;
                if (!initChannel)
                {
                    string[] channels = line.Split(new[] { " ", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
                    int num = channels.Length;
                    int minus = num - 3;
                    for (int j = 0; j < num; j++)
                    {
                        if (channels[j].Contains("Xrotation")) m_IndexX = j - minus;
                        else if (channels[j].Contains("Yrotation")) m_IndexY = j - minus;
                        else if (channels[j].Contains("Zrotation")) m_IndexZ = j - minus;
                    }
                    initChannel = true;
                }
            }
            else if (line.Contains("MOTION"))
                break;
            i++;
        }
        
        List<Vector3[]> rotationOrder = new List<Vector3[]>();

        m_JointController.SetOffsetData(m_OffsetDatas, m_FlagCount);
        for (int j = 0; j < m_JointController.m_Joints.Count; j++)
        {
            Vector3[] tmpVec = new Vector3[3];
            Transform tmpTrans = m_JointController.m_Joints[j];
            if(tmpTrans != null)
            {
                tmpVec[m_IndexX] = Vector3.right;
                tmpVec[m_IndexY] = Vector3.up;
                tmpVec[m_IndexZ] = Vector3.forward;
                rotationOrder.Add(tmpVec);
            }
            else
                rotationOrder.Add(null);

        }

        //frame Data
        while (i < arrLine.Length)
        {
            i++;
            string line = arrLine[i];
            if(line.Contains("Frames"))
            {
                string[] lines = line.Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
                m_TotalFrames = System.Convert.ToInt32(lines[1]);
                print("Frames : " + m_TotalFrames);
            }
            else if(line.Contains("Frame Time"))
            {
                string[] lines = line.Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
                m_FrameTime = (float)System.Convert.ToDouble(lines[1]);
                i++;
                print("FrameTime : " + m_FrameTime);
                break;
            }
        }
        
        //motion data
        while (i < arrLine.Length)
        {
            string line = arrLine[i];
            if (line == "") continue;
            string[] lines = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

            float px, py, pz;
            float.TryParse(lines[0], out px);
            float.TryParse(lines[1], out py);
            float.TryParse(lines[2], out pz);
            Vector3 pos = new Vector3(-px,py,pz);
            m_PositionData.Add(pos);

            Quaternion[] arrQuatFromEulerAngle = new Quaternion[m_ChanaelCount];
            Vector3[] arrQuatFromEulerAngleV = new Vector3[m_ChanaelCount];
            for (int j = 3; j < lines.Length; j += 3) // 0~2번째는 hip의 position데이터
            {
                if (lines[j] == "") continue;

                int index = (j / 3) - 1;

                float[] rot = new float[3];
                float.TryParse(lines[j], out rot[0]); //-178 z
                float.TryParse(lines[j + 1], out rot[1]); //47 y
                float.TryParse(lines[j + 2], out rot[2]); //179 

                Vector3 v = new Vector3(rot[m_IndexX], rot[m_IndexY], rot[m_IndexZ]); 
                Quaternion q = new Quaternion();
                if (rotationOrder[index] != null)
                {
                    //right hand -> left hand
                    rot[m_IndexZ] = -rot[m_IndexZ];
                    rot[m_IndexY] = -rot[m_IndexY];

                    q = Quaternion.AngleAxis(rot[0], rotationOrder[index][0]) //z
                        * Quaternion.AngleAxis(rot[1], rotationOrder[index][1]) //y
                        * Quaternion.AngleAxis(rot[2], rotationOrder[index][2]); //x
                }
                arrQuatFromEulerAngle[index] = q;
                arrQuatFromEulerAngleV[index] = v;
            }
            m_LocalQuatData.Add(arrQuatFromEulerAngle);
            m_RotationDataV.Add(arrQuatFromEulerAngleV);
            i++;
        }
        rotationOrder.Clear();
        m_JointController.SetHipsHeight(m_PositionData[0].y);
        m_DataInit = true;
        return true;
    }

    /**
     * @brief 현재 로딩돼있는 bvh 저장 함수.
     * @param string savePath 저장할 파일경로.
     * @param int startframe 지정 영역 시작 프레임 (1프레임 -> startframe = 0)
     * @param int endframe 지정 영역 끝 프레임
     * @param bool remove true:지정영역 삭제후 나머지프레임 저장. false:지정영역 프레임 저장.
     */
    public void SaveBvhFile(string savePath, int startframe, int endframe, bool remove)
    {
        if (!File.Exists(m_BvhPath)) return;

        Debug.Log(string.Format("Save Bvb File [{0}] => [{1}]", m_BvhPath, savePath));
        Debug.Log(string.Format("Save Bvb File startFrame : {0}, EndFrame : {1}", startframe, endframe));
        string[] arrLine = File.ReadAllLines(m_BvhPath);

        if (remove) //선택부분 삭제후 저장
        {
            int saveframe = m_TotalFrames - ( endframe - startframe + 1);
            int newfileLine = arrLine.Length - m_TotalFrames + saveframe;

            using (var stream = File.OpenWrite(savePath))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                int i = 0;
                while (i <= newfileLine - saveframe)
                {
                    if(arrLine[i] != string.Empty)
                        writer.WriteLine(arrLine[i]);
                    i++;
                    if (arrLine[i].Contains("Frames"))
                        break;
                }

                writer.WriteLine(string.Format("Frames : {0}", saveframe));
                i++;
                writer.WriteLine(arrLine[i]);    //frame time
                i++;

                Debug.Log(string.Format("Frames : {0}", saveframe));
                string line = "";
                for (int j = (startframe == 0) ? endframe + 1 : 0; j < m_TotalFrames; j++)
                {
                    line = arrLine[i + j];
                    if(endframe == m_TotalFrames - 1 && j >= startframe - 1) writer.Write(line);
                    else if (j < m_TotalFrames - 1) writer.WriteLine(line);
                    else writer.Write(line);
                    if (j >= startframe - 1 && j < endframe) j = endframe;
                }
                writer.Close();
            }
        }
        else
        {
            int saveframe = endframe - startframe + 1;
            int newfileLine = arrLine.Length - m_TotalFrames + saveframe;

            using (var stream = File.OpenWrite(savePath))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                int i = 0;
                while (i <= newfileLine - saveframe)
                {
                    if (arrLine[i] != string.Empty)
                        writer.WriteLine(arrLine[i]);
                    i++;
                    if (arrLine[i].Contains("Frames"))
                        break;
                }

                writer.WriteLine(string.Format("Frames : {0}", saveframe));
                i++;
                writer.WriteLine(arrLine[i]);    //frame time
                i++;

                Debug.Log(string.Format("Frames : {0}", saveframe));
                for (int j = 0; j < saveframe - 1; j++)
                {
                    writer.WriteLine(arrLine[i + startframe + j]);
                }
                writer.Write(arrLine[i + startframe + saveframe - 1]);
                writer.Close();
            }
        }

        Debug.Log("bvh save done!!");

        /* WriteAllLines을 쓰면 마지막 null한줄이 더 추가되는 마법
        string[] newLine = new string[newfileLine];
        int i = 0;
        while (i <= newfileLine - saveframe)
        {
            newLine[i] = arrLine[i];
            i++;
            if (arrLine[i].Contains("Frames"))
                break;
        }

        newLine[i] = string.Format("Frames : {0}", saveframe);
        i++;
        newLine[i] = arrLine[i];    //frame time

        Debug.Log(string.Format("Frames : {0}", saveframe));
        for (int j = 1; j <= saveframe; j++)
        {
            newLine[i + j] = arrLine[i + startframe + j];
        }

        File.WriteAllLines(savePath, newLine);
        */
    }
}
