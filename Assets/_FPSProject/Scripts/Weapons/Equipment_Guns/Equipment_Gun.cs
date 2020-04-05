using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Gun : Equipment_Base
{
    #region Gun Vairables
    [Header("Gun Variables")]
    public Transform m_fireSpot;
    public FireBehaviour_Base m_fireBehaviour;
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
    #endregion

    #region Aim Assist
    [Header("Aim Assist")]
    public AimAssist m_aimAssist;
    [System.Serializable]
    public struct AimAssist
    {
        public LayerMask m_hitDetectLayer;
        public LayerMask m_playerLayer;
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


    public override void OnShootInputDown(Transform p_playerCam)
    {
        base.OnShootInputDown(p_playerCam);
        ShootInputDown(p_playerCam);
    }
    public virtual void ShootInputDown(Transform p_playerCam)
    {
        if (m_canFire)
        {
            GameObject hitPlayerObject;
            PerformAimAssist(p_playerCam, out hitPlayerObject);
            FireBullet(p_playerCam);
            StartFireDelay();
        }
    }
    public void FireBullet(Transform p_playerCam)
    {
        m_fireBehaviour.FireBullet(m_teamLabel, m_bulletProperties.m_bulletPrefab, m_fireSpot, m_bulletProperties.m_bulletSpeed, m_bulletProperties.m_bulletDamage, p_playerCam);
    }

    public override void OnShootInputUp(Transform p_playerCam)
    {
        base.OnShootInputUp(p_playerCam);
        ShootInputUp(p_playerCam);
    }

    public virtual void ShootInputUp(Transform p_playerCam)
    {
    }

    public void PerformAimAssist(Transform p_playerCam, out GameObject p_hitPlayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_hitDetectLayer))
        {
            m_fireSpot.LookAt(hit.point);
            p_hitPlayer = null;
            if (Physics.Raycast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_playerLayer))
            {
                p_hitPlayer = hit.transform.gameObject;
            }
            return;
        }
        m_fireSpot.localRotation = Quaternion.identity;
        p_hitPlayer = null;
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


    private void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawLine(transform.position + (transform.forward * m_aimAssist.m_minAssistDistance), transform.position + transform.forward * m_aimAssist.m_maxAssistDistance);
    }
}
