using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomEventSystemInput : MonoBehaviour {

    private InputManager m_Input;

    // Use this for initialization
    void Start ()
    {
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        AxisEventData currentAxis = new AxisEventData(EventSystem.current);
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;

        if (m_Input.GetButtonDown(InputManager.EBUTTON.DPadUp)) // move up
        {
            currentAxis.moveDir = MoveDirection.Up;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
        }
        else if (m_Input.GetButtonDown(InputManager.EBUTTON.DPadDown)) // move down
        {
            currentAxis.moveDir = MoveDirection.Down;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
        }
        else if (m_Input.GetButtonDown(InputManager.EBUTTON.DPadRight)) // move right
        {
            currentAxis.moveDir = MoveDirection.Right;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
        }
        else if (m_Input.GetButtonDown(InputManager.EBUTTON.DPadLeft)) // move left
        {
            currentAxis.moveDir = MoveDirection.Left;
            ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
        }
    }
}
