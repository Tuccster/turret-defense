using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStats : MonoBehaviour
{
    [Header("Settings")]
    public Stat m_turnSpeed;
    public Stat m_fireDelay;
    public Stat m_shotType;

    [Header("Resources")]
    public Int32Variable m_currencyAmount;

    private void Awake()
    {
        ResetStatValues();
    }

    public void ResetStatValues()
    {
        // "Wait why does this value keep changing to 100 when I start the game!?"
        // TODO : Make this not this; yuck.
        if (m_turnSpeed.m_level != null) m_turnSpeed.m_level.Value = 1;
        if (m_fireDelay.m_level != null) m_fireDelay.m_level.Value = 1;
        if (m_shotType.m_level != null) m_shotType.m_level.Value = 1;
        if (m_turnSpeed.m_upgradeCost != null) m_turnSpeed.m_upgradeCost.Value = 100;
        if (m_fireDelay.m_upgradeCost != null) m_fireDelay.m_upgradeCost.Value = 100;
        if (m_shotType.m_upgradeCost != null) m_shotType.m_upgradeCost.Value = 100;
    }

    [System.SerializableAttribute]
    public class Stat
    {
        public Int32Variable m_level;
        public Int32Variable m_maxLevel;
        public Int32Variable m_upgradeCost;
        public float m_upgradeCostMultiplier = 0.15f;
    }

    public bool UpgradeAffordable(Stat stat)
    {
        return m_currencyAmount.Value >= stat.m_upgradeCost.Value;
    }

    public bool UpgradeAvailable(Stat stat)
    {
        return stat.m_level.Value < stat.m_maxLevel.Value;
    }

    public void Upgrade(Stat stat)
    {
        if (!UpgradeAvailable(stat) || !UpgradeAffordable(stat)) return;
        m_currencyAmount.Value -= stat.m_upgradeCost.Value;
        stat.m_level.Value++;
        stat.m_upgradeCost.Value += (int)(stat.m_upgradeCost.Value * stat.m_upgradeCostMultiplier);
    }

    public int[] GetUpgradeProgression(Stat stat)
    {
        int[] upgradesCosts = new int[stat.m_maxLevel.Value];
        for (int i = 1; i < stat.m_maxLevel.Value; i++)
            upgradesCosts[i] = (int)(stat.m_upgradeCost.Value * Mathf.Pow(1 + stat.m_upgradeCostMultiplier, i));
        return upgradesCosts;
    }

    // Methods used by UI to directcly upgrade stats in TurretStats SO
    public void UpgradeFireDelay() { Upgrade(m_fireDelay); }
    public void UpgradeTurnSpeed() { Upgrade(m_turnSpeed); }
    public void UpgradeShotType()  { Upgrade(m_shotType);  }
}
