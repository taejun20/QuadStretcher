/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class SlingshotHandler : MonoBehaviour, IEngageHandler
{
    [SerializeField]
    SerialManager _serialManager;
    [SerializeField]
    LineRenderer leftElasticLine_LineRenderer, rightElasticLine_LineRenderer;
    [SerializeField]
    GameObject leatherLine;
    [SerializeField]
    Transform leatherLinePositionMarkerTransform;
    [SerializeField]
    Transform RealRightHandTransform, VirtualRightHandTransform;
    [SerializeField]
    StretcherManager m_mngR;
    [HideInInspector] public bool slingshotGrabbed = false;

    Vector3 initleatherLinePosition = new Vector3();
    Vector3 initVRHPosition = new Vector3();
    Quaternion initVRHRotation = new Quaternion();
    Vector3 deltaVector3, stretchInputVector3;

    void Start()
    {
        initVRHPosition = VirtualRightHandTransform.position;
        initVRHRotation = VirtualRightHandTransform.rotation;
        leftElasticLine_LineRenderer.SetPosition(1, new Vector3(0, 0, 0.1f));
        rightElasticLine_LineRenderer.SetPosition(1, new Vector3(0, 0, 0.1f));
        initleatherLinePosition = leatherLine.transform.position;
    }

    void Update()
    {
        if (slingshotGrabbed)
        {
            deltaVector3 = (initVRHPosition - RealRightHandTransform.position) * -1;
            stretchInputVector3 = deltaVector3 * 3f;     // Want to give full stretch when actual deltaVector3 is at 0.5
            if (stretchInputVector3.magnitude > 1)
			{
                deltaVector3 = deltaVector3.normalized / 3f;
                stretchInputVector3 = stretchInputVector3.normalized;
            }
            VirtualRightHandTransform.position = initVRHPosition + deltaVector3;
            Debug.Log("stretchInputVector3: " + stretchInputVector3.ToString("f4") + ", stretchInputVector3.magnitude: " + stretchInputVector3.magnitude.ToString("f4"));

            float diffX = deltaVector3.x;
            float diffY = deltaVector3.y;
            float diffZ = deltaVector3.z;

            leftElasticLine_LineRenderer.SetPosition(1, new Vector3(diffX * 20, diffY * 20, diffZ * 20));
            rightElasticLine_LineRenderer.SetPosition(1, new Vector3(diffX * 20, diffY * 20, diffZ * 20));
            leatherLine.transform.position = leatherLinePositionMarkerTransform.position;
            if (SerialManager.SerialPort.IsOpen)
			{
                stretchInputVector3 = stretchInputVector3 * 1.1f;
                float[][] stretchGainMapping = new float[3][];
                stretchGainMapping[0] = new float[] { 1f, 1f, 1f, 1f };
                stretchGainMapping[1] = new float[] { 0, -1, 0, 1 };
                stretchGainMapping[2] = new float[] { -1, 0, 1, 0 };
                m_mngR.stretchGains = stretchGainMapping;
                m_mngR.n_DoF = 3;
                m_mngR.updateStretcher(stretchInputVector3, false);
                m_mngR.stretcherUpdate();
            }
        }
    }


    public void InitOnEnabled()
    {
        slingshotGrabbed = false;
    }

    public void WrapupOnDisabled()
    {
        VirtualRightHandTransform.position = initVRHPosition;
        VirtualRightHandTransform.rotation = initVRHRotation;

        leftElasticLine_LineRenderer.SetPosition(1, new Vector3(0, 0, 0.1f));
        rightElasticLine_LineRenderer.SetPosition(1, new Vector3(0, 0, 0.1f));
        leatherLine.transform.position = initleatherLinePosition;

        m_mngR.AllTactorStretch(0);
    }
}
