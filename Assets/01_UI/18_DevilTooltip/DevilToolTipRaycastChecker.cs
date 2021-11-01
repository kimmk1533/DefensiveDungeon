using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevilToolTipRaycastChecker : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        //
        // check click world position (for summoned towers)
        // when tower hit by raycast
        // activate tooltip
        if (Input.GetMouseButtonDown(1))
        {
            /// perspective only
            // if mouse is out of UI 
            if (false == EventSystem.current.IsPointerOverGameObject())
            {
                // raycast ( mouse -> summoned tower)
                Vector3 mouse_pos = Input.mousePosition;
                mouse_pos.z = Camera.main.farClipPlane;

                int layermask = 1 << LayerMask.NameToLayer("Node");
                RaycastHit hitinfo;
                Ray ray = new Ray(Camera.main.transform.position,
                    Camera.main.ScreenToWorldPoint(mouse_pos));

                //if (Physics.Raycast(ray, out hitinfo))
                //    Debug.Log($"DevilToolTipRaycastChecker : {hitinfo.collider.name}");

                if (Physics.Raycast(ray,
                    out hitinfo,
                    Camera.main.farClipPlane,
                   layermask))
                {
                    Node hit_node = hitinfo.collider.gameObject.GetComponent<Node>();
                    Devil hit_devil = hit_node.m_Devil;
                    if (hit_devil != null)
                        DevilToolTipManager.Instance.ActivateToolTip(
                            hit_devil.transform.position
                            );
                }
            }
        }
    }
}
