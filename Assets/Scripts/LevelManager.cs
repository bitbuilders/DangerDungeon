using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    static LevelManager ms_manager;

    private void Start()
    {
        if (ms_manager == null)
        {
            ms_manager = this;
        }
        else
        {
            Destroy(ms_manager.gameObject);
            ms_manager = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string level)
    {
        Color c = new Color() { r = 0.0f, g = 0.0f, b = 0.0f, a = 1.0f };
        Fade fade = FindObjectOfType<Fade>();
        fade.StartFade(c, 2.0f, level);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
