using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera m_camera;
    private float m_shakeTime, m_currentShakeTime;
    private Vector2 m_shakeAmount;
    private float m_kickbackAmount;

    private bool m_isShaking;
    public Photon.Pun.PhotonView m_photonView;

    private float m_defulatFOV;
    public float m_zoomTime;

    private float m_currentTargetFOV, m_startingLerpFOV;
    private float m_zoomLerpTimer;
    private bool m_isZoomLerping;

    private void Start()
    {
        if (!m_photonView.IsMine)
        {
            enabled = false;
        }
        m_defulatFOV = m_camera.fieldOfView;

    }


    private void Update()
    {
        PerformShake();
        PerformZoom();
    }
    private void PerformShake()
    {
        if (!m_isShaking) return;
        if (m_currentShakeTime > m_shakeTime)
        {
            m_camera.transform.localPosition = Vector3.zero;
            m_isShaking = false;
        }
        else
        {
            m_currentShakeTime += Time.deltaTime;
            SetPosition();
        }

    }
    public void StartShakeCamera(float p_shakeTime, float p_kickbackAmount, Vector2 p_shakeAmount)
    {
        if (m_isShaking)
        {
            //If the current requested shake is less powerful then the current shake being performed, do not apply the shake
            if (p_shakeAmount.magnitude < Vector2.Lerp(m_shakeAmount, Vector2.zero, m_currentShakeTime / m_shakeTime).magnitude)
            {
                return;
            }
        }
        m_isShaking = true;
        m_currentShakeTime = 0;
        m_shakeTime = p_shakeTime;
        m_kickbackAmount = p_kickbackAmount;
        m_shakeAmount = p_shakeAmount;
    }
    private void SetPosition()
    {
        float percent = m_currentShakeTime / m_shakeTime;
        Vector2 max2d = new Vector2(Mathf.Lerp(m_shakeAmount.x, 0, percent), Mathf.Lerp(m_shakeAmount.y, 0, percent));

        m_camera.transform.localPosition = new Vector3(Random.Range(-max2d.x, max2d.x), Random.Range(-max2d.y, max2d.y), Mathf.Lerp(-m_kickbackAmount, 0, percent));
    }

    private void PerformZoom()
    {
        if (!m_isZoomLerping) return;
        if (m_zoomLerpTimer > m_zoomTime)
        {
            m_camera.fieldOfView = m_currentTargetFOV;
            m_isZoomLerping = false;
        }
        else
        {
            m_zoomLerpTimer += Time.deltaTime;
            m_camera.fieldOfView = Mathf.Lerp(m_startingLerpFOV, m_currentTargetFOV, m_zoomLerpTimer / m_zoomTime);
        }
    }
    public void ChangeFOV(float p_fov, float p_sensitivityMultiplier)
    {
        m_startingLerpFOV = m_camera.fieldOfView;
        m_currentTargetFOV = p_fov;
        PlayerInput.Instance.ChangeSensitivity(p_sensitivityMultiplier);
        m_zoomLerpTimer = 0;
        m_isZoomLerping = true;
    }
    public void ResetFOV()
    {
        m_currentTargetFOV = m_defulatFOV;
        m_startingLerpFOV = m_camera.fieldOfView;
        m_zoomLerpTimer = 0;
        PlayerInput.Instance.ChangeSensitivity(1);
        m_isZoomLerping = true;
    }
}
