using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillSetter : MonoBehaviour
{
    [Header("Resources")]
    public Image m_fillImage;
    public Int32Variable m_max;
    public Int32Variable m_data;

    private void FixedUpdate()
    {
        m_fillImage.fillAmount = (float)m_data.Value / (float)m_max.Value;
    }
}
