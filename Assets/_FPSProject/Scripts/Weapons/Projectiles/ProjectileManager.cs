using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;
    public PhotonView m_photonView;
    private void Awake()
    {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Called from the scriptable Object, creates syncronized bullets over the network
    /// </summary>
    /// <param name="p_bulletData"></param>
    [PunRPC]
    public virtual void RPC_FireBullet(string p_bulletData)
    {
        DeserializeBulletData(p_bulletData);
    }

    public void DeserializeBulletData(string p_bulletData)
    {
        BulletData newBullet = JsonUtility.FromJson<BulletData>(p_bulletData);
        GameObject newBulletObject = ObjectPooler.instance.NewObject(Resources.Load("Bullets/" + newBullet.m_bulletPrefabName) as GameObject, new Vector3(newBullet.m_bulletStartX, newBullet.m_bulletStartY, newBullet.m_bulletStartZ), Quaternion.identity);
        Transform target = null;
        if (newBullet.m_targetPlayer)
        {
            PhotonView checkPhoton = PhotonView.Find(newBullet.m_targetPlayerPhotonID);
            if (checkPhoton != null)
            {
                target = checkPhoton.transform;
            }
        }
        newBulletObject.GetComponent<Projectiles_Base>().SetVariables(TeamTypes.GetTeamFromInt(newBullet.m_bulletTeam), new Vector3(newBullet.m_bulletDirX, newBullet.m_bulletDirY, newBullet.m_bulletDirZ) * newBullet.m_bulletSpeed, newBullet.m_bulletOwnerID, target, newBullet.m_bulletDamage, newBullet.m_armorPiercing);
    }
}
