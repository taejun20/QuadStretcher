/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 20
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class ArcheryHandler : MonoBehaviour
{
    [SerializeField]
    StretcherManager stretcherManager_Right;
    [SerializeField]
    Transform bowStringGrab, bowStringGrabOrigin, rightHandCenterTransform, arrowOrigin;
    [SerializeField]
    GameObject arrow, virtualArrow, target;
    [SerializeField]
    Transform LeftHandTrackedTransform, RightHandTrackedTransform, VirtualRightHandTransform;    
    [SerializeField]
    Transform markerForBowPlane1, markerForBowPlane2, markerForBowPlane3;

    bool bowStringGrabbed = false;      // if the bow string is grabbed at the moment
    bool rightHandGrabCond = false;     // if the right hand is grabbing
    Vector3 arrowPulledDeltaVector3;
    void Start()
    {
        target.transform.position = arrowOrigin.position + new Vector3(0f, 6f, 10f);
        arrow.SetActive(false);
        virtualArrow.SetActive(true);
        VirtualRightHandTransform.GetComponent<SkinnedMeshRenderer>().enabled = false;
        VirtualRightHandTransform.GetComponent<OVRMeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (LeftHandTrackedTransform.Find("Capsules") == null || RightHandTrackedTransform.Find("Capsules") == null || VirtualRightHandTransform.Find("Capsules") == null)
        {
            return;
        }
        else    // Removing rigidbodies that can potentially collide with arrow.
        {
            LeftHandTrackedTransform.Find("Capsules").gameObject.SetActive(false);
            RightHandTrackedTransform.Find("Capsules").gameObject.SetActive(false);
            VirtualRightHandTransform.Find("Capsules").gameObject.SetActive(false);
        }

        float indexFingerBentValue = RightHandTrackedTransform.Find("Bones").FindChildRecursive("Hand_Index1").localRotation.z;
        float middleFingerBentValue = RightHandTrackedTransform.Find("Bones").FindChildRecursive("Hand_Middle1").localRotation.z;
        float ringFingerBentValue = RightHandTrackedTransform.Find("Bones").FindChildRecursive("Hand_Ring1").localRotation.z;
        rightHandGrabCond = (indexFingerBentValue < -0.40f) && (middleFingerBentValue < -0.50f) && (ringFingerBentValue < -0.40f);  // Check if the right hand is grabbing the arrow for shooting
        if (!bowStringGrabbed)
        {
            float dist = Vector3.Distance(rightHandCenterTransform.position, arrowOrigin.position);
            if (dist < 0.1f)
            {
                if (!virtualArrow.activeSelf)
                    virtualArrow.SetActive(true);

                virtualArrow.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32(0x0A, 0xFF, 0x00, 0xFF);
                virtualArrow.transform.GetChild(1).GetComponent<Renderer>().material.color = new Color32(0x0A, 0xFF, 0x00, 0xFF);

                if (rightHandGrabCond)
                {
                    bowStringGrabbed = true;

                    VirtualRightHandTransform.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    VirtualRightHandTransform.GetComponent<OVRMeshRenderer>().enabled = true;
                    RightHandTrackedTransform.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    RightHandTrackedTransform.GetComponent<OVRMeshRenderer>().enabled = false;

                    float inputMagnitude = 0f;
                    float[][] stretchGainMapping = new float[3][];
                    stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                    stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                    stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                    stretcherManager_Right.stretchGains = stretchGainMapping;
                    stretcherManager_Right.AllTactorStretch(inputMagnitude);
                    stretcherManager_Right.stretcherUpdate();
                    Debug.Log("stretch_magnitude: " + inputMagnitude.ToString());

                    if (!arrow.activeSelf)
                        arrow.SetActive(true);
                    virtualArrow.SetActive(false);
                    arrow.GetComponent<Rigidbody>().isKinematic = false;    // to get affected by gravity while flying
                    arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    arrow.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    arrow.transform.position = arrowOrigin.position;
                    arrow.transform.rotation = arrowOrigin.rotation;
                }
            }
            else
            {
                if (dist < 0.15f)   // Regenerate the virtual arrow in the bow after a shoot
                {
                    if (!virtualArrow.activeSelf)
                        virtualArrow.SetActive(true);
                }
                virtualArrow.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                virtualArrow.transform.GetChild(1).GetComponent<Renderer>().material.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
        }
        else
        {
            if (!rightHandGrabCond)
            {
                bowStringGrabbed = false;
                bowStringGrab.position = bowStringGrabOrigin.position;

                arrow.GetComponent<Rigidbody>().AddForce(arrowPulledDeltaVector3 * -1 * 80f, ForceMode.Impulse);
                arrow.transform.parent = null;

                VirtualRightHandTransform.GetComponent<SkinnedMeshRenderer>().enabled = false;
                VirtualRightHandTransform.GetComponent<OVRMeshRenderer>().enabled = false;
                RightHandTrackedTransform.GetComponent<SkinnedMeshRenderer>().enabled = true;
                RightHandTrackedTransform.GetComponent<OVRMeshRenderer>().enabled = true;

                float inputMagnitude = 0f;
                float[][] stretchGainMapping = new float[3][];
                stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                stretcherManager_Right.stretchGains = stretchGainMapping;
                stretcherManager_Right.AllTactorStretch(inputMagnitude);
                stretcherManager_Right.stretcherUpdate();
                Debug.Log("stretch_magnitude: " + inputMagnitude.ToString());

                stretcherManager_Right.AllTactorStretch(0f);
                stretcherManager_Right.stretcherUpdate();
            }
            else
            {
                // calculate projected location of the rightHandTracked towards the 2D plane of the bow
                Vector3 bowPlaneNormalVector = Vector3.Normalize(markerForBowPlane2.position - markerForBowPlane1.position);
                Vector3 projectedPos = Vector3.ProjectOnPlane(rightHandCenterTransform.position, bowPlaneNormalVector) + Vector3.Dot(markerForBowPlane1.position, bowPlaneNormalVector) * bowPlaneNormalVector;

                // calculate projected location of the rightHandTracked towards the 2D plane of the arrow
                Vector3 bowPlane2NormalVector = Vector3.Normalize(markerForBowPlane3.position - markerForBowPlane1.position);
                Vector3 projectedPos2 = Vector3.ProjectOnPlane(projectedPos, bowPlane2NormalVector) + Vector3.Dot(markerForBowPlane1.position, bowPlane2NormalVector) * bowPlane2NormalVector;

                bowStringGrab.position = projectedPos2;
                VirtualRightHandTransform.position = projectedPos2 + (RightHandTrackedTransform.position - rightHandCenterTransform.position); ;
                arrow.transform.position = projectedPos2;
                arrow.transform.rotation = arrowOrigin.rotation;
                arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
                arrow.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                arrowPulledDeltaVector3 = arrow.transform.position - arrowOrigin.position;  // max: 0.25, min: 0

                float inputMagnitude = calculateInputMagnitude(arrowPulledDeltaVector3.magnitude);
                float[][] stretchGainMapping = new float[3][];
                stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                stretcherManager_Right.stretchGains = stretchGainMapping;
                stretcherManager_Right.AllTactorStretch(inputMagnitude);
                stretcherManager_Right.stretcherUpdate();
                Debug.Log("stretch_magnitude: " + inputMagnitude.ToString());
            }
        }
    }

    float calculateInputMagnitude(float arrowPulledDeltaVectorMagnitude)
    {
        if (arrowPulledDeltaVectorMagnitude > 0.25)
            return 1f;
        else if (arrowPulledDeltaVectorMagnitude >= 0)
            return arrowPulledDeltaVectorMagnitude * 4f;
        else
            return 0f;
    }
}
