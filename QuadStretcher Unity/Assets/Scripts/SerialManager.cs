/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;

public class SerialManager : MonoBehaviour
{
    [HideInInspector] public List<string> SerialPortList = new List<string>();
    [HideInInspector] public int selectedPortIndex = 0;
    public static SerialPort SerialPort = new SerialPort();

    public void clickSearchPort()
    {
        // Get available serial ports
        int p = (int)System.Environment.OSVersion.Platform;
        List<string> serialPorts = new List<string>();

        // Are we on Unix
        if (p == 4 || p == 128 || p == 6)
        {
            string[] ttys = Directory.GetFiles("/dev/", "cu.*");
            foreach (string dev in ttys)
            {
                serialPorts.Add(dev);
                Debug.Log(string.Format(dev));
            }
        }
        else
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                serialPorts.Add(port);
            }
        }

        // Display ports in combobox
        SerialPortList.Clear();
        for (int i =0; i<serialPorts.Count; i++)
            SerialPortList.Add(serialPorts[i]);
        if (serialPorts.Count > 0)
        {
            SerialPort.BaudRate = 115200;
            SerialPort.DtrEnable = true;
            SerialPort.RtsEnable = true;
        }

        selectedPortIndex = SerialPortList.Count - 1;
    }

    public void clickSerialConnect()
    {
        // Connect to dropdown selected serial port
        if (!SerialPort.IsOpen)
        {
            SerialPort.PortName = SerialPortList[selectedPortIndex];
            SerialPort.Open();
            string line = SerialPort.ReadExisting();
            Debug.Log("Serial connected: " + SerialPort.PortName);
        }
        else
        {
            SerialPort.Close();
        }
    }

    private void OnApplicationQuit()
    {
        if (SerialPort.IsOpen)
            SerialPort.Close();
    }
}
