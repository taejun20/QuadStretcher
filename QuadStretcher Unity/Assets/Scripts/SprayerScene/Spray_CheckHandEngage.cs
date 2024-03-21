/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class Spray_CheckHandEngage : MonoBehaviour
{
    [SerializeField]
    GameObject RightHandTracked, RightHandEngaged;
    [SerializeField]
    Material EngagedHandMaterial, DisengagedHandMaterial;
    [SerializeField]
    SprayerHandler _SprayHandler;

    Transform[] FingertipConds_Stiff = new Transform[5];
    Transform[] FingertipTracked = new Transform[5];
    Transform RightHandTracked_Root;
    bool Engaged = false;

    Vector3 RightHandEngaged_InitPos = new Vector3();
    Quaternion RightHandEngaged_InitRot = new Quaternion();

    void Start()
    {
        RightHandEngaged_InitPos = RightHandEngaged.transform.position;
        RightHandEngaged_InitRot = RightHandEngaged.transform.rotation;

        FingertipConds_Stiff[0] = RightHandEngaged.transform.FindChildRecursive("Hand_ThumbTip");
        FingertipConds_Stiff[1] = RightHandEngaged.transform.FindChildRecursive("Hand_IndexTip");
        FingertipConds_Stiff[2] = RightHandEngaged.transform.FindChildRecursive("Hand_MiddleTip");
        FingertipConds_Stiff[3] = RightHandEngaged.transform.FindChildRecursive("Hand_RingTip");
        FingertipConds_Stiff[4] = RightHandEngaged.transform.FindChildRecursive("Hand_PinkyTip");
    }

    void Update()
    {
        if(RightHandTracked_Root == null)
        {
            if (RightHandTracked.GetComponent<OVRHand>().IsTracked)
            {
                RightHandTracked_Root = RightHandTracked.GetComponentInParent<Transform>();
                FingertipTracked[0] = RightHandTracked.transform.FindChildRecursive("Hand_ThumbTip");
                FingertipTracked[1] = RightHandTracked.transform.FindChildRecursive("Hand_IndexTip");
                FingertipTracked[2] = RightHandTracked.transform.FindChildRecursive("Hand_MiddleTip");
                FingertipTracked[3] = RightHandTracked.transform.FindChildRecursive("Hand_RingTip");
                FingertipTracked[4] = RightHandTracked.transform.FindChildRecursive("Hand_PinkyTip");
            }
        }
        else
        {
            if(!_SprayHandler.isEngaged)
            {
                if(CheckEngageCondition())
                {
                    Debug.Log("Engaged!");
                    RightHandTracked.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    RightHandTracked.GetComponent<OVRMeshRenderer>().enabled = false;
                    RightHandEngaged.GetComponent<SkinnedMeshRenderer>().sharedMaterial = EngagedHandMaterial;
                    _SprayHandler.InitOnEnabled();
                    _SprayHandler.isEngaged = true;
                }
            }
            else
            {
                if(CheckDisengageCondition())
                {
                    Debug.Log("Disengaged!");
                    RightHandEngaged.GetComponent<SkinnedMeshRenderer>().sharedMaterial = DisengagedHandMaterial;
                    RightHandTracked.GetComponent<OVRMeshRenderer>().enabled = true;
                    _SprayHandler.WrapupOnDisabled();
                    _SprayHandler.isEngaged = false;
                }
            }
        }
    }

    private bool CheckEngageCondition()
    {
        bool isEngaged = false;
        if ((RightHandEngaged_InitPos - RightHandTracked_Root.transform.position).magnitude < 0.05)
        {
            if (Quaternion.Angle(RightHandEngaged_InitRot, RightHandTracked_Root.transform.rotation) < 60)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((FingertipConds_Stiff[j].position - FingertipTracked[j].position).magnitude > 0.05)
                        break;
                }
                return true;
            }
        }
        return isEngaged;
    }

    private bool CheckDisengageCondition()
    {
        if ((RightHandEngaged_InitPos - RightHandTracked_Root.transform.position).magnitude > 0.30f || Quaternion.Angle(RightHandEngaged_InitRot, RightHandTracked_Root.transform.rotation) > 150f)
            return true;
        else
            return false;
    }
}