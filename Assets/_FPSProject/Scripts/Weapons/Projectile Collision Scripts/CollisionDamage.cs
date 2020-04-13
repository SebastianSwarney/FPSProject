using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollisionDamage : MonoBehaviour, IProjectile_Collision
{
    private Projectiles_Base m_projectileDamage;
    private TeamLabel m_myTeamLabel;
    private void Start()
    {
        m_myTeamLabel = GetComponent<TeamLabel>();
        m_projectileDamage = GetComponent<Projectiles_Base>();
    }


    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition, int p_bulletOwnerPhotonID)
    {
        if (p_collidedObject == null) return;
        Health newHealth = p_collidedObject.GetComponent<Health>();
        if (newHealth != null)
        {
            if (newHealth.m_teamLabel.m_myTeam == m_myTeamLabel.m_myTeam) return;
            newHealth.TakeDamage(m_projectileDamage.m_projectileDamage, p_bulletOwnerPhotonID);
            PhotonView.Find(p_bulletOwnerPhotonID).GetComponent<PhotonView>().RPC("RPC_PlayerHitSomeone", RpcTarget.All,newHealth.m_photonView.ViewID);
        }
    }
}
