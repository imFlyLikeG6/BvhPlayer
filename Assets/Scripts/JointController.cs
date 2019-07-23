using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file JointController.cs
 * @brief 모델의 관절 컨트롤러. 각 씬의 모델에 붙어있습니다.
 */
public class JointController : MonoBehaviour {

    public Transform m_Hips;        //0
    public Transform m_Spine1;      //1
    public Transform m_Spine2;      //2
    public Transform m_Spine3;      //3
    public Transform m_Chest;       //4
    public Transform m_Neck;        //5
    public Transform m_Head;        //6
    public Transform m_HeadEnd;     //7
    public Transform m_L_Shoulder;  //8
    public Transform m_L_UpArm;     //9
    public Transform m_L_LowArm;    //10
    public Transform m_L_Hand;      //11
    public Transform m_L_HandEnd;   //12
    public Transform m_R_Shoulder;  //13
    public Transform m_R_UpArm;     //14
    public Transform m_R_LowArm;    //15
    public Transform m_R_Hand;      //16
    public Transform m_R_HandEnd;   //17
    public Transform m_L_UpLeg;     //18
    public Transform m_L_LowLeg;    //19
    public Transform m_L_Foot;      //20
    public Transform m_L_Toe;       //21
    public Transform m_R_UpLeg;     //22
    public Transform m_R_LowLeg;    //23
    public Transform m_R_Foot;      //24
    public Transform m_R_Toe;       //25

    public Transform m_L_Finger;      //26
    public Transform m_R_Finger;      //27

    private Transform m_Dummy;       //
    public Transform m_Cog;

    public Transform[] m_JointBall;
    public MeshRenderer[] m_SensorBall;
    public MeshRenderer[] m_MuscleBall;
    public GameObject m_Muscles;
    public SkinnedMeshRenderer m_SkinRenderer;

    public bool m_SensorViewOn;
    public bool m_MuscleViewOn;

    public float m_Height = 180f;
    private float m_ModelHeight = 2.743842f;
    private float m_HeightUnit = 1/180f;

    private float m_ModelHips = 1.472f;
    private float m_HipsHeight = 100f;
    private float m_HipsHeightUnit = 1.472f / 100f;

    private int[] m_CsvOrderTable = { 2, 0, 1, 8, 3, 5, 6, 15, 10, 12, 13, 9, 7, 4, 16, 14, 11 };


    //public List<Vector3[]> m_ModelAxisOffset = new List<Vector3[]>();
    private Vector3[] m_ModelWorldPosOffset = new Vector3[28];
    private Quaternion[] m_ModelQuaOffset = new Quaternion[28];
    public List<Quaternion> m_JointsQuatOffset = new List<Quaternion>();
    public List<Transform> m_Joints = new List<Transform>();
    public float PositionUnit = 1;

    public Quaternion m_NullQuat;

    /**
    * @brief Unity에서 쓰는 모델의 설정 되어있는 rotation과 position값을 저장합니다.
    */
    public void Awake()
    {
        m_NullQuat = new Quaternion(0, 0, 0, 0);

        m_ModelQuaOffset[0] = m_Hips.rotation;
        m_ModelQuaOffset[1] = m_Spine1.rotation;
        m_ModelQuaOffset[2] = m_Spine2.rotation;
        m_ModelQuaOffset[3] = m_Spine3.rotation;
        m_ModelQuaOffset[4] = m_Chest.rotation;
        m_ModelQuaOffset[5] = m_Neck.rotation;
        m_ModelQuaOffset[6] = m_Head.rotation;
        m_ModelQuaOffset[7] = m_HeadEnd.rotation;
        m_ModelQuaOffset[8] = m_L_Shoulder.rotation;
        m_ModelQuaOffset[9] = m_L_UpArm.rotation;
        m_ModelQuaOffset[10] = m_L_LowArm.rotation;
        m_ModelQuaOffset[11] = m_L_Hand.rotation;
        m_ModelQuaOffset[12] = m_L_HandEnd.rotation;
        m_ModelQuaOffset[13] = m_R_Shoulder.rotation;
        m_ModelQuaOffset[14] = m_R_UpArm.rotation;
        m_ModelQuaOffset[15] = m_R_LowArm.rotation;
        m_ModelQuaOffset[16] = m_R_Hand.rotation;
        m_ModelQuaOffset[17] = m_R_HandEnd.rotation;
        m_ModelQuaOffset[18] = m_L_UpLeg.rotation;
        m_ModelQuaOffset[19] = m_L_LowLeg.rotation;
        m_ModelQuaOffset[20] = m_L_Foot.rotation;
        m_ModelQuaOffset[21] = m_L_Toe.rotation;
        m_ModelQuaOffset[22] = m_R_UpLeg.rotation;
        m_ModelQuaOffset[23] = m_R_LowLeg.rotation;
        m_ModelQuaOffset[24] = m_R_Foot.rotation;
        m_ModelQuaOffset[25] = m_R_Toe.rotation;
        m_ModelQuaOffset[26] = m_L_Finger.rotation;
        m_ModelQuaOffset[27] = m_R_Finger.rotation;


        m_ModelWorldPosOffset[0] = m_Hips.position;
        m_ModelWorldPosOffset[1] = m_Spine1.position;
        m_ModelWorldPosOffset[2] = m_Spine2.position;
        m_ModelWorldPosOffset[3] = m_Spine3.position;
        m_ModelWorldPosOffset[4] = m_Chest.position;
        m_ModelWorldPosOffset[5] = m_Neck.position;
        m_ModelWorldPosOffset[6] = m_Head.position;
        m_ModelWorldPosOffset[7] = m_HeadEnd.position;
        m_ModelWorldPosOffset[8] = m_L_Shoulder.position;
        m_ModelWorldPosOffset[9] = m_L_UpArm.position;
        m_ModelWorldPosOffset[10] = m_L_LowArm.position;
        m_ModelWorldPosOffset[11] = m_L_Hand.position;
        m_ModelWorldPosOffset[12] = m_L_HandEnd.position;
        m_ModelWorldPosOffset[13] = m_R_Shoulder.position;
        m_ModelWorldPosOffset[14] = m_R_UpArm.position;
        m_ModelWorldPosOffset[15] = m_R_LowArm.position;
        m_ModelWorldPosOffset[16] = m_R_Hand.position;
        m_ModelWorldPosOffset[17] = m_R_HandEnd.position;
        m_ModelWorldPosOffset[18] = m_L_UpLeg.position;
        m_ModelWorldPosOffset[19] = m_L_LowLeg.position;
        m_ModelWorldPosOffset[20] = m_L_Foot.position;
        m_ModelWorldPosOffset[21] = m_L_Toe.position;
        m_ModelWorldPosOffset[22] = m_R_UpLeg.position;
        m_ModelWorldPosOffset[23] = m_R_LowLeg.position;
        m_ModelWorldPosOffset[24] = m_R_Foot.position;
        m_ModelWorldPosOffset[25] = m_R_Toe.position;
        m_ModelWorldPosOffset[26] = m_L_Finger.position;
        m_ModelWorldPosOffset[27] = m_R_Finger.position;


        //m_ModelQuaOffset[0] = m_Hips.localRotation;
        //m_ModelQuaOffset[1] = m_Spine1.localRotation;
        //m_ModelQuaOffset[2] = m_Spine2.localRotation;
        //m_ModelQuaOffset[3] = m_Spine3.localRotation;
        //m_ModelQuaOffset[4] = m_Chest.localRotation;
        //m_ModelQuaOffset[5] = m_Neck.localRotation;
        //m_ModelQuaOffset[6] = m_Head.localRotation;
        //m_ModelQuaOffset[7] = m_HeadEnd.localRotation;
        //m_ModelQuaOffset[8] = m_L_Shoulder.localRotation;
        //m_ModelQuaOffset[9] = m_L_UpArm.localRotation;
        //m_ModelQuaOffset[10] = m_L_LowArm.localRotation;
        //m_ModelQuaOffset[11] = m_L_Hand.localRotation;
        //m_ModelQuaOffset[12] = m_L_HandEnd.localRotation;
        //m_ModelQuaOffset[13] = m_R_Shoulder.localRotation;
        //m_ModelQuaOffset[14] = m_R_UpArm.localRotation;
        //m_ModelQuaOffset[15] = m_R_LowArm.localRotation;
        //m_ModelQuaOffset[16] = m_R_Hand.localRotation;
        //m_ModelQuaOffset[17] = m_R_HandEnd.localRotation;
        //m_ModelQuaOffset[18] = m_L_UpLeg.localRotation;
        //m_ModelQuaOffset[19] = m_L_LowLeg.localRotation;
        //m_ModelQuaOffset[20] = m_L_Foot.localRotation;
        //m_ModelQuaOffset[21] = m_L_Toe.localRotation;
        //m_ModelQuaOffset[22] = m_R_UpLeg.localRotation;
        //m_ModelQuaOffset[23] = m_R_LowLeg.localRotation;
        //m_ModelQuaOffset[24] = m_R_Foot.localRotation;
        //m_ModelQuaOffset[25] = m_R_Toe.localRotation;
        //m_ModelQuaOffset[26] = m_L_Finger.localRotation;
        //m_ModelQuaOffset[27] = m_R_Finger.localRotation;

        m_SensorViewOn = false;
        m_MuscleViewOn = false;
    }

    /**
    * @brief 모델의 각 관절의 회전값과 hips의 위치값을 설정합니다.
    * @details 받은 local rotation값을 world rotation으로 셋팅하기 때문에 한번 기존회전값으로 셋팅 후, bvh트리구조의 하위 세그먼트부터 설정합니다.
    * @param rot    관절의 local rotation값.
    * @param pos    hips의 위치 값.
    */
    public void SetRotation(Quaternion[] rot, Vector3 pos)
    {
        for (int i = 0; i < rot.Length; i++)
        {
            if (i < m_Joints.Count && m_Joints[i] != null)
            {
                m_Joints[i].rotation = m_JointsQuatOffset[i];
            }
        }

        for (int i = rot.Length - 1; i >= 0; i--)
        {
            if (i < m_Joints.Count && m_Joints[i] != null)
            {
                if(!rot[i].Equals(m_NullQuat))
                    m_Joints[i].rotation = rot[i] * m_JointsQuatOffset[i];
            }
        }
        pos *= m_HipsHeightUnit;
        m_Hips.position = pos;

    }

    /**
    * @brief 모델의 각 관절의 회전값과 hips의 위치값을 설정합니다.
    * @details world rotation값을 받기때문에, bvh트리구조의 상위 세그먼트부터 설정합니다.
    * @param rot    관절의 world rotation값.
    * @param pos    hips의 위치 값.
    */
    public void SetWorldRotation(Quaternion[] rot, Vector3 pos)
    {
        for (int i = 0; i < rot.Length; i++)
        {
            int idx = m_CsvOrderTable[i];
            if (idx < m_Joints.Count && m_Joints[idx] != null)
            {
                if (!rot[idx].Equals(m_NullQuat))
                    m_Joints[idx].rotation = rot[idx] * m_JointsQuatOffset[idx];
            }
        }
        pos *= m_HipsHeightUnit;
        m_Hips.position = pos;
    }

    /**
    * @brief 초기화 함수.
    */
    public void Clear()
    {
        if (m_JointsQuatOffset != null)
        {
            for (int i = 0; i < m_Joints.Count; i++)
            {
                if (m_Joints[i] != null && m_JointsQuatOffset[i] != null)
                    m_Joints[i].rotation = m_JointsQuatOffset[i];
            }

            m_JointsQuatOffset.Clear();
        }
        if (m_Joints != null)
            m_Joints.Clear();
    }

    /**
    * @brief bvh에서 읽은 offsetdata를 토대로 m_Joints(Transform)배열을 설정합니다.
    * @param offsetData    offsetdata배열 값.
    * @param flagCount    offsetData에 있는 트리구조의 하위세그먼트의 갯수.
    */
    public void SetOffsetData(List<BvhController.OffsetData> offsetData, List<int> flagCount)
    {
        Clear();
        int flag = 0;
        int i = 0;

        while (i < offsetData.Count)
        {
            int jointFlag = -1;
            for (int j = i; j < i + flagCount[flag]; j++)
            {
                if (offsetData[j].name.Contains("Hips"))
                {
                    //PositionUnit = m_Hips.position.y / offsetData[j].offset.y;
                    jointFlag = 0;
                    break;
                }
                else if (offsetData[j].name.Contains("Head"))
                {
                    jointFlag = 1;
                    break;
                }
                else if (offsetData[j].name.Contains("Shoulder")
                    || offsetData[j].name.Contains("Collar"))
                {
                    if (offsetData[j].name.Contains("Left"))
                        jointFlag = 2;
                    else
                        jointFlag = 3;
                    break;
                }
                else if (offsetData[j].name.Contains("Leg")
                    || offsetData[j].name.Contains("Knee"))
                {
                    if (offsetData[j].name.Contains("Left"))
                        jointFlag = 4;
                    else
                        jointFlag = 5;
                    break;
                }

            }
            AddJoint(jointFlag, flagCount[flag]);
            i += flagCount[flag];
            flag++;
        }
    }

    /**
    * @brief bvh의 0프레임에서 읽은 t자세를 통해 hips의 높이를 설정합니다.
    * @param hipsHeight hips의 높이 값.
    */
    public void SetHipsHeight(float hipsHeight)
    {
        if(hipsHeight != 0 )
        {
            m_HipsHeight = hipsHeight;
        }
        else
            m_HipsHeight = 100f;

        m_HipsHeightUnit =  m_ModelHips / m_HipsHeight;
    }

    /**
    * @brief m_Joints와 m_JointsQuatOffset에 관절데이터를 추가합니다.
    * @param flag   관절 상위 flag값. 0:hips, 1:head, 2: lhand, 3: rhand, 4:lleg, 5:rleg.
    * @param count  상위세그먼트에 대한 하위세그먼트 갯수.
    */
    public void AddJoint(int flag, int count)
    {
        switch (flag)
        {
            case 0:
                m_Joints.Add(m_Hips);
                m_JointsQuatOffset.Add(m_ModelQuaOffset[0]);
                break;
            case 1: // head
                if (count == 3)
                {
                    m_Joints.Add(m_Neck);
                    m_Joints.Add(m_Head);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[6]);
                }
                else if (count == 4)
                {
                    m_Joints.Add(m_Spine1);
                    m_Joints.Add(m_Neck);
                    m_Joints.Add(m_Head);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[1]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[6]);
                }
                else if (count == 5)
                {
                    m_Joints.Add(m_Spine3);
                    m_Joints.Add(m_Chest);
                    m_Joints.Add(m_Neck);
                    m_Joints.Add(m_Head);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[3]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[4]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[6]);
                }
                else if (count >= 6)
                {
                    m_Joints.Add(m_Spine1);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[1]);
                    AddDummyJoint(count - 6);
                    m_Joints.Add(m_Spine3);
                    m_Joints.Add(m_Chest);
                    m_Joints.Add(m_Neck);
                    m_Joints.Add(m_Head);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[3]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[4]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[6]);
                }
                break;
            case 2: // l hand
                if (count >= 5)
                {
                    m_Joints.Add(m_L_Shoulder);
                    m_Joints.Add(m_L_UpArm);
                    m_Joints.Add(m_L_LowArm);
                    m_Joints.Add(m_L_Hand);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[8]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[9]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[10]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[11]);
                    AddDummyJoint(count - 5);
                }
                break;
            case 3: // r hand
                if (count >= 5 && count < 9)
                {
                    m_Joints.Add(m_R_Shoulder);
                    m_Joints.Add(m_R_UpArm);
                    m_Joints.Add(m_R_LowArm);
                    m_Joints.Add(m_R_Hand);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[13]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[14]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[15]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[16]);
                    AddDummyJoint(count - 5);
                }
                else if (count >= 9) // neck & hand
                {
                    m_Joints.Add(m_Spine1);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[1]);
                    m_Joints.Add(m_Spine3);
                    m_Joints.Add(m_Chest);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[3]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[4]);

                    m_Joints.Add(m_R_Shoulder);
                    m_Joints.Add(m_R_UpArm);
                    m_Joints.Add(m_R_LowArm);
                    m_Joints.Add(m_R_Hand);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[13]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[14]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[15]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[16]);
                    AddDummyJoint(count - 8);
                }
                break;
            case 4: // l leg
                if (count == 4)
                {
                    m_Joints.Add(m_L_UpLeg);
                    m_Joints.Add(m_L_LowLeg);
                    m_Joints.Add(m_L_Foot);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[18]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[19]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[20]);
                }
                else if (count >= 5)
                {
                    m_Joints.Add(m_L_UpLeg);
                    m_Joints.Add(m_L_LowLeg);
                    m_Joints.Add(m_L_Foot);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[18]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[19]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[20]);
                    AddDummyJoint(count - 5);
                    m_Joints.Add(m_L_Toe);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[21]);
                }
                break;
            case 5: // r leg
                if (count == 4)
                {
                    m_Joints.Add(m_R_UpLeg);
                    m_Joints.Add(m_R_LowLeg);
                    m_Joints.Add(m_R_Foot);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[22]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[23]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[24]);
                }
                else if (count >= 5)
                {
                    m_Joints.Add(m_R_UpLeg);
                    m_Joints.Add(m_R_LowLeg);
                    m_Joints.Add(m_R_Foot);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[22]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[23]);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[24]);
                    AddDummyJoint(count - 5);
                    m_Joints.Add(m_R_Toe);
                    m_JointsQuatOffset.Add(m_ModelQuaOffset[25]);
                }
                break;
        }
    }

    /**
    * @brief bvh에비해 모델에 관절이없어서 더미데이터를 추가합니다.
    */
    public void AddDummyJoint(int count)
    {
        for (int i = 0; i < count; i++)
        {
            m_Joints.Add(null);
            m_JointsQuatOffset.Add(Quaternion.identity);
        }
    }

    /**
    * @brief 모델에 붙어있는 관절볼을 보여줍니다.
    */
    public void SetJointBallActive(bool active)
    {
        foreach (Transform t in m_JointBall)
        {
            t.gameObject.SetActive(active);
        }
        SkinTransparent();
    }

    /**
    * @brief 모델에 붙어있는 센서 세그먼트를 보여줍니다.
    */
    public void SetSensorActive(bool active)
    {
        foreach (MeshRenderer t in m_SensorBall)
        {
            t.enabled = active;
        }
        m_SensorViewOn = active;
        SkinTransparent();
    }

    /**
    * @brief 모델에 붙어있는 근육 스킨을 보여줍니다.
    */
    public void SetMuscleActive(bool active)
    {
        //foreach (MeshRenderer t in m_MuscleBall)
        //{
        //    t.enabled = active;
        //}
        m_MuscleViewOn = active;
        m_Muscles.SetActive(active);
        SkinTransparent();
    }

    /**
    * @brief 센서,관절,근육들을 보여줄때마다 모델의 겉 스킨을 투명하게 합니다.
    */
    private void SkinTransparent()
    {
        if (m_SensorViewOn || m_JointBall[0].gameObject.activeSelf || m_MuscleViewOn)
        {
            //m_SkinRenderer.material.SetFloat("_Mode", 3f);
            //m_SkinRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //m_SkinRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //m_SkinRenderer.material.renderQueue = 3000;
            m_SkinRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);

        }
        else
        {
            //m_SkinRenderer.material.SetFloat("_Mode", 0f);
            //m_SkinRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            //m_SkinRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            //m_SkinRenderer.material.renderQueue = -1;
            m_SkinRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    /**
    * @brief 기본 joint데이터 배열 구성. 현재 쓰는곳 없음.
    */
    public void SetDelfaultJoint()
    {
        Clear();

        m_Joints.Add(m_Hips);   //Hips
        m_JointsQuatOffset.Add(m_ModelQuaOffset[0]);

        m_Joints.Add(m_R_UpLeg);   //RightThigh
        m_Joints.Add(m_R_LowLeg);   //RightLeg
        m_Joints.Add(m_R_Foot);   //RightFoot
        m_Joints.Add(m_R_Toe);   //RightToe
        m_JointsQuatOffset.Add(m_ModelQuaOffset[22]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[23]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[24]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[25]);

        m_Joints.Add(m_L_UpLeg);    //LeftThigh
        m_Joints.Add(m_L_LowLeg);    //LeftLeg
        m_Joints.Add(m_L_Foot);    //LeftFoot
        m_Joints.Add(m_L_Toe);    //LeftToe
        m_JointsQuatOffset.Add(m_ModelQuaOffset[18]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[19]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[20]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[21]);


        m_Joints.Add(m_Spine1);    //SpineLow
        m_Joints.Add(m_Spine3);    //SpineMid
        m_Joints.Add(m_Chest);    //Chest
        m_JointsQuatOffset.Add(m_ModelQuaOffset[1]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[3]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[4]);

        m_Joints.Add(m_R_Shoulder);    //RightShoulder
        m_Joints.Add(m_R_UpArm);    //RightArm
        m_Joints.Add(m_R_LowArm);    //RightForearm
        m_Joints.Add(m_R_Hand);    //RightHand
        m_Joints.Add(m_R_Finger);    //RightFinger
        m_JointsQuatOffset.Add(m_ModelQuaOffset[13]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[14]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[15]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[16]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[27]);

        m_Joints.Add(m_L_Shoulder);    //LeftShoulder
        m_Joints.Add(m_L_UpArm);    //LeftArm
        m_Joints.Add(m_L_LowArm);    //LeftForearm
        m_Joints.Add(m_L_Hand);    //LeftHand
        m_Joints.Add(m_L_Finger);    //LeftFinger
        m_JointsQuatOffset.Add(m_ModelQuaOffset[8]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[9]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[10]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[11]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[26]);

        m_Joints.Add(m_Neck);    //Neck
        m_Joints.Add(m_Head);    //Head
        m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[6]);

    }

    /**
    * @brief csv데이터에 따라 joint배열을 설정합니다.
    */
    public void SetCsvJoint()
    {
        Clear();

        m_Joints.Add(m_Chest);    //Chest
        m_JointsQuatOffset.Add(m_ModelQuaOffset[4]);

        m_Joints.Add(m_Neck);    //Neck
        m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);

        m_Joints.Add(m_Hips);   //Hips
        m_JointsQuatOffset.Add(m_ModelQuaOffset[0]);

        m_Joints.Add(m_L_UpArm);    //LeftArm
        m_Joints.Add(m_L_Foot);    //LeftFoot
        m_Joints.Add(m_L_LowArm);    //LeftForearm
        m_Joints.Add(m_L_Hand);    //LeftHand
        m_Joints.Add(m_L_LowLeg);    //LeftLeg
        m_Joints.Add(m_L_Shoulder);    //LeftShoulder
        m_Joints.Add(m_L_UpLeg);    //LeftThigh
        m_JointsQuatOffset.Add(m_ModelQuaOffset[9]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[20]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[10]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[11]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[19]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[8]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[18]);

        m_Joints.Add(m_R_UpArm);    //RightArm
        m_Joints.Add(m_R_Foot);   //RightFoot
        m_Joints.Add(m_R_LowArm);    //RightForearm
        m_Joints.Add(m_R_Hand);    //RightHand
        m_Joints.Add(m_R_LowLeg);   //RightLeg
        m_Joints.Add(m_R_Shoulder);    //RightShoulder
        m_Joints.Add(m_R_UpLeg);   //RightThigh
        m_JointsQuatOffset.Add(m_ModelQuaOffset[14]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[24]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[15]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[16]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[23]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[13]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[22]);
    }

    /**
    * @brief live씬에서 쓰는 sensor id 순서에 따라 joint배열을 설정합니다.
    */
    public void SetLiveJoint()
    {
        Clear();

        m_Joints.Add(m_Hips);   //Hips
        m_JointsQuatOffset.Add(m_ModelQuaOffset[0]);

        m_Joints.Add(m_Spine3);    //SpineMid
        m_JointsQuatOffset.Add(m_ModelQuaOffset[3]);

        m_Joints.Add(m_Neck);    //Neck
        m_JointsQuatOffset.Add(m_ModelQuaOffset[5]);

        m_Joints.Add(m_R_Shoulder);    //RightShoulder
        m_Joints.Add(m_R_UpArm);    //RightArm
        m_Joints.Add(m_R_LowArm);    //RightForearm
        m_Joints.Add(m_R_Hand);    //RightHand
        m_JointsQuatOffset.Add(m_ModelQuaOffset[13]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[14]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[15]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[16]);

        m_Joints.Add(m_L_Shoulder);    //LeftShoulder
        m_Joints.Add(m_L_UpArm);    //LeftArm
        m_Joints.Add(m_L_LowArm);    //LeftForearm
        m_Joints.Add(m_L_Hand);    //LeftHand
        m_JointsQuatOffset.Add(m_ModelQuaOffset[8]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[9]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[10]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[11]);

        m_Joints.Add(m_R_UpLeg);   //RightThigh
        m_Joints.Add(m_R_LowLeg);   //RightLeg
        m_Joints.Add(m_R_Foot);   //RightFoot
        m_JointsQuatOffset.Add(m_ModelQuaOffset[22]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[23]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[24]);

        m_Joints.Add(m_L_UpLeg);    //LeftThigh
        m_Joints.Add(m_L_LowLeg);    //LeftLeg
        m_Joints.Add(m_L_Foot);    //LeftFoot
        m_JointsQuatOffset.Add(m_ModelQuaOffset[18]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[19]);
        m_JointsQuatOffset.Add(m_ModelQuaOffset[20]);
        
    }

    /**
    * @brief 모델의 키를 설정합니다. defalut : 180
    * @param height   설정할 키. 추천범위:150~190.
    */
    public void SetHeightAuto(float height)
    {
        Quaternion[] tmpRot = new Quaternion[m_Joints.Count];
        Vector3 tmpPos = m_Hips.position;
        for (int i = 0; i < m_Joints.Count; i++)
        {
            if (m_Joints[i] != null && m_JointsQuatOffset[i] != null)
                tmpRot[i] = m_Joints[i].rotation;
        }

        if (m_JointsQuatOffset != null)
        {
            for (int i = 0; i < m_Joints.Count; i++)
            {
                if (m_Joints[i] != null && m_JointsQuatOffset[i] != null)
                    m_Joints[i].rotation = m_JointsQuatOffset[i];
            }
        }
        m_Hips.position = new Vector3(0, m_ModelHips, 0);


        m_Height = height;
        float totalHeight = height * m_HeightUnit;

        print(totalHeight + ", " + m_HeightUnit);
        m_Hips.position         = m_ModelWorldPosOffset[0] * totalHeight;
        m_Spine1.position       = m_ModelWorldPosOffset[1] * totalHeight;
        m_Spine2.position       = m_ModelWorldPosOffset[2] * totalHeight;
        m_Spine3.position       = m_ModelWorldPosOffset[3] * totalHeight;
        m_Chest.position        = m_ModelWorldPosOffset[4] * totalHeight;
        m_Neck.position         = m_ModelWorldPosOffset[5] * totalHeight;
        m_Head.position         = m_ModelWorldPosOffset[6] * totalHeight;
        m_HeadEnd.position      = m_ModelWorldPosOffset[7] * totalHeight;
        m_L_Shoulder.position   = m_ModelWorldPosOffset[8] * totalHeight;
        m_L_UpArm.position      = m_ModelWorldPosOffset[9] * totalHeight;
        m_L_LowArm.position     = m_ModelWorldPosOffset[10] * totalHeight;
        m_L_Hand.position       = m_ModelWorldPosOffset[11] * totalHeight;
        m_L_HandEnd.position    = m_ModelWorldPosOffset[12] * totalHeight;
        m_R_Shoulder.position   = m_ModelWorldPosOffset[13] * totalHeight;
        m_R_UpArm.position      = m_ModelWorldPosOffset[14] * totalHeight;
        m_R_LowArm.position     = m_ModelWorldPosOffset[15] * totalHeight;
        m_R_Hand.position       = m_ModelWorldPosOffset[16] * totalHeight;
        m_R_HandEnd.position    = m_ModelWorldPosOffset[17] * totalHeight;
        m_L_UpLeg.position      = m_ModelWorldPosOffset[18] * totalHeight;
        m_L_LowLeg.position     = m_ModelWorldPosOffset[19] * totalHeight;
        m_L_Foot.position       = m_ModelWorldPosOffset[20] * totalHeight;
        m_L_Toe.position        = m_ModelWorldPosOffset[21] * totalHeight;
        m_R_UpLeg.position      = m_ModelWorldPosOffset[22] * totalHeight;
        m_R_LowLeg.position     = m_ModelWorldPosOffset[23] * totalHeight;
        m_R_Foot.position       = m_ModelWorldPosOffset[24] * totalHeight;
        m_R_Toe.position        = m_ModelWorldPosOffset[25] * totalHeight;
        m_L_Finger.position     = m_ModelWorldPosOffset[26] * totalHeight;
        m_R_Finger.position     = m_ModelWorldPosOffset[27] * totalHeight;

        string tmp;
        tmp =
        "m_Hips.position        "+ m_Hips.position       *   (m_Height/m_ModelHeight)
        + "\nm_Spine1.position      "+ m_Spine1.position     *   (m_Height/m_ModelHeight)
        + "\nm_Spine2.position      "+ m_Spine2.position     *   (m_Height/m_ModelHeight)
        + "\nm_Spine3.position      "+ m_Spine3.position     *   (m_Height/m_ModelHeight)
        + "\nm_Chest.position       "+ m_Chest.position      *   (m_Height/m_ModelHeight)
        + "\nm_Neck.position        "+ m_Neck.position       *   (m_Height/m_ModelHeight)
        + "\nm_Head.position        "+ m_Head.position       *   (m_Height/m_ModelHeight)
        + "\nm_HeadEnd.position     "+ m_HeadEnd.position    *   (m_Height/m_ModelHeight)
        + "\nm_L_Shoulder.position  "+ m_L_Shoulder.position *   (m_Height/m_ModelHeight)
        + "\nm_L_UpArm.position     "+ m_L_UpArm.position    *   (m_Height/m_ModelHeight)
        + "\nm_L_LowArm.position    "+ m_L_LowArm.position   *   (m_Height/m_ModelHeight)
        + "\nm_L_Hand.position      "+ m_L_Hand.position     *   (m_Height/m_ModelHeight)
        + "\nm_L_HandEnd.position   "+ m_L_HandEnd.position  *   (m_Height/m_ModelHeight)
        + "\nm_R_Shoulder.position  "+ m_R_Shoulder.position *   (m_Height/m_ModelHeight)
        + "\nm_R_UpArm.position     "+ m_R_UpArm.position    *   (m_Height/m_ModelHeight)
        + "\nm_R_LowArm.position    "+ m_R_LowArm.position   *   (m_Height/m_ModelHeight)
        + "\nm_R_Hand.position      "+ m_R_Hand.position     *   (m_Height/m_ModelHeight)
        + "\nm_R_HandEnd.position   "+ m_R_HandEnd.position  *   (m_Height/m_ModelHeight)
        + "\nm_L_UpLeg.position     "+ m_L_UpLeg.position    *   (m_Height/m_ModelHeight)
        + "\nm_L_LowLeg.position    "+ m_L_LowLeg.position   *   (m_Height/m_ModelHeight)
        + "\nm_L_Foot.position      "+ m_L_Foot.position     *   (m_Height/m_ModelHeight)
        + "\nm_L_Toe.position       "+ m_L_Toe.position      *   (m_Height/m_ModelHeight)
        + "\nm_R_UpLeg.position     "+ m_R_UpLeg.position    *   (m_Height/m_ModelHeight)
        + "\nm_R_LowLeg.position    "+ m_R_LowLeg.position   *   (m_Height/m_ModelHeight)
        + "\nm_R_Foot.position      "+ m_R_Foot.position     *   (m_Height/m_ModelHeight)
        + "\nm_R_Toe.position       "+ m_R_Toe.position      *   (m_Height/m_ModelHeight)
        + "\nm_L_Finger.position    "+ m_L_Finger.position   *   (m_Height/m_ModelHeight)
        + "\nm_R_Finger.position    " + m_R_Finger.position * (m_Height / m_ModelHeight);
        print(tmp);
               
        for (int i = tmpRot.Length - 1; i >= 0; i--)
        {
            if (i < m_Joints.Count && m_Joints[i] != null)
            {
                if (!tmpRot[i].Equals(m_NullQuat))
                    m_Joints[i].rotation = tmpRot[i];
            }
        }

        float ratio = m_Hips.position.y / m_ModelHips;
        m_ModelHips = m_Hips.position.y;
        tmpPos *= ratio;
        m_Hips.position = tmpPos;
    }

    /**
    * @brief live씬에서 해당 인덱스에 따른 모델길이를 가져옵니다.
    * @param index   0-upLeg~loLeg, 1-loLeg~foot, 2-upArm~loArm, 3-loArm~Hand 
    */
    public float GetConfig(int index)
    {
        float unit = m_Height / m_ModelHeight;
        float result = 0;
        switch(index)
        {
            case 0:
                result = (m_L_UpLeg.position.y - m_L_LowLeg.position.y) * unit;
                break;
            case 1:
                result = (m_L_LowLeg.position.y) * unit;
                break;
            case 2:
                result = (m_L_UpArm.position - m_L_LowArm.position).magnitude * unit;
                break;
            case 3:
                result = (m_L_LowArm.position - m_L_Hand.position).magnitude * unit;
                break;
            default:
                result = (m_Head.position.y) * unit;
                break;

        }
        return result;
    }

    /**
    * @brief cog의 위치를 설정합니다.
    * @param pos cog의 위치 값. 유니티단위가 아닌 실제 cm값.
    */
    public void SetCogPosition(Vector3 pos)
    {        
        pos.y = 0; // 무조건 바닥에 붙게
        pos *= m_HipsHeightUnit;
        //pos /= 84.0729f;
        m_Cog.position = pos;
    }
}
