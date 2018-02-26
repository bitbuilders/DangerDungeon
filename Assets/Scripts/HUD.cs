using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenu = null;
    [SerializeField] GameObject m_resume = null;
    [SerializeField] GameObject m_title = null;

    static HUD ms_hud = null;

    void Start()
    {
        if (ms_hud == null)
        {
            ms_hud = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (ms_hud != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape) && !m_pauseMenu.activeInHierarchy)
        //{
        //    Time.timeScale = 0.0f;
        //    m_pauseMenu.SetActive(true);
        //    m_resume.SetActive(true);
        //    m_title.SetActive(true);
        //}
    }
}
