/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;

public class StretcherManager : MonoBehaviour
{
    public SerialManager serialManager;
    public float[][] stretchGains = new float[3][];   // dorsal, left, ventral, right order
    public int n_DoF = 1;
    public bool IsLeft = false;

    private Vector3 stretchOrientation = new Vector3(); // max magnitude: 1, x: yaw, y: pitch, z: forward
    private int[] tactorDegrees = new int[8]; // order: dist_dorsal, dist_left, dist_ventral, dist_right, prox_dorsal, prox_left, prox_ventral, prox_right
    private int startIdx = 0;

    int[][] DegPerDoF = new int[3][];  //gain for each forward, right, up

    // Start is called before the first frame update
    void Start()
    {
        if (IsLeft)
            startIdx = 0;
        else
            startIdx = 4;
        for (int i = 0; i < 8; i++)
        {
            tactorDegrees[i] = 90;
        }

        for (int i = 0; i < 3; i++)
        {
            stretchGains[i] = new float[] { 0, 0, 0, 0 };
            DegPerDoF[i] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }

    private void OnApplicationQuit()
    {
        turnOffServos();
    }

    public void updateStretcher(Vector3 absStretchDir, bool fishingOrTennis)
    {
        stretchOrientation = absStretchDir;

        //Update gain for each DoF
        for (int i = 0; i < n_DoF; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                DegPerDoF[i][j] = (int)(stretchGains[i][j] * 90f);
            }
        }

        dirToDegree(fishingOrTennis);
    }  

    public void updateStretcher(int[] degree)
    {
        for (int i = 0; i < 4; i++)
        {
            tactorDegrees[i] = degree[i];
        }
    }

    public void turnOffServos()
    {
        if (SerialManager.SerialPort.IsOpen)
        {
            lock (SerialManager.SerialPort)
            {
                SerialManager.SerialPort.WriteLine("t0p090");
                Debug.Log("Servo neutral");
                SerialManager.SerialPort.WriteLine("t0p200");
            }
        }
    }

    void dirToDegree(bool fishingOrTennis)
    {
        for (int i = 0; i < 4; i++)
        {
            tactorDegrees[i] = 90 + (int)(stretchOrientation.x * DegPerDoF[1][i] + stretchOrientation.y * DegPerDoF[2][i] + stretchOrientation.z * DegPerDoF[0][i]);
        }
        if (fishingOrTennis)
		{
            // 8 -> 7, 7 -> 6, 6 -> 5, 5 -> 8
            int tactor5DegreeTemp = tactorDegrees[0];
            tactorDegrees[0] = tactorDegrees[1];
            tactorDegrees[1] = tactorDegrees[2];
            tactorDegrees[2] = tactorDegrees[3];
            tactorDegrees[3] = tactor5DegreeTemp;
        }
    }

    public void stretcherUpdate()
    {
        if (SerialManager.SerialPort.IsOpen)
        {
            lock (SerialManager.SerialPort)
            {
                for (int i = 0; i < 4; i++)
                {
                    tactorDegrees[i] = tactorDegrees[i] > 160 ? 160 : tactorDegrees[i];
                    tactorDegrees[i] = tactorDegrees[i] < 20 ? 20 : tactorDegrees[i];
                    string cmd = string.Format("t{0}p{1,3:D3}", i + 1 + startIdx, tactorDegrees[i]);
                    SerialManager.SerialPort.WriteLine(cmd);
                    //Debug.Log(cmd);
                }
            }
        }
    }

    public void AllTactorStretch(float magnitude)
    {
        if (n_DoF == 1)
        {
            int[] stretchDeg = new int[4];
            // Update stretchGain
            for (int i = 0; i < 4; i++)
            {
                stretchDeg[i] = (int)(90 + stretchGains[0][i] * magnitude * (90 - 20));
            }

            updateStretcher(stretchDeg);
        }
    }
}
