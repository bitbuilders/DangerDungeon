using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] Image m_image = null;
    [SerializeField] [Range(0.0f, 5.0f)] float m_time;
    [SerializeField] Color m_color;
    [SerializeField] bool m_startOnAwake = true;

    float m_timer = 0.0f;
    Color m_colorStart;
    string m_levelName = "";

    void Start()
    {
        if (m_startOnAwake)
        {
            StartFade(m_color, m_time);
        }
    }

    public void StartFade(Color color, float time, string levelName = "")
    {
        m_color = color;
        m_time = time;
        m_timer = m_time;
        m_colorStart = m_image.color;
        m_levelName = levelName;

        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        while (m_timer > 0.0f)
        {
            m_timer = m_timer - Time.deltaTime;

            float t = 1.0f - (m_timer / m_time);
            m_image.color = Color.Lerp(m_colorStart, m_color, t);

            yield return null;
        }

        if (m_levelName != "")
        {
            SceneManager.LoadScene(m_levelName);
        }

        yield return null;
    }
}
