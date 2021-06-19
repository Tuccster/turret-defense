using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VariableTextDisplay : MonoBehaviour
{
    [Header("Settings")]
    public string m_format = "value = {0}";
    public int m_updateInterval = 4;

    [Header("Resources")]
    public Int32Variable[] m_values;

    private Text m_valueDisplay;
    private int m_frameCount = 0;

    private void Awake()
    {
        m_valueDisplay = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        // Yes, this is a horrible solution.
        // Yes, this is a horrible component.
        // Yes, I know.
        
        m_frameCount++;
        if (m_frameCount % m_updateInterval != 0) return;
        object[] values = new object[m_values.Length];
        for (int i = 0; i < m_values.Length; i++)
            values[i] = m_values[i].Value;
        m_valueDisplay.text = string.Format(m_format, values);
    }
}
