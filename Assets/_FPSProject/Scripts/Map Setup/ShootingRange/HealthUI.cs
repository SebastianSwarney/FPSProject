using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public UnityEngine.UI.Image m_healthBar;
    public TMPro.TextMeshProUGUI m_healthText;
    public Health m_health;

    private void Update()
    {
        m_healthBar.fillAmount = (m_health.GetHealthValues().x / m_health.GetHealthValues().y);
        m_healthText.text = m_health.GetHealthValues().x.ToString();
    }
}
