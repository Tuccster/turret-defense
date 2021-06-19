using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [Header("Resources")]
    public Text m_currencyDisplay;

    public void UpdateCurrencyDisplay(int amount)
    {
        m_currencyDisplay.text = $"Ꝓ{amount}";
    }
}
