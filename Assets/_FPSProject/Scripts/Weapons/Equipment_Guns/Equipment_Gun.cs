using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Gun : Equipment_Base
{
    
    [Header("Gun Variables")]
    public Transform m_fireSpot;
    public BulletProperties m_bulletProperties;
    [System.Serializable]
    public struct BulletProperties
    {
        public GameObject m_bulletPrefab;
        public float m_bulletDamage;
        public float m_bulletSpeed;
        public float m_gunFireDelay;
    }


    private bool m_canFire = true;
    private IEnumerator m_cor_fireDelay;
    private float m_currentFireRateDelay = 0;


    [Header("Aim Assist")]
    public AimAssist m_aimAssist;
    [System.Serializable]
    public struct AimAssist
    {
        public LayerMask m_hitDetectLayer;
        public LayerMask m_playerLayer;
        public float m_minAssistDistance, m_maxAssistDistance;
    }

    [Header("Optimization")]
    public float m_coroutineLifeTime;
    private ObjectPooler m_pooler;

    [Header("Debugging")]
    public bool m_debugGizmos;
    public Color m_gizmosColor1, m_gizmosColor2;
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }

    public override void OnShootInputDown(Transform p_playerCam)
    {
        base.OnShootInputDown(p_playerCam);
        if (m_canFire)
        {
            GameObject hitPlayerObject;
            PerformAimAssist(p_playerCam, out hitPlayerObject);
            CreateBullet(hitPlayerObject);
            StartFireDelay();
        }
    }

    public override void OnShootInputUp(Transform p_playerCam)
    {
        base.OnShootInputUp(p_playerCam);
    }

    private void CreateBullet(GameObject p_hitObj)
    {
        m_pooler.NewObject(m_bulletProperties.m_bulletPrefab, m_fireSpot.position, m_fireSpot.rotation).GetComponent<Projectiles_Base>().SetVariables(m_teamLabel.m_myTeam, m_fireSpot.forward * m_bulletProperties.m_bulletSpeed, p_hitObj.transform,m_bulletProperties.m_bulletDamage);
    }

    private void PerformAimAssist(Transform p_playerCam, out GameObject p_hitPlayer)
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

    private void StartFireDelay()
    {
        m_canFire = false;
        m_currentFireRateDelay = 0;
        if (m_cor_fireDelay == null)
        {
            StartCoroutine(FireRateDelay());
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

    public Transform m_camera;
    private void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawLine(m_camera.position + (m_camera.forward * m_aimAssist.m_minAssistDistance), m_camera.position + m_camera.forward * m_aimAssist.m_maxAssistDistance);
    }
}
