using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameEndData
{
    public bool IsWin;
}

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_win_lose_textpro;
    [SerializeField] TMPro.TextMeshProUGUI m_textTextpro;
    [SerializeField] GameOverDataLine m_line1;
    [SerializeField] GameOverDataLine m_line2;
    [SerializeField] GameOverDataLine m_line3;
    [SerializeField] GameOverProgressBar m_progressBar;

   

    public void SetUI(GameEndData data)
    {
        GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero; //Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

        if (data.IsWin)
        {
            m_win_lose_textpro.text = "�¸�";
            m_textTextpro.text = "�� ���� ���� ������ ���Ƴ��� ���ձ��� �¸��߽��ϴ�.";
            m_line1.SetUI("�¸� ���ʽ�", 300);
            m_line2.SetUI("���̺� ���ʽ�", 400);
            m_line3.SetUI("�� ����ġ", 300 + 400);
            m_progressBar.SetUI(700, 1000);
        }
        else
        {
            m_win_lose_textpro.text = "�й�";
            m_textTextpro.text = "���ձ��� �ᱹ ������ ���Ƴ��� ���߽��ϴ�.";
            m_line1.SetUI("�¸� ���ʽ�", 0);
            m_line2.SetUI("���̺� ���ʽ�", 200);
            m_line3.SetUI("�� ����ġ", 0 + 200);
            m_progressBar.SetUI(200, 1000);
        }
    }

    public void __OnToMainButtonClicked()
    {
        // TODO...
        // TODO : call all finalize first

        Debug.Log("---------------------------GAME END---------------------------");
    }
}
