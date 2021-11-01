using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerToolTipRaycastChecker : MonoBehaviour
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

                //Debug.Log($"{ray},{Camera.main.ScreenPointToRay(Input.mousePosition)}");

                if (Physics.Raycast(ray,
                    out hitinfo,
                    Camera.main.farClipPlane,
                   layermask))
                {
                    Node hit_node = hitinfo.collider.gameObject.GetComponent<Node>();
                    Tower tower = hit_node.m_Tower;
                    if(tower != null)
                        TowerToolTipManager.Instance.ActivateToolTip(
                            tower.transform.position,
                            tower,
                            tower.ExcelData);
                }
            }
        }           
    }
}
