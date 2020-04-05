using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform m_followTransform;
    public float m_lifespanAfterTransform;
    private ObjectPooler m_pooler;
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }
    private void Update()
    {
        if (m_followTransform != null)
        {
            transform.position = m_followTransform.position;
            if(!m_followTransform.gameObject.activeSelf)
            {
                m_followTransform = null;
                StartCoroutine(StartDecay());
            }
        }
    }


    public void SetFollower(Transform p_follower)
    {
        m_followTransform = p_follower;
    }
    private IEnumerator StartDecay()
    {
        yield return new WaitForSeconds(m_lifespanAfterTransform);
        m_pooler.ReturnToPool(this.gameObject);
    }
}
