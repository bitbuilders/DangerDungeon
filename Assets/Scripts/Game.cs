using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Transform m_startPosition;
    [SerializeField] GameObject[] m_enemies;
    [SerializeField] GameObject m_dog;
    //[SerializeField] GameObject m_pauseMenu;
    
    void Start()
    {


        Player player = FindObjectOfType<Player>();
        //player.m_bulletSprites.Clear();

        //HUD hud = FindObjectOfType<HUD>();
        //RectTransform[] children = hud.GetComponentsInChildren<RectTransform>();
        //foreach (RectTransform t in children)
        //{
        //    if (t.name == "Bullets")
        //    {
        //        Image[] children2 = t.GetComponentsInChildren<Image>();
        //        foreach (Image i in children2)
        //        {
        //            player.m_bulletSprites.Add(i);

        //        }
        //        break;
        //    }
        //}

        player.m_levelStartPoint = m_startPosition;
        player.Respawn();
        player.gameObject.SetActive(true);
        
        foreach (GameObject enemy in m_enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().m_foe = player.gameObject;
            }
        }

        GameObject dog = null;
        if (UpgradeManager.Instance.HasUpgrade(UpgradeManager.PotentialUpgrades.COMPANION))
        {
            dog = Instantiate(m_dog, m_startPosition.position, Quaternion.identity);
            dog.GetComponent<Dog>().m_owner = player.gameObject;
        }

        CameraMove camera = FindObjectOfType<CameraMove>();
        camera.gameObject.SetActive(true);
        if (dog)
        {
            camera.SetNewPointOfInterest(dog, 1.0f);
        }
        else
        {
            camera.SetNewPointOfInterest(player.gameObject, 0.0f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelManager.Instance.Quit();
        }
    }
}
