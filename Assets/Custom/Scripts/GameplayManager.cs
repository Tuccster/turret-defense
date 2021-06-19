using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Resources")]
    public IntEventSO m_onCurrencyChanged;

    public int m_currency = 0;

    // Called from EnemyEventSO
    public void OnEnemyKilled(Enemy enemy)
    {
        m_currency += 1;
        m_onCurrencyChanged.RaiseEvent(m_currency);
    }
}
