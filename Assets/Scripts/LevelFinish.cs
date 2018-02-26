using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] string m_nextLevel = "";
    [SerializeField] GameObject[] m_enemies = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && AllEnemiesDefeated())
        {
            Color c = new Color() { r = 0.0f, g = 0.0f, b = 0.0f, a = 1.0f };
            Fade fade = FindObjectOfType<Fade>();
            fade.StartFade(c, 2.0f, m_nextLevel);
            collision.gameObject.GetComponent<Player>().Activate(false);
        }
    }

    private bool AllEnemiesDefeated()
    {
        bool defeated = true;

        foreach (GameObject enemy in m_enemies)
        {
            if (enemy != null)
            {
                defeated = false;
                break;
            }
        }

        return defeated;
    }
}
