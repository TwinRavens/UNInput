using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniversalNetworkInput;
using UniversalNetworkInput.Network;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject backgroundSprite;
    public GameObject outlineSprite;
    public GameObject pointerSprite;
    public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the Network Input
    public string verticalAxisName = "Vertical";     // The name given to the vertical axis for the Network Input

    //Private Variables
    [SerializeField]
    float movementRange = 160;
    [SerializeField]
    bool dragable = true;
    GameObject m_background_instance;
    GameObject m_outline_instance;
    GameObject m_pointer_instance;
    VirtualInput m_input;
    Vector3 m_initial_position;
    Vector3 m_last_position;

    // Use this for initialization
    void Start()
    {
        m_input = UNClient.control;
        m_initial_position = transform.position;
        movementRange *= (Screen.width / 800.0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Define object position
        transform.position = m_last_position = eventData.position;

        //Instantiate Joystick parts with assigned parent
        if (backgroundSprite)
            m_background_instance = Instantiate(backgroundSprite, transform);
        if (outlineSprite)
            m_outline_instance = Instantiate(outlineSprite, transform);
        if (pointerSprite)
            m_pointer_instance = Instantiate(pointerSprite, transform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Reset pointer position to initial position
        transform.position = m_initial_position;

        //Destroy every instantiated objects
        if (m_background_instance)
            Destroy(m_background_instance);
        if (m_outline_instance)
            Destroy(m_outline_instance);
        if (m_pointer_instance)
            Destroy(m_pointer_instance);

        //Reset UNInput axis to zero
        //(SetAxisZero will use reliable connection ensure arrival)
        m_input.SetAxisZero(horizontalAxisName);
        m_input.SetAxisZero(verticalAxisName);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Reset to Initial position
        transform.position = m_last_position;

        //Get Vector3 of pointer delta from last frame
        Vector3 touch_delta = eventData.delta;

        //Get Vector3 of pointer position
        Vector3 touch_position = eventData.position;

        //A vector comming from joystick center to touch's current position
        Vector3 difference = (touch_position - m_last_position);

        //Update pointer position
        m_pointer_instance.transform.position = m_last_position + difference;

        //Get the distance of touch from joystick current center
        float distance = difference.magnitude;

        //If pointer went too far away
        if (distance > movementRange)
        {
            //If draggable, update transform, otherwise, limit pointer
            if (dragable)
            {
                //A vector that makes our outline be in reach
                Vector3 reach = difference.normalized * (distance - movementRange);

                //Drag transform as much as it is needed to be in reach
                transform.position += reach;

                //Override pointer position
                m_pointer_instance.transform.position = m_last_position + difference;

                //Update last position with current position
                m_last_position += reach;
            }
            else
            {
                //Update pointer position to maximum reach position
                m_pointer_instance.transform.position = m_last_position + difference.normalized * movementRange;
            }
        }

        //Update UNInput axis
        m_input.SetAxis(horizontalAxisName, Mathf.Min(difference.x, movementRange) / movementRange);
        m_input.SetAxis(verticalAxisName, Mathf.Min(difference.y, movementRange) / movementRange);
    }
}