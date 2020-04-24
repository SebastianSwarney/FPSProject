using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health_Player : Health
{

    public override void TakeDamage(float p_takenDamage, int p_bulletOwnerPhotonID)
    {
        if (!m_photonView.IsMine) return;
        m_photonView.RPC("RPC_TakeDamage", Photon.Pun.RpcTarget.All, p_takenDamage, p_bulletOwnerPhotonID);
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
