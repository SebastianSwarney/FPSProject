using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health_Player : Health
{
    public UnityEngine.UI.Image m_playerHealthBar;
    public TMPro.TextMeshProUGUI m_healthNum;
    public override void TakeDamage(float p_takenDamage, int p_bulletOwnerPhotonID)
    {
        if (!m_photonView.IsMine) return;
            m_photonView.RPC("RPC_TakeDamage", Photon.Pun.RpcTarget.All, p_takenDamage, p_bulletOwnerPhotonID);  
        
    }
    private void Update()
    {
        m_playerHealthBar.fillAmount = (m_currentHealth / m_maxHealth);
        m_healthNum.text = ((int)m_currentHealth).ToString() ;
    }

    public override void Died(int p_attackerID)
    {
        base.Died(p_attackerID);
        if (m_photonView.IsMine)
        {
            PhotonView.Find(p_attackerID).GetComponent<PhotonView>().RPC("RPC_PlayerKilledSomeone", RpcTarget.All, m_photonView.ViewID);
        }
    }

    public override void KillZoneCollision()
    {
        if (!m_isDead)
        {
            m_isDead = true;
            m_onDied.Invoke();
            m_photonView.RPC("RPC_PlayerKilledSomeone", RpcTarget.All, m_photonView.ViewID);
        }
    }
}
