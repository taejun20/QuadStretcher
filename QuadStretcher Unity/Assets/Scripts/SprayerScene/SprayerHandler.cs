/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class SprayerHandler : MonoBehaviour, IEngageHandler
{
    [SerializeField]
    SerialManager _serialManager;
    [SerializeField]
    Transform VirtualRightHandAndSprayTransform, RightHandTrackedTransform;
    [SerializeField]
    StretcherManager m_mngR;
    [SerializeField]
    OVRSkeleton m_skeleton;
    [HideInInspector] public bool isEngaged = false;
    
    Quaternion[] IndexOpen = new Quaternion[3];
    Quaternion[] MidOpen = new Quaternion[3];
    Quaternion[] IndexGrip = new Quaternion[3];
    Quaternion[] MidGrip = new Quaternion[3];
    Quaternion HandleOpen = new Quaternion();
    Quaternion HandleGrip = new Quaternion();    

    bool GotBones = false;
    Transform[] RingBones = new Transform[4];
    Transform[] PinkyBones = new Transform[4];
    Transform HandStart, MidTip;

    Transform[] StaticHandIndex = new Transform[3];
    Transform[] StaticHandMiddle = new Transform[3];

    Transform SprayHandle;
    float SprayHandleRotate = 35f; //degree
    float gripMag = 0f;

    Vector3 initPos = new Vector3();
    Quaternion initRot = new Quaternion();

    void Start()
    {
        initPos = VirtualRightHandAndSprayTransform.position;
        initRot = VirtualRightHandAndSprayTransform.rotation;

        IndexOpen[0] = Quaternion.Euler(7.117f, 8.255f, -12.4f);
        IndexOpen[1] = Quaternion.Euler(-2.924f, 0.82f, -23.723f);
        IndexOpen[2] = Quaternion.Euler(-2.048f, 2.974f, -11.833f);
        IndexGrip[0] = Quaternion.Euler(8.508f, 12.957f, -16.297f);
        IndexGrip[1] = Quaternion.Euler(-2.924f, 0.82f, -48.58f);
        IndexGrip[2] = Quaternion.Euler(-2.048f, 2.974f, -52.769f);

        MidOpen[0] = Quaternion.Euler(4.979f, 8.884f, -13.066f);
        MidOpen[1] = Quaternion.Euler(-1.406f, 0.604f, -39.152f);
        MidOpen[2] = Quaternion.Euler(-9.347f, 2.176f, -11.296f);
        MidGrip[0] = Quaternion.Euler(5.614f, 11.659f, -49.178f);
        MidGrip[1] = Quaternion.Euler(-1.406f, 0.604f, -75.643f);
        MidGrip[2] = Quaternion.Euler(-9.347f, 2.176f, -38.803f);

        for(int i=0;i<3;i++)
        {
            StaticHandIndex[i] = VirtualRightHandAndSprayTransform.transform.Find("Bones").Find("Hand_Start").FindChildRecursive("Hand_Index" + (i + 1).ToString());
            StaticHandMiddle[i] = VirtualRightHandAndSprayTransform.transform.Find("Bones").Find("Hand_Start").FindChildRecursive("Hand_Middle" + (i + 1).ToString());
        }
        SprayHandle = VirtualRightHandAndSprayTransform.transform.Find("Sprayer hand").Find("HandleAxis");

        HandleOpen = Quaternion.Euler(0, 0, 0);
        HandleGrip= Quaternion.Euler(SprayHandleRotate, 0, 0);
        initPos = VirtualRightHandAndSprayTransform.transform.position;
        initRot = VirtualRightHandAndSprayTransform.transform.rotation;
    }

    void Update()
    {
        if(isEngaged)
		{
            if (!GotBones)
            {
                if (RightHandTrackedTransform.Find("Bones"))
                {
                    Transform RHandBones = RightHandTrackedTransform.Find("Bones");
                    for (int i = 1; i <= 3; i++)
                    {
                        RingBones[i] = RHandBones.FindChildRecursive("Hand_Ring" + i.ToString());
                        PinkyBones[i] = RHandBones.FindChildRecursive("Hand_Pinky" + i.ToString());
                        //Debug.Log(i.ToString() + ": " + RingBones[i].ToString() + ", " + PinkyBones[i].ToString());
                    }
                    RingBones[0] = RHandBones.FindChildRecursive("Hand_Start");
                    PinkyBones[0] = RingBones[0];
                    HandStart = RingBones[0];

                    MidTip = RHandBones.FindChildRecursive("Hand_MiddleTip");
                    //Debug.Log("0: " + RingBones[0].ToString() + ", " + PinkyBones[0].ToString());
                    GotBones = true;
                }
            }
            else
            {
                gripMag = GripMagnitude();
                UpdateGripAnimation(gripMag);
                Debug.Log("gripMag: " + gripMag.ToString("f4"));

                float input_magnitude = gripMag;
                if (SerialManager.SerialPort.IsOpen)
				{
                    float[][] stretchGainMapping = new float[3][];
                    stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                    stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                    stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                    m_mngR.stretchGains = stretchGainMapping;
                    m_mngR.AllTactorStretch(input_magnitude);
                    m_mngR.stretcherUpdate();
                    Debug.Log("stretch_magnitude: " + input_magnitude.ToString());
                }
            }
        }
    }

    private float GripMagnitude()
    {
        // 0.185~0.0774 -> 0.16~0.1
        float minMag = 0.1f, maxMag = 0.16f;
        float magnitude = (HandStart.position - MidTip.position).magnitude;

        magnitude = 1 - (magnitude - minMag) / (maxMag - minMag);
        if (magnitude > 1)
            magnitude = 1;
        else if (magnitude < 0)
            magnitude = 0;
        //Debug.Log("Magnitude: " + magnitude.ToString());
        return magnitude;
    }

    private void UpdateGripAnimation(float magnitude)
    {
        for (int i = 0; i < 3; i++)
        {
            StaticHandIndex[i].localRotation = Quaternion.Lerp(IndexOpen[i], IndexGrip[i], magnitude);
            StaticHandMiddle[i].localRotation = Quaternion.Lerp(MidOpen[i], MidGrip[i], magnitude);
        }
        SprayHandle.localRotation = Quaternion.Lerp(HandleOpen, HandleGrip, magnitude);
    }

    public void InitOnEnabled()
    {
        m_skeleton._updateRootPose = true;
    }

    public void WrapupOnDisabled()
    {
        for (int i = 0; i < 3; i++)
        {
            StaticHandIndex[i].localRotation = Quaternion.Lerp(IndexOpen[i], IndexGrip[i], 0);
            StaticHandMiddle[i].localRotation = Quaternion.Lerp(MidOpen[i], MidGrip[i], 0);
        }
        SprayHandle.localRotation = Quaternion.Lerp(HandleOpen, HandleGrip, 0);
        gripMag = 0f;
        VirtualRightHandAndSprayTransform.position = initPos;
        VirtualRightHandAndSprayTransform.rotation = initRot;
        m_skeleton._updateRootPose = false;
        m_mngR.AllTactorStretch(0);
        m_mngR.stretcherUpdate();
    }
}
