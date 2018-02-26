using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    public enum PotentialUpgrades
    {
        COMPANION,
        GUN,
        HEALTH,
        DAGGER_RANGE
    }

    [System.Serializable]
    public struct UpgradeData
    {
        public Sprite m_icon;
        public string m_name;
        public string m_info;
        public PotentialUpgrades m_upgradeName;
    }

    [SerializeField] List<UpgradeData> m_upgradePool = new List<UpgradeData>();
    [SerializeField] List<UpgradeData> m_upgradesForNextChoice = new List<UpgradeData>();
    [SerializeField] List<UpgradeData> m_playerUpgrades = new List<UpgradeData>();

    static UpgradeManager ms_manager = null;

    private void Start()
    {
        if (ms_manager == null)
        {
            ms_manager = this;
        }
        else
        {
            Destroy(ms_manager);
            ms_manager = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public UpgradeData GetUpgrade()
    {
        int x = Random.Range(0, m_upgradePool.Count);
        
        UpgradeData data = m_upgradePool[x];
        m_upgradePool.Remove(data);
        m_upgradesForNextChoice.Add(data);

        return data;
    }

    public void SelectUpgrade(UpgradeData data)
    {
        m_playerUpgrades.Add(data);
        m_upgradesForNextChoice.Remove(data);
        m_upgradePool.AddRange(m_upgradesForNextChoice);
        m_upgradesForNextChoice.Clear();

        FindObjectOfType<Player>().Activate(true);
        LevelManager.Instance.LoadLevel(FindObjectOfType<NextLevel>().m_nextLevel);
    }

    public bool HasUpgrade(PotentialUpgrades upgrade)
    {
        bool has = false;

        if (m_upgradePool != null)
        {
            foreach (UpgradeData u in m_playerUpgrades)
            {
                if (u.m_upgradeName.Equals(upgrade))
                {
                    has = true;
                    break;
                }
            }
        }

        return has;
    }
}
