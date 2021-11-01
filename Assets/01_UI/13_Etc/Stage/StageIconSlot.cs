using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIconSlot : MonoBehaviour
{
    [SerializeField] Sprite_TableExcelLoader m_sprite_loader;
    [SerializeField] Image m_image;
    private RectTransform m_rt;

    private void Awake()
    {
        m_rt = this.GetComponent<RectTransform>();
        m_rt.anchorMin = new Vector2(0f, 1f);
        m_rt.anchorMax = new Vector2(0f, 1f);
    }

    public void SetSize(Vector2 size)
    {
        m_rt.sizeDelta = size;
    }

    public void SetPosition(float anckor_x)
    {        
        m_rt.anchoredPosition = new Vector2(anckor_x, -10);
    }
    public float GetAnckorX()
    {
        return m_rt.anchoredPosition.x;
    }

    public void ChangeImage(int sprite_code)
    {
        Sprite sprite = m_sprite_loader.GetSprite(sprite_code);
        m_image.sprite = sprite;
    }

    // 0 -1f
    public void SetImageAlpha(float alpha)
    {
        Color color = m_image.color;
        color.a = alpha;
        m_image.color = color;
    }

    public Color GetImageColor()
    {
        return m_image.color;
    }
    public void SetImageColor(Color color)
    {
        m_image.color = color;
    }
}
