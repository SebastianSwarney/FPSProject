using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
    public UnityEngine.UI.Image m_playerHealthBar;
    public TMPro.TextMeshProUGUI m_healthNum;
    public override void TakeDamage(float p_takenDamage, int p_bulletOwnerPhotonID)
    {
        if (!m_photonView.IsMine)
        {
            m_photonView.RPC("RPC_TakeDamage", Photon.Pun.RpcTarget.AllBuffered, p_takenDamage, p_bulletOwnerPhotonID);  
        }
    }
    private void Update()
    {
        m_playerHealthBar.fillAmount = (m_currentHealth / m_maxHealth);
        m_healthNum.text = ((int)m_currentHealth).ToString() ;
    }
}
