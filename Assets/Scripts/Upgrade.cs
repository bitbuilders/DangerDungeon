using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    //[SerializeField] Sprite m_icon;
    //[SerializeField] string m_name;
    //[SerializeField] string m_info;
    [SerializeField] Image m_iconLocation;
    [SerializeField] TextMeshProUGUI m_nameLocation;
    [SerializeField] TextMeshProUGUI m_infoLocation;
    public UpgradeManager.PotentialUpgrades m_upgradeName;

    UpgradeManager.UpgradeData m_upgradeData;

    private void Start()
    {
        m_upgradeData = UpgradeManager.Instance.GetUpgrade();
        m_iconLocation.sprite = m_upgradeData.m_icon;
        m_nameLocation.text = "<color=yellow>" + m_upgradeData.m_name + "</color>";
        m_infoLocation.text = m_upgradeData.m_info;
    }

    public void OnClick()
    {
        UpgradeManager.Instance.SelectUpgrade(m_upgradeData);
    }
}
