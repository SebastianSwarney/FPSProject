﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Equipment_Gun : Equipment_Base
{
    #region Gun Vairables
    [Header("Gun Variables")]
    public Transform m_fireSpot;
    public FireBehaviour_Base m_fireBehaviour;

    public float m_recoilAmountY;

    public Vector2 m_bulletSpread;

    public BulletProperties m_bulletProperties;

    [System.Serializable]
    public struct BulletProperties
    {
        public GameObject m_bulletPrefab;

        public float m_bulletDamage;
        public float m_bulletSpeed;
        public float m_gunFireDelay;
    }

    [HideInInspector]
    public bool m_canFire = true;
    private Coroutine m_cor_fireDelay;
    private float m_currentFireRateDelay = 0;
    [HideInInspector]
    public PhotonView m_myPhotonView;
    #endregion

    #region Aim Assist
    [Header("Aim Assist")]
    public AimAssist m_aimAssist;
    [System.Serializable]
    public struct AimAssist
    {
        public LayerMask m_hitDetectLayer;
        public LayerMask m_playerLayer;
        public float m_aimAssistRadius;
        public float m_minAssistDistance, m_maxAssistDistance;
    }
    #endregion


    #region Optimization
    [Header("Optimization")]
    public float m_coroutineLifeTime;
    #endregion

    #region Debugging
    [Header("Debugging")]
    public bool m_debugGizmos;
    public Color m_gizmosColor1, m_gizmosColor2;
    #endregion

    private void Start()
    {
        m_myPhotonView = GetComponent<PhotonView>();
    }
    public override void OnShootInputDown(Transform p_playerCam)
    {
        base.OnShootInputDown(p_playerCam);
        ShootInputDown(p_playerCam);
    }
    public virtual void ShootInputDown(Transform p_playerCam)
    {
        if (m_canFire)
        {
            Transform hitPlayerObject;
            PerformAimAssist(p_playerCam, out hitPlayerObject);
            FireBullet(p_playerCam, hitPlayerObject);
            StartFireDelay();
        }
    }
    public void FireBullet(Transform p_playerCam, Transform p_targetObject)
    {
        
        m_fireBehaviour.FireBullet(m_myPhotonView, m_teamLabel, m_bulletProperties.m_bulletPrefab, m_fireSpot, m_bulletProperties.m_bulletSpeed, m_bulletProperties.m_bulletDamage, m_bulletSpread, p_targetObject);
        ApplyRecoil(m_recoilAmountY);
    }

    public virtual void ApplyRecoil(float p_yAmount)
    {
        m_equipController.ApplyRecoilCameraRotation(-p_yAmount);
    }

    public override void OnShootInputUp(Transform p_playerCam)
    {
        base.OnShootInputUp(p_playerCam);
        ShootInputUp(p_playerCam);
    }

    public virtual void ShootInputUp(Transform p_playerCam)
    {
    }


    public void PerformAimAssist(Transform p_playerCam, out Transform p_hitPlayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_hitDetectLayer))
        {
            m_fireSpot.LookAt(hit.point);
            Debug.DrawLine(transform.position, hit.point, Color.blue,1f);

            if (Physics.SphereCast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), m_aimAssist.m_aimAssistRadius, p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_playerLayer))
            {
                p_hitPlayer = hit.transform;
                m_fireSpot.LookAt(hit.point);
                Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                return;
            }
            p_hitPlayer = null;
            return;
        }
        else
        {
            if (Physics.SphereCast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), m_aimAssist.m_aimAssistRadius, p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_playerLayer))
            {
                p_hitPlayer = hit.transform;
                m_fireSpot.LookAt(hit.point);
                Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                return;
            }
            else
            {
                m_fireSpot.localRotation = Quaternion.identity;
                p_hitPlayer = null;
                return;
            }
        }


    }

    public void StartFireDelay()
    {
        m_currentFireRateDelay = 0;
        m_canFire = false;
        if (m_cor_fireDelay == null)
        {

            m_cor_fireDelay = StartCoroutine(FireRateDelay());
        }
    }

    private IEnumerator FireRateDelay()
    {
        float corLifetime = 0;
        while (corLifetime < m_coroutineLifeTime)
        {
            while (m_currentFireRateDelay < m_bulletProperties.m_gunFireDelay)
            {
                corLifetime = 0;

                m_currentFireRateDelay += Time.deltaTime;
                yield return null;
            }
            corLifetime += Time.deltaTime;
            m_canFire = true;
            yield return null;
        }
        
        m_cor_fireDelay = null;
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
        newBulletObject.GetComponent<Projectiles_Base>().SetVariables(TeamTypes.GetTeamFromInt(newBullet.m_bulletTeam), new Vector3(newBullet.m_bulletDirX, newBullet.m_bulletDirY, newBullet.m_bulletDirZ) * newBullet.m_bulletSpeed, target, newBullet.m_bulletDamage);
    }



    private void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * m_aimAssist.m_minAssistDistance), m_aimAssist.m_aimAssistRadius);
        Gizmos.DrawLine(transform.position + (transform.forward * m_aimAssist.m_minAssistDistance), transform.position + transform.forward * m_aimAssist.m_maxAssistDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_aimAssist.m_maxAssistDistance, m_aimAssist.m_aimAssistRadius);


        Gizmos.color = m_gizmosColor2;
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position+ Quaternion.AngleAxis(m_bulletSpread.x, m_fireSpot.up) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position+ Quaternion.AngleAxis(-m_bulletSpread.x, m_fireSpot.up) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position+ Quaternion.AngleAxis(m_bulletSpread.y, m_fireSpot.right) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position+ Quaternion.AngleAxis(-m_bulletSpread.y, m_fireSpot.right) * (m_fireSpot.forward * 5));
    }
}
