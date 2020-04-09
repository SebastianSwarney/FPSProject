using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed_Message : MonoBehaviour
{
    public /*TMPro.TextMeshProUGUI*/ UnityEngine.UI.Text m_messageText;
    private CanvasGroup m_canvasGroup;
    public float m_fadeTime;
    public AnimationCurve m_fadeCurve;
    private float m_currentTime;

    public float m_coroutineLifetime;
    private float m_currentCoroutineLife;
    private Coroutine m_fadeCoroutine;

    private void Awake()
    {
        m_messageText = GetComponentInChildren<UnityEngine.UI.Text>();//GetComponent<TMPro.TextMeshProUGUI>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ChangeMessage(string p_message)
    {
        m_messageText.text = p_message;
        m_canvasGroup.alpha = 1;
        m_currentCoroutineLife = 0;
        m_currentTime = 0;
        if(m_fadeCoroutine == null)
        {
            m_fadeCoroutine = StartCoroutine(PerformFade());
        }
    }

    private IEnumerator PerformFade()
    {
        while(m_currentCoroutineLife < m_coroutineLifetime)
        {
            while (m_currentTime < m_fadeTime)
            {
                m_currentTime += Time.deltaTime;
                m_canvasGroup.alpha = 1 - (m_fadeCurve.Evaluate(m_currentTime / m_fadeTime));
                yield return null;
            }
            m_currentCoroutineLife += Time.deltaTime;
            yield return null;
        }
        m_fadeCoroutine = null;
    }
}
