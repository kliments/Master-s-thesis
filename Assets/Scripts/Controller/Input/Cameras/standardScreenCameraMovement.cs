using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class standardScreenCameraMovement : MonoBehaviour {

    public float movementSpeed;
    public float rotationSpeed;

    private bool holdRightMouse = false;
    Vector3 lastMousePosition = new Vector3(0,0,0);

    private bool automaticMove = false;
    private bool lookAtSpecificPosition = false;
    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;
    private float currentTime = 0.0f;
    private float endTime = 0.0f;
    private Vector3 startRot = Vector3.zero;
    private Vector3 endRot = Vector3.zero;
    private Vector3 lookAtPos = Vector3.zero;
    
    private bool checkIfInputFieldIsFocused()
    {
        UnityEngine.UI.InputField[] inputs = GameObject.FindObjectsOfType<UnityEngine.UI.InputField>();
        foreach(UnityEngine.UI.InputField input in inputs)
        {
            if (input.isFocused)
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update () {
        //Vector3 mainCameraLocation = Camera.main.transform.localPosition;
        if (automaticMove)
        {
            currentTime += Time.deltaTime;
            float percentage = (currentTime / endTime);
            this.transform.position = Vector3.Lerp(startPos, endPos, percentage);
            if (!lookAtSpecificPosition)
            {
                this.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, endRot.x, percentage),
                Mathf.LerpAngle(startRot.y, endRot.y, percentage),
                Mathf.LerpAngle(startRot.z, endRot.z, percentage));
            }
            else
            {
                this.transform.LookAt(lookAtPos);
            }
            
            if (currentTime >= endTime)
            {
                automaticMove = false;
                lookAtSpecificPosition = false;
            }
        }
        else
        {
            if (!checkIfInputFieldIsFocused())
            {
                if (Input.GetKey("a"))
                {
                    Camera.main.transform.Translate(-Vector3.right * movementSpeed * Time.deltaTime);
                }
                if (Input.GetKey("d"))
                {
                    Camera.main.transform.Translate(-Vector3.left * movementSpeed * Time.deltaTime);
                }
                if (Input.GetKey("w"))
                {
                    Camera.main.transform.Translate(Vector3.forward * movementSpeed * 2 * Time.deltaTime);
                }
                if (Input.GetKey("s"))
                {
                    Camera.main.transform.Translate(Vector3.back * movementSpeed * 2 * Time.deltaTime);
                }
                if (Input.GetKey("e"))
                {
                    Camera.main.transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
                }
                if (Input.GetKey("q"))
                {
                    Camera.main.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
                }
            }
        }

        if (!checkIfInputFieldIsFocused())
        {
            // Positions make sense in scene 2_2
            if (Input.GetKey("1"))
            {
                moveToPosition(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            }
            if (Input.GetKey("2"))
            {
                moveToPosition(new Vector3(16.0f, 1.0f, 18.0f), new Vector3(2.0f, -54.0f, 0.0f), 1.0f);
            }
            if (Input.GetKey("3"))
            {
                moveToPosition(new Vector3(-5.1f, 5.1f, 24.5f), new Vector3(22.6f, 154.7f, 0.0f), 1.0f);
            }
            if (Input.GetKey("4"))
            {
                moveToPositionWhileLookingAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(7.0f, 0.2f, 27.0f), 1.0f);
            }
            if (Input.GetKey("5"))
            {
                moveToPositionWhileLookingAt(new Vector3(16.0f, 1.0f, 18.0f), new Vector3(7.0f, 0.2f, 27.0f), 1.0f);
            }
            if (Input.GetKey("6"))
            {
                moveToPositionWhileLookingAt(new Vector3(-5.1f, 5.1f, 24.5f), new Vector3(7.0f, 0.2f, 27.0f), 1.0f);
            }
        }
        
        if (Input.GetMouseButton(1))
        {
            
            if (!holdRightMouse)
            {
                holdRightMouse = true;
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector3 currPos = Input.mousePosition;
                Vector3 diff = currPos - lastMousePosition;
                if (diff.x < -50) diff.x = -50;
                if (diff.x > 50) diff.x = 50;
                if (diff.y < -50) diff.y = -50;
                if (diff.y > 50) diff.y = 50;
                Vector3 rotation = new Vector3(-diff.y * rotationSpeed, diff.x * rotationSpeed, 0);
                Camera.main.transform.Rotate(rotation);
                Vector3 rot = Camera.main.transform.eulerAngles;
                rot.z = 0;
                Camera.main.transform.eulerAngles = rot;
                lastMousePosition = Input.mousePosition;
            }
        }
        else
        {
            holdRightMouse = false;
        }

       
        
    }

    


    public void moveToPosition(Vector3 newPos, Vector3 newRot, float time)
    {
        startPos = this.transform.position;
        endPos = newPos;
        currentTime = 0.0f;
        endTime = time;
        startRot = this.transform.eulerAngles;
        endRot = newRot;
        lookAtSpecificPosition = false;
        automaticMove = true;
    }

    public void moveToPositionWhileLookingAt(Vector3 newPos, Vector3 lookAtPosition, float time)
    {
        startPos = this.transform.position;
        endPos = newPos;
        currentTime = 0.0f;
        endTime = time;
        automaticMove = true;
        this.lookAtPos = lookAtPosition;
        lookAtSpecificPosition = true;
    }


}
