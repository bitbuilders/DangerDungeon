using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenu = null;
    [SerializeField] GameObject m_resume = null;
    [SerializeField] GameObject m_title = null;

    public void UnPauseGame()
    {
        Time.timeScale = 1.0f;
        m_pauseMenu.SetActive(false);
        m_resume.SetActive(false);
        m_title.SetActive(false);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1.0f;
        m_pauseMenu.SetActive(false);
        m_resume.SetActive(false);
        m_title.SetActive(false);
        LevelManager.Instance.LoadLevel("Menu");
    }
}
