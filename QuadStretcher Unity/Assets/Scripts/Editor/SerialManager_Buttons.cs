/*
* Writer: Taejun Kim (https://taejunkim.com/), Youngbo Aram Shim, HCI Lab KAIST
* Last Update: 2024. 3. 21
* QuadStretcher: A Forearm-Worn Skin Stretch Display for Bare-Hand Interaction in AR/VR (ACM CHI 2024)
* ACM CHI 24': Conference on Human Factors in Computing Systems.
*/

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SerialManager))]
public class SerialManager_Buttons : Editor
{
	[HideInInspector] public int selected;
	public override void OnInspectorGUI()
	{
		SerialManager sm = (SerialManager)target;
		if(GUILayout.Button("Search Port")){
			sm.clickSearchPort();
		}

		string[] options = sm.SerialPortList.ToArray();
		sm.selectedPortIndex = EditorGUILayout.Popup("Port", sm.selectedPortIndex, options);

		if (GUILayout.Button("Connect"))
		{
			sm.clickSerialConnect();
		}
	
		base.OnInspectorGUI();
	}
}
