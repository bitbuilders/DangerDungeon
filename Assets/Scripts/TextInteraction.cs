using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInteraction : MonoBehaviour
{
    [SerializeField] AudioClip m_hover = null;
    [SerializeField] AudioClip m_click = null;

    AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void Enter()
    {
        Color c = GetComponent<TextMeshProUGUI>().color;
        c.a = 0.75f;
        GetComponent<TextMeshProUGUI>().color = c;

        m_audioSource.clip = m_hover;
        m_audioSource.Play();
    }

    public void Exit()
    {
        Color c = GetComponent<TextMeshProUGUI>().color;
        c.a = 1.0f;
        GetComponent<TextMeshProUGUI>().color = c;
    }

    public void Click()
    {
        Color c = GetComponent<TextMeshProUGUI>().color;
        c.a = 0.5f;
        GetComponent<TextMeshProUGUI>().color = c;

        m_audioSource.clip = m_click;
        m_audioSource.Play();
    }
}
