using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
    public UnityEngine.UI.Image m_playerHealthBar;
    public override void TakeDamage(float p_takenDamage)
    {
        if (!m_photonView.IsMine)
        {
            m_photonView.RPC("RPC_TakeDamage", Photon.Pun.RpcTarget.AllBuffered, p_takenDamage);
            m_playerHealthBar.fillAmount = m_currentHealth / m_maxHealth;
        }
    }
}
