using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct MouseArgs
{
    public Vector2 beforePos;
    public Vector2 currentPos;

    public bool isLeftDown;
    public bool isRightDown;
}

//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.
public class MouseInputManager : Singleton<MouseInputManager>
{

    [SerializeField] Canvas m_canvas;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public delegate void MouseEventHandler(GameObject _selected, MouseArgs args);
    Dictionary<string, MouseEventHandler> MouseLeftDownEvent;
    Dictionary<string, MouseEventHandler> MouseLeftDoubleDownEvent;
    Dictionary<string, MouseEventHandler> MouseLeftUpEvent;
    Dictionary<string, MouseEventHandler> MouseLeftHoldEvent;

    Dictionary<string, MouseEventHandler> MouseRightDownEvent;
    Dictionary<string, MouseEventHandler> MouseRightDoubleDownEvent;
    Dictionary<string, MouseEventHandler> MouseRightUpEvent;
    Dictionary<string, MouseEventHandler> MouseRightHoldEvent;
    
    List<string> left_up_taglist;
    List<string> left_down_taglist;
    List<string> left_double_down_taglist;
    List<string> left_Hold_taglist;

    List<string> right_up_taglist;
    List<string> right_down_taglist;
    List<string> right_double_down_taglist;
    List<string> right_Hold_taglist;

    [SerializeField] GameObject current_gameObj;
    [SerializeField] MouseArgs current_args;

    [SerializeField] float current_mousedown_time;  // double click üũ��, ù down ��  �ι�° down�� ���̷� �Ǻ���
    [SerializeField] float tolerance;       // double click ���� �Ǵ��ϴ� �ð� ��
    
    string temptag;
    bool findtag;

    public void __Initialize()
    {
        MouseLeftDownEvent = new Dictionary<string, MouseEventHandler>();
        MouseLeftUpEvent = new Dictionary<string, MouseEventHandler>();
        MouseLeftDoubleDownEvent = new Dictionary<string, MouseEventHandler>();
        MouseLeftHoldEvent = new Dictionary<string, MouseEventHandler>();

        MouseRightDownEvent = new Dictionary<string, MouseEventHandler>();
        MouseRightUpEvent = new Dictionary<string, MouseEventHandler>();
        MouseRightHoldEvent = new Dictionary<string, MouseEventHandler>();
        MouseRightDoubleDownEvent = new Dictionary<string, MouseEventHandler>();

        left_up_taglist = new List<string>();
        left_down_taglist = new List<string>();
        left_double_down_taglist = new List<string>();
        left_Hold_taglist = new List<string>();

        right_up_taglist = new List<string>();
        right_down_taglist = new List<string>();
        right_double_down_taglist = new List<string>();
        right_Hold_taglist = new List<string>();

        temptag = null;
        findtag = false;
        if(tolerance <= 0.5f)
            tolerance = 0.5f;
    }
    public void RightDown_Regist(string _tag, MouseEventHandler _handler)
    {
        right_down_taglist.Add(_tag);
        MouseRightDownEvent.Add(_tag, _handler);
    }
    public void RightUp_Regist(string _tag, MouseEventHandler _handler)
    {
        right_up_taglist.Add(_tag);
        MouseRightUpEvent.Add(_tag, _handler);
    }
    public void RightDoubleDown_Regist(string _tag, MouseEventHandler _handler)
    {
        right_double_down_taglist.Add(_tag);
        MouseRightDownEvent.Add(_tag, _handler);
    }
    public void RightHold_Regist(string _tag, MouseEventHandler _handler)
    {
        right_Hold_taglist.Add(_tag);
        MouseRightHoldEvent.Add(_tag, _handler);
    }
    public void LeftUp_Regist(string _tag, MouseEventHandler _hendler)
    {
        left_up_taglist.Add(_tag);
        MouseLeftUpEvent.Add(_tag, _hendler);
    }
    public void LeftDown_Regist(string _tag, MouseEventHandler _hendler)
    {
        left_down_taglist.Add(_tag);
        MouseLeftDownEvent.Add(_tag, _hendler);
    }
    public void LeftDoubleDown_Regist(string _tag, MouseEventHandler _hendler)
    {
        left_double_down_taglist.Add(_tag);
        MouseLeftDoubleDownEvent.Add(_tag, _hendler);
    }
    public void LeftHold_Regist(string _tag, MouseEventHandler _hendler)
    {
        left_Hold_taglist.Add(_tag);
        MouseLeftHoldEvent.Add(_tag, _hendler);
    }
    private void Awake()
    {
        __Initialize();
        m_canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        current_args = new MouseArgs();
        current_mousedown_time = Time.time;
    }
    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = m_canvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = m_canvas.GetComponent<EventSystem>();
    }

    void Update()
    {
        current_args.beforePos = current_args.currentPos;
        current_args.currentPos = Input.mousePosition;

        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            current_args.isLeftDown = true;
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);


            if (results.Count > 0)
            {

                for (int i = 0; i < results.Count; i++)
                {
                    if (left_down_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }

                // ���� Ŭ������ �ð� ��

                float delta = Time.time - current_mousedown_time;

                if (delta < tolerance)
                {   // call double click event
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (left_double_down_taglist.Contains(results[i].gameObject.tag))
                        {
                            temptag = results[i].gameObject.tag;
                            findtag = true;
                            break;
                        }
                        else findtag = false;
                    }
                    current_mousedown_time = Time.time;
                    if (!findtag) return;
                    MouseLeftDoubleDownEvent[temptag]?.Invoke(results[0].gameObject, current_args);

                    return;
                }
                current_mousedown_time = Time.time;
                if (!findtag) return;
                current_gameObj = results[0].gameObject;

                MouseLeftDownEvent[temptag]?.Invoke(current_gameObj, current_args);

            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            current_args.isLeftDown = false;


            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (left_up_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }
                if (!findtag)
                    return;
                MouseLeftUpEvent[temptag]?.Invoke(results[0].gameObject, current_args);
            }

            current_gameObj = null;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            current_args.isRightDown = true;
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);


            if (results.Count > 0)
            {

                for (int i = 0; i < results.Count; i++)
                {
                    if (right_down_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }

                // ���� Ŭ������ �ð� ��

                float delta = Time.time - current_mousedown_time;

                if (delta < tolerance)
                {   // call double click event
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (right_double_down_taglist.Contains(results[i].gameObject.tag))
                        {
                            temptag = results[i].gameObject.tag;
                            findtag = true;
                            break;
                        }
                        else findtag = false;
                    }
                    current_mousedown_time = Time.time;
                    if (!findtag) return;
                    MouseRightDoubleDownEvent[temptag]?.Invoke(results[0].gameObject,  current_args);

                    return;
                }
                current_mousedown_time = Time.time;
                if (!findtag) return;
                current_gameObj = results[0].gameObject;

                MouseRightDownEvent[temptag]?.Invoke(current_gameObj, current_args);

            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            current_args.isRightDown = false;


            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (right_up_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }
                if (!findtag)
                    return;
                MouseRightUpEvent[temptag]?.Invoke(results[0].gameObject, current_args);
            }

            current_gameObj = null;
        }
        //Debug.Log(current_args.isLeftDown);
        if (current_args.isLeftDown)
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (left_Hold_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }
                if (!findtag)
                    return;
                MouseLeftHoldEvent[temptag]?.Invoke(current_gameObj, current_args);
            }
        }
        if (current_args.isRightDown)
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (right_Hold_taglist.Contains(results[i].gameObject.tag))
                    {
                        temptag = results[i].gameObject.tag;
                        findtag = true;
                        break;
                    }
                    else findtag = false;
                }
                if (!findtag)
                    return;
                MouseRightHoldEvent[temptag]?.Invoke(current_gameObj,current_args);
            }
        }
    }
}

