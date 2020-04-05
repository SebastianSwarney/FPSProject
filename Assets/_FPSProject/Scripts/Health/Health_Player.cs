using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
    public override void TakeDamage(float p_takenDamage)
    {
        if (!m_photonView.IsMine)
        {
            m_photonView.RPC("RPC_TakeDamage", Photon.Pun.RpcTarget.AllBuffered, p_takenDamage);
        }
    }
}
