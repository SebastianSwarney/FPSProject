using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireBehaviour_Base : ScriptableObject
{
    public virtual void FireBullet(PhotonView p_gunPhotonView, TeamLabel p_teamLabel, GameObject p_bullet, Transform p_fireSpot, float p_bulletSpeed, float p_bulletDamage, Vector2 p_bulletSpread, Transform p_target)
    {
        Vector3 dir = Quaternion.AngleAxis(Random.Range(-p_bulletSpread.x, p_bulletSpread.x),p_fireSpot.up)* Quaternion.AngleAxis(Random.Range(-p_bulletSpread.y, p_bulletSpread.y),p_fireSpot.right) * p_fireSpot.forward;
        string newBullet = SerializeBulletData(p_teamLabel.m_myTeam, p_bullet.name, p_fireSpot.position, dir, p_bulletSpeed, p_bulletDamage, p_target);
        p_gunPhotonView.RPC("RPC_FireBullet", RpcTarget.All, newBullet);
    }



    public string SerializeBulletData(TeamTypes.TeamType p_teamType, string p_bulletPrefabName, Vector3 p_fireSpot, Vector3 p_fireDir, float p_bulletSpeed, float p_bulletDamage, Transform p_target)
    {
        BulletData bulletData = new BulletData();
        bulletData.m_bulletPrefabName = p_bulletPrefabName;

        bulletData.m_bulletStartX = p_fireSpot.x;
        bulletData.m_bulletStartY = p_fireSpot.y;
        bulletData.m_bulletStartZ = p_fireSpot.z;

        bulletData.m_bulletDirX = p_fireDir.x;
        bulletData.m_bulletDirY = p_fireDir.y;
        bulletData.m_bulletDirZ = p_fireDir.z;

        bulletData.m_bulletSpeed = p_bulletSpeed;
        bulletData.m_bulletDamage = p_bulletDamage;

        if (p_target != null)
        {
            bulletData.m_targetPlayer = true;
            bulletData.m_targetPlayerPhotonID = p_target.GetComponent<PhotonView>().ViewID;
        }
        else
        {
            bulletData.m_targetPlayer = false;
        }
        bulletData.m_bulletTeam = TeamTypes.GetTeamAsInt(p_teamType);

        return JsonUtility.ToJson(bulletData);
    }


}
[SerializeField]
public class BulletData
{
    [SerializeField] public string m_bulletPrefabName;
    [SerializeField] public bool m_targetPlayer;
    [SerializeField] public int m_bulletTeam;
    [SerializeField] public int m_targetPlayerPhotonID;
    [SerializeField] public float m_bulletStartX, m_bulletStartY, m_bulletStartZ;


    [SerializeField] public float m_bulletDirX, m_bulletDirY, m_bulletDirZ;
    [SerializeField] public float m_bulletSpeed, m_bulletDamage;

}
