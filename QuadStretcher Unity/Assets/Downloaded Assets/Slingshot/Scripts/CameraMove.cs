using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public bool brakeMouse = true; //Controls whether the mouse cursor is displayed
    public float sensibility = 2.0f; //Controls the sensitivity of the mouse

    private float mouseX = 0.0f, mouseY = 0.0f; //Variables that controls the mouse rotation

    void Start()
    {
        if (!brakeMouse)
        {
            return;
        }

        Cursor.visible = false; //Hides the mouse cursor
        Cursor.lockState = CursorLockMode.Locked; //Hold center cursor
    }


    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * sensibility; //Increases X-axis value and multiplies by sensitivity
        mouseY -= Input.GetAxis("Mouse Y") * sensibility; // Increases the value of the Y axis and multiplies by the sensitivity. (Note that we use the - to invert the values)

        transform.eulerAngles = new Vector3(mouseY, mouseX, 0); //Rotate the camera according to the axes
    }
}
