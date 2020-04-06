using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "FireBehaviour_Scatter", menuName = "Fire Behaviours/Scatter", order = 0)]
public class FireBehaviour_Scatter : FireBehaviour_Base
{
    public float m_bulletSpreadAngle;
    public int m_bulletAmount;
    public override void FireBullet(PhotonView p_firedPhotonView, TeamLabel p_teamLabel, GameObject p_bullet, Transform p_fireSpot, float p_bulletSpeed, float p_bulletDamage, Transform p_target)
    {
        float damagePerBullet = p_bulletDamage / m_bulletAmount;
        for (int i = 0; i < m_bulletAmount; i++)
        {
            Vector3 newDir = Quaternion.AngleAxis(Random.Range(-m_bulletSpreadAngle, m_bulletSpreadAngle), p_fireSpot.up) * Quaternion.AngleAxis(Random.Range(-m_bulletSpreadAngle, m_bulletSpreadAngle),p_fireSpot.right) *p_fireSpot.forward;
            string newBullet = SerializeBulletData(p_teamLabel.m_myTeam, p_bullet.name, p_fireSpot.position, newDir, p_bulletSpeed, damagePerBullet, p_target);
            p_firedPhotonView.RPC("RPC_FireBullet", RpcTarget.All, newBullet);
        }
    }


}
