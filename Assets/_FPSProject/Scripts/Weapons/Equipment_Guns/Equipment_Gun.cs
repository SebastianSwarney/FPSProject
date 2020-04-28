using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Equipment_Gun : Equipment_Base
{
    #region Gun Vairables
    [Header("Gun Variables")]
    public Transform m_fireSpot;
    public Vector3 m_gunOffset;
    public FireBehaviour_Base m_fireBehaviour;

    public Vector2 m_bulletSpread;

    public BulletProperties m_bulletProperties;

    [HideInInspector]
    public int m_currentClipSize;
    private int m_totalAmmoAmount;
    [HideInInspector]
    public bool m_canFire = true;
    [HideInInspector]
    public bool m_isReloading = false;
    private Coroutine m_cor_fireDelay;
    private float m_currentFireRateDelay = 0;


    public float m_reloadTime;
    private float m_currentReloadingTime;
    private float m_currentReloadCoroutineLife;
    private Coroutine m_reloadingCoroutine;

    [Header("Camera Shake")]
    public CameraProperties m_cameraPropeties;
    private bool m_isZoomed, m_isDoubleZoomed;
    [System.Serializable]
    public struct CameraProperties
    {
        [Header("Shake Properties")]
        public float m_shakeTime;
        [Range(0,0.5f)]
        public float m_kickbackAmount;
        public Vector2 m_shakeAmount;

        [Header("Zoom Properties")]
        public bool m_canZoom;
        public float m_zoomFOV;
        [Range(0f, 1f)]
        public float m_sensitivtyMultiplier;
        public bool m_canDoubleZoom;
        public float m_doubleZoomFOV;
        [Range(0f, 1f)]
        public float m_doubleSensitivtyMultiplier;


    }



    [System.Serializable]
    public struct BulletProperties
    {
        public GameObject m_bulletPrefab;

        public int m_clipSize;
        public int m_startingClipAmount;

        public float m_bulletDamage;
        public float m_bulletSpeed;
        public float m_gunFireDelay;
    }

    #endregion
    
    #region Recoil
    //Variables for the recoil system
    [Header("Recoil Variables")]
    public float m_bulletsToCompleteRecoilPattern;
    public float m_horizontalRecoilAmount;
    public AnimationCurve m_horizontalRecoilPattern;
    public AnimationCurve m_horizontalRecoilDecayPattern;

    public float m_verticalRecoilAmount;
    public AnimationCurve m_verticalRecoilPattern;
    [HideInInspector]
    public float m_amountOfBulletsShot;




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

    #region Setup Functions
    private void OnEnable()
    {
        m_canFire = true;
        m_currentFireRateDelay = 0;
        m_cor_fireDelay = null;
        m_amountOfBulletsShot = 0;
    }
    private void Start()
    {
        m_currentClipSize = m_bulletProperties.m_clipSize;   
    }
    public override void PutEquipmentAway()
    {
        base.PutEquipmentAway();
        if (m_reloadingCoroutine != null)
        {
            StopCoroutine(m_reloadingCoroutine);
            m_reloadingCoroutine = null;
        }

    }

    public override void ResetEquipment()
    {
        base.ResetEquipment();
        transform.localPosition = m_gunOffset;
        m_currentClipSize = m_bulletProperties.m_clipSize;
        m_canFire = true;
        m_canUse = true;
        StopAllCoroutines();
        m_cor_fireDelay = null;
        m_reloadingCoroutine = null;
        m_totalAmmoAmount = m_bulletProperties.m_clipSize * m_bulletProperties.m_startingClipAmount;
    }

    /// <summary>
    /// Called by the equipment controller, to properly set up the equipment when it is equiped
    /// </summary>
    /// <param name="p_currentTeam"></param>
    /// <param name="p_equipController"></param>
    /// <param name="p_currentPhotonView"></param>
    public override void SetUpEquipment(TeamTypes.TeamType p_currentTeam, EquipmentController p_equipController, PhotonView p_currentPhotonView)
    {
        base.SetUpEquipment(p_currentTeam, p_equipController, p_currentPhotonView);
        if (m_currentClipSize <= 0)
        {
            StartReloading();
        }
    }

    #endregion

    #region Input Functions
    public override void OnShootInputDown(Transform p_playerCam)
    {
        base.OnShootInputDown(p_playerCam);
        ShootInputDown(p_playerCam);
    }
    public virtual void ShootInputDown(Transform p_playerCam)
    {
        if (m_isReloading) return;
        if (m_canFire)
        {
            Transform hitPlayerObject;
            PerformAimAssist(p_playerCam, out hitPlayerObject);
            FireBullet(p_playerCam, hitPlayerObject);
            StartFireDelay();
        }
    }

    /// <summary>
    /// Called by the equipment controller to put the equipment away
    /// </summary>

    public override void OnShootInputUp(Transform p_playerCam)
    {
        base.OnShootInputUp(p_playerCam);
        ShootInputUp(p_playerCam);
    }

    public virtual void ShootInputUp(Transform p_playerCam)
    {
        m_amountOfBulletsShot = 0;
    }

    public override void OnReloadDown()
    {
        ReloadDown();
    }

    public virtual void ReloadDown()
    {
        StartReloading();
        
    }

    #endregion

    public void FireBullet(Transform p_playerCam, Transform p_targetObject)
    {

        m_fireBehaviour.FireBullet(m_ownerID, m_teamLabel, m_bulletProperties.m_bulletPrefab, m_fireSpot, m_bulletProperties.m_bulletSpeed, m_bulletProperties.m_bulletDamage, m_bulletSpread, p_targetObject);
        
        m_amountOfBulletsShot++;
        ApplyRecoil(Mathf.Clamp(m_amountOfBulletsShot / m_bulletsToCompleteRecoilPattern, 0, 1));
        m_equipController.ShakeCamera(m_cameraPropeties.m_shakeTime, m_cameraPropeties.m_kickbackAmount, m_cameraPropeties.m_shakeAmount);
        m_currentClipSize--;
        if (m_currentClipSize == 0)
        {
            StopShooting();
            StartReloading();
        }

    }

    /// <summary>
    /// Used to stop any coroutines if the clip size becomes zero in the middle of the coroutine
    /// </summary>
    public virtual void StopShooting()
    {
    }

    public void StartReloading()
    {
        m_amountOfBulletsShot = 0;

        m_isReloading = true;
        m_currentReloadingTime = 0;
        m_currentReloadCoroutineLife = 0;
        if (m_reloadingCoroutine == null)
        {
            m_reloadingCoroutine = StartCoroutine(ReloadingCoroutine());
        }

    }

    private IEnumerator ReloadingCoroutine()
    {
        bool reloaded = false;
        while (m_currentReloadCoroutineLife < m_coroutineLifeTime)
        {

            while (m_currentReloadingTime < m_reloadTime)
            {
                reloaded = false;
                m_currentReloadingTime += Time.deltaTime;
                yield return null;
            }
            if (!reloaded)
            {
                reloaded = true;
                m_isReloading = false;
                m_currentClipSize = m_bulletProperties.m_clipSize;
            }
            m_currentReloadCoroutineLife += Time.deltaTime;
            yield return null;
        }

        m_reloadingCoroutine = null;
    }

    public virtual void ApplyRecoil(float p_patternProgress)
    {
        float yPatternProgress = m_horizontalRecoilPattern.Evaluate(p_patternProgress);
        float currentYRecoil = Mathf.Lerp(m_horizontalRecoilAmount, -m_horizontalRecoilAmount, yPatternProgress);

        float currentYDecayProgress = m_horizontalRecoilDecayPattern.Evaluate(p_patternProgress);
        float currentYDecay = Mathf.Lerp(1, 0, currentYDecayProgress);

        float xPatternProgress = m_verticalRecoilPattern.Evaluate(p_patternProgress);
        float currentXRecoil = Mathf.Lerp(m_verticalRecoilAmount, 0, xPatternProgress);

        m_equipController.ApplyRecoilCameraRotation(currentXRecoil, currentYRecoil * currentYDecay, m_bulletProperties.m_gunFireDelay);
    }

    public void PerformAimAssist(Transform p_playerCam, out Transform p_hitPlayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_hitDetectLayer))
        {
            m_fireSpot.LookAt(hit.point);

            if (Physics.SphereCast(p_playerCam.position + (p_playerCam.forward * m_aimAssist.m_minAssistDistance), m_aimAssist.m_aimAssistRadius, p_playerCam.forward, out hit, m_aimAssist.m_maxAssistDistance, m_aimAssist.m_playerLayer))
            {
                p_hitPlayer = hit.transform;
                m_fireSpot.LookAt(hit.point);
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


    public Vector3Int GetAmmoAmount()
    {
        return new Vector3Int(m_totalAmmoAmount, m_currentClipSize, m_bulletProperties.m_clipSize);
    }


    public void ToggleZoom()
    {
        if (!m_cameraPropeties.m_canZoom) return;
        if (!m_isZoomed)
        {
            m_isZoomed = true;
            m_equipController.ZoomCamera(true, m_cameraPropeties.m_zoomFOV, m_cameraPropeties.m_sensitivtyMultiplier);
        }
        else
        {
            m_isZoomed = false;
            m_isDoubleZoomed = false;
            m_equipController.ZoomCamera(false);
        }
        
    }

    public void ToggleDoubleZoom()
    {
        if (!m_isZoomed) return;
        if (!m_cameraPropeties.m_canZoom || !m_cameraPropeties.m_canDoubleZoom) return;
        if (!m_isDoubleZoomed)
        {
            m_isDoubleZoomed = true;
            m_equipController.ZoomCamera(true, m_cameraPropeties.m_doubleZoomFOV, m_cameraPropeties.m_doubleSensitivtyMultiplier);
        }
        else
        {
            m_isDoubleZoomed = false;
            m_equipController.ZoomCamera(true, m_cameraPropeties.m_zoomFOV, m_cameraPropeties.m_sensitivtyMultiplier);
        }
            
        
    }



    private void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * m_aimAssist.m_minAssistDistance), m_aimAssist.m_aimAssistRadius);
        Gizmos.DrawLine(transform.position + (transform.forward * m_aimAssist.m_minAssistDistance), transform.position + transform.forward * m_aimAssist.m_maxAssistDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_aimAssist.m_maxAssistDistance, m_aimAssist.m_aimAssistRadius);


        Gizmos.color = m_gizmosColor2;
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position + Quaternion.AngleAxis(m_bulletSpread.x, m_fireSpot.up) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position + Quaternion.AngleAxis(-m_bulletSpread.x, m_fireSpot.up) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position + Quaternion.AngleAxis(m_bulletSpread.y, m_fireSpot.right) * (m_fireSpot.forward * 5));
        Gizmos.DrawLine(m_fireSpot.position, m_fireSpot.position + Quaternion.AngleAxis(-m_bulletSpread.y, m_fireSpot.right) * (m_fireSpot.forward * 5));
    }
}
