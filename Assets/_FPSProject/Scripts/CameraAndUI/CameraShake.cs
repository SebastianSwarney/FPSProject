using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float m_shakeTime, m_currentShakeTime;
    private Vector2 m_shakeAmount;
    private float m_kickbackAmount;

    private bool m_isShaking;
    public Photon.Pun.PhotonView m_photonView;

    private void Start()
    {
        if (!m_photonView.IsMine)
        {
            enabled = false;
        }
    }


    private void Update()
    {
        if (!m_isShaking) return;
        if (m_currentShakeTime > m_shakeTime)
        {
            transform.localPosition = Vector3.zero;
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

        transform.localPosition =  new Vector3(Random.Range(-max2d.x, max2d.x), Random.Range(-max2d.y, max2d.y), Mathf.Lerp(-m_kickbackAmount,0,percent));
    }
}
