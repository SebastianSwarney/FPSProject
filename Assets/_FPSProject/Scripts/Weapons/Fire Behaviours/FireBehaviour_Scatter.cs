using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBehaviour_Scatter", menuName = "Fire Behaviours/Scatter", order = 0)]
public class FireBehaviour_Scatter : FireBehaviour_Base
{
    public float m_bulletSpreadAngle;
    public int m_bulletAmount;
    public override void FireBullet(TeamLabel p_teamLabel, GameObject p_bullet, Transform p_fireSpot, float p_bulletSpeed, float p_bulletDamage, Transform p_target)
    {
        float damagePerBullet = p_bulletDamage / m_bulletAmount;
        for (int i = 0; i < m_bulletAmount; i++)
        {
            GameObject newBullet = ObjectPooler.instance.NewObject(p_bullet, p_fireSpot.position, p_fireSpot.rotation);
            Vector3 newDir = Quaternion.AngleAxis(Random.Range(-m_bulletSpreadAngle, m_bulletSpreadAngle), p_fireSpot.up) * Quaternion.AngleAxis(Random.Range(-m_bulletSpreadAngle, m_bulletSpreadAngle),p_fireSpot.right) *p_fireSpot.forward;
            newBullet.GetComponent<Projectiles_Base>().SetVariables(p_teamLabel.m_myTeam, newDir * p_bulletSpeed, p_target, damagePerBullet);
            Debug.DrawLine(newBullet.transform.position, newBullet.transform.position + newDir * 10, Color.magenta, 2.5f);
        }
    }
}
