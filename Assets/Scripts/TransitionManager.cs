using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] TransitionDialog m_dialog = null;
    [SerializeField] TextMeshProUGUI m_infoText = null;
    string m_nextLevel = "";
    float m_messageDuration = 0.0f;
    float m_currentMessageTime = 0.0f;
    float m_fadeDuration = 2.0f;
    float m_fadeTime = 0.0f;
    int m_index = -1;
    bool m_paused = true;
    bool m_fadingOut = true;

    private void Start()
    {
        SetDialog(1.0f);
        SetNextLevel(m_dialog.m_nextLevel);
    }

    private void Update()
    {
        if (!m_paused)
        {
            if (Input.GetButton("SpeedText"))
            {
                TextManager.Instance.SpeedUpTextSpeed();
                m_messageDuration -= Time.deltaTime * 2.0f;
            }
            else if (Input.GetButtonUp("SpeedText"))
            {
                TextManager.Instance.SlowDownTextSpeed();
            }

            m_currentMessageTime += Time.deltaTime;
            if (m_currentMessageTime >= m_messageDuration)
            {
                if (m_index >= m_dialog.m_dialogInfo.Length - 1)
                {
                    m_currentMessageTime = 0.0f;
                    LoadNextLevel();
                }
                else
                {
                    ChangeDialog();
                }
            }
        }

        FadeInfo();
    }

    private void FadeInfo()
    {
        Color c = m_infoText.color;

        m_fadeTime += Time.deltaTime;
        if (m_fadingOut)
        {
            float a = 1.0f - (m_fadeTime / m_fadeDuration);
            c.a = a;
        }
        else
        {
            float a = (m_fadeTime / m_fadeDuration);
            c.a = a;
        }

        m_infoText.color = c;

        if (m_fadeTime >= m_fadeDuration)
        {
            m_fadeTime = 0.0f;
            m_fadingOut = !m_fadingOut;
        }
    }

    private void ChangeDialog()
    {
        ++m_index;
        TransitionDialog.DialogInfo info = m_dialog.m_dialogInfo[m_index];
        m_messageDuration = TextManager.Instance.GetDisplayLength(info.message.Length);
        TextManager.Instance.DisplayTextWithImage(info.message, info.imageID);
        m_currentMessageTime = 0.0f;
    }

    public void SetDialog(float delay)
    {
        m_index = -1;
        m_messageDuration = delay;
        m_paused = false;
    }

    public void SetNextLevel(string level)
    {
        m_nextLevel = level;
    }
    
    public void LoadNextLevel()
    {
        Color c = new Color() { r = 0.0f, g = 0.0f, b = 0.0f, a = 1.0f };
        Fade fade = FindObjectOfType<Fade>();
        fade.StartFade(c, 2.0f, m_nextLevel);
    }

    public void Pause()
    {
        m_paused = true;
    }

    public void UnPause()
    {
        m_paused = false;
    }
}
