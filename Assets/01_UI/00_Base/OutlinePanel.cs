using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OutlinePanel : MonoBehaviour
{
    [System.Serializable]
    public enum EOutLineAnckor
    {
        Center,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }

    [SerializeField] Image m_body;
    [SerializeField] EOutLineAnckor m_anckor;
    [SerializeField] Vector2 m_paddingSize;

#if UNITY_EDITOR
    private void Update()
    {
        if (m_body != null)
        {
            RectTransform body_tf = m_body.GetComponent<RectTransform>();
            RectTransform this_tf = this.GetComponent<RectTransform>();

            // position
            this_tf.position = body_tf.position;
            // size
            this_tf.sizeDelta = body_tf.rect.size + m_paddingSize;
            // anckor
            this_tf.anchorMin = body_tf.anchorMin;
            this_tf.anchorMax = body_tf.anchorMax;
            // pivot
            this_tf.pivot = body_tf.pivot;

            switch (m_anckor)
            {
                case EOutLineAnckor.Center:
                    this_tf.position = body_tf.position;
                    break;
                case EOutLineAnckor.LeftTop:
                    this_tf.position += new Vector3(m_paddingSize.x, -m_paddingSize.y, 0);
                    break;
                case EOutLineAnckor.RightTop:
                    this_tf.position += new Vector3(-m_paddingSize.x, -m_paddingSize.y, 0);
                    break;
                case EOutLineAnckor.LeftBottom:
                    this_tf.position += new Vector3(m_paddingSize.x, m_paddingSize.y, 0);
                    break;
                case EOutLineAnckor.RightBottom:
                    this_tf.position += new Vector3(-m_paddingSize.x, m_paddingSize.y, 0);
                    break;
                default:
                    break;
            }
        }
    }
#endif
}
