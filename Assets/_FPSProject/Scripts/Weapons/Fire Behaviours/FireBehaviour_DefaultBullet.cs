using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBehaviour_Default", menuName = "Fire Behaviours/Default Fire Behaviour", order = 0)]
public class FireBehaviour_DefaultBullet : FireBehaviour_Base
{
    public override void FireBullet(TeamLabel p_teamLabel, GameObject p_bullet, Transform p_fireSpot, float p_bulletSpeed, float p_bulletDamage, Transform p_target)
    {
        GameObject newBullet = ObjectPooler.instance.NewObject(p_bullet, p_fireSpot.position, p_fireSpot.rotation);
        newBullet.GetComponent<Projectiles_Base>().SetVariables(p_teamLabel.m_myTeam, p_fireSpot.forward * p_bulletSpeed, p_target, p_bulletDamage);
        
    }
}
