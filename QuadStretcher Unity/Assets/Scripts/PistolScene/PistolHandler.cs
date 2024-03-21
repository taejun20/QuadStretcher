/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class PistolHandler : MonoBehaviour
{
    [SerializeField]
    StretcherManager stretcherManager_Right;
    [SerializeField]
    Transform pistol, pistolOrigin, bullet, bulletOrigin;
    [SerializeField]
    Transform RightHandTrackedTransform, VirtualRightHandTransform;

    bool recoilingUp = false;
    bool recoilingDown = false;
    int recoiledCounter = 0;
    bool loaded = true;
    float timeOfShooting = 0;
    bool initializeQuadStretcher = false;

    void Start()
    {
        // Hide real hand tracking, replacing it to the virtual 'pistol-grabbed' hand.
        RightHandTrackedTransform.GetComponent<SkinnedMeshRenderer>().enabled = false;
        RightHandTrackedTransform.GetComponent<OVRMeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (RightHandTrackedTransform.Find("Bones") == null)
        {
            return;
        }

        Transform Hand_Index1 = RightHandTrackedTransform.Find("Bones/Hand_Start/Hand_Index1");
        Transform Hand_Index2 = RightHandTrackedTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2");
        Transform Hand_Index3 = RightHandTrackedTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3");
        Transform Hand_IndexTip = RightHandTrackedTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip");
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1").localPosition = Hand_Index1.localPosition;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1").localRotation = Hand_Index1.localRotation;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2").localPosition = Hand_Index2.localPosition;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2").localRotation = Hand_Index2.localRotation;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3").localPosition = Hand_Index3.localPosition;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3").localRotation = Hand_Index3.localRotation;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip").localPosition = Hand_IndexTip.localPosition;
        VirtualRightHandTransform.Find("Bones/Hand_Start/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip").localRotation = Hand_IndexTip.localRotation;

        // Initialize stretching modules after 100 ms of the pistol shooting
        if (Time.time - timeOfShooting > 0.1f)
        {
            if (initializeQuadStretcher)
            {
                float inputMagnitude = 0f;
                float[][] stretchGainMapping = new float[3][];
                stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                stretcherManager_Right.stretchGains = stretchGainMapping;
                stretcherManager_Right.AllTactorStretch(inputMagnitude);
                stretcherManager_Right.stretcherUpdate();
                Debug.Log("stretch_magnitude: " + inputMagnitude.ToString());
                initializeQuadStretcher = false;
            }
        }

        // Implementing recoiling of the pistol when fired.
        if (recoilingUp)
        {
            pistol.transform.localEulerAngles = pistol.transform.localEulerAngles + new Vector3(0f, -6f, 0f);
            pistol.transform.localPosition = pistol.transform.localPosition + new Vector3(0f, 0f, -0.012f);
            recoiledCounter++;
            if (recoiledCounter == 3)
            {
                recoilingUp = false;
                recoilingDown = true;
            }
        }
        else if (recoilingDown)
        {
            pistol.transform.localEulerAngles = pistol.transform.localEulerAngles + new Vector3(0f, 6f, 0f);
            pistol.transform.localPosition = pistol.transform.localPosition + new Vector3(0f, 0f, 0.012f);
            recoiledCounter--;
            if (recoiledCounter == 0)
            {
                recoilingDown = false;
                pistol.position = pistolOrigin.position;
                pistol.rotation = pistolOrigin.rotation;
            }
        }

        if (Hand_Index1.localRotation.z < -0.40f)     // pistol triggered
        {
            pistol.Find("Cube.004").localScale = new Vector3(70f, 100f, 100f);
            if (loaded)
            {
                bullet.position = bulletOrigin.position;
                bullet.rotation = bulletOrigin.rotation;
                bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                bullet.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                bullet.transform.parent = null;
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 10f, ForceMode.Impulse);

                timeOfShooting = Time.time;
                float inputMagnitude = 1f;
                float[][] stretchGainMapping = new float[3][];
                stretchGainMapping[0] = new float[] { -1f, -1f, -1f, -1f };
                stretchGainMapping[1] = new float[] { 0, 0, 0, 0 };
                stretchGainMapping[2] = new float[] { 0, 0, 0, 0 };
                stretcherManager_Right.stretchGains = stretchGainMapping;
                stretcherManager_Right.AllTactorStretch(inputMagnitude);
                stretcherManager_Right.stretcherUpdate();
                Debug.Log("stretch_magnitude: " + inputMagnitude.ToString());

                loaded = false;
                recoilingUp = true;
                initializeQuadStretcher = true;
            }
        }
        else
        {
            pistol.Find("Cube.004").localScale = new Vector3(100f, 100f, 100f);
            loaded = true;
        }
    }
}
