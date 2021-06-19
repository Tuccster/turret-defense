using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretHealthDisplay : MonoBehaviour
{
    [Header("Resources")]
    public Text m_turretHealthDisplay;
    public Int32Variable m_turretMaxHealth;
    public Int32Variable m_turretHealthVar;

    private void FixedUpdate()
    {
        m_turretHealthDisplay.text = $"{m_turretHealthVar.Value} / {m_turretMaxHealth.Value}";
    }
}
