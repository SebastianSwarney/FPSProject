using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RespawnEvent : UnityEngine.Events.UnityEvent { }
public class RespawnObject : MonoBehaviour
{
    public float m_respawnTime;
    float m_respawnTimer;
    private Vector3 m_respawnPosition;
    private Quaternion m_respawnRotation;
    public RespawnEvent m_respawnedEvent;


    public virtual void Start()
    {
        m_respawnPosition = transform.position;
        m_respawnRotation = transform.rotation;

    }

    public IEnumerator RespawnCoroutine()
    {
        m_respawnTimer = 0;
        while (m_respawnTimer < m_respawnTime)
        {
            m_respawnTimer += Time.deltaTime;
            yield return null;
        }

        RespawnMe();
    }

    public void ObjectDied()
    {
        StartCoroutine(RespawnCoroutine());
    }

    public virtual void RespawnMe()
    {
        transform.position = m_respawnPosition;
        transform.rotation = m_respawnRotation;
        m_respawnedEvent.Invoke();
    }
}
