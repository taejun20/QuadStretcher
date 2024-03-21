/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class Slingshot_CheckHandEngage : MonoBehaviour
{
    [SerializeField]
    GameObject RightHandTracked, RightHandEngaged;
    [SerializeField]
    Material EngagedHandMaterial;
    [SerializeField]
    Material DisengagedHandMaterial;
    [SerializeField]
    SlingshotHandler _SlingshotHandler;

    Transform[] FingertipConds = new Transform[5];
    Transform[] FingertipTracked = new Transform[5];
    Transform RightHandTracked_Root;
    bool Engaged = false;

    // Start is called before the first frame update
    void Start()
    {
        FingertipConds[0] = RightHandEngaged.transform.FindChildRecursive("Hand_ThumbTip");
        FingertipConds[1] = RightHandEngaged.transform.FindChildRecursive("Hand_IndexTip");
        FingertipConds[2] = RightHandEngaged.transform.FindChildRecursive("Hand_MiddleTip");
        FingertipConds[3] = RightHandEngaged.transform.FindChildRecursive("Hand_RingTip");
        FingertipConds[4] = RightHandEngaged.transform.FindChildRecursive("Hand_PinkyTip");
    }

    // Update is called once per frame
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
            if(!Engaged)
            {
                if(CheckEngageCondition())
                {
                    Debug.Log("Engaged");

                    Engaged = true;
                    RightHandTracked.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    RightHandTracked.GetComponent<OVRMeshRenderer>().enabled = false;
                    RightHandEngaged.GetComponent<SkinnedMeshRenderer>().sharedMaterial = EngagedHandMaterial;                    
                    _SlingshotHandler.InitOnEnabled();
                    _SlingshotHandler.slingshotGrabbed = true;
                }
            }
            else
            {
                if(CheckDisengageCondition())
                {
                    Debug.Log("Disengaged");

                    Engaged = false;
                    RightHandEngaged.GetComponent<SkinnedMeshRenderer>().sharedMaterial = DisengagedHandMaterial;
                    RightHandTracked.GetComponent<OVRMeshRenderer>().enabled = true;
                    _SlingshotHandler.WrapupOnDisabled();
                    _SlingshotHandler.slingshotGrabbed = false;
                }
            }
        }
    }

    private bool CheckEngageCondition()
    {
        if ((RightHandEngaged.transform.position - RightHandTracked_Root.transform.position).magnitude < 0.25)
        {
            if (Quaternion.Angle(RightHandEngaged.transform.rotation, RightHandTracked_Root.transform.rotation) < 60)
            {
                for (int i = 0; i < 5; i++)
                {
                    if ((FingertipConds[i].position - FingertipTracked[i].position).magnitude > 0.05)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    private bool CheckDisengageCondition()
    {
        /*
        if ((RightHandEngaged.transform.position - RightHandTracked_Root.transform.position).magnitude > 5f || Quaternion.Angle(RightHandEngaged.transform.rotation, RightHandTracked_Root.transform.rotation) > 160f)
        {
            return true;
        }
        */
        return false;
    }
}
