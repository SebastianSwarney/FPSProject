using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeedManager : MonoBehaviour
{
    public static KillFeedManager Instance;

    public GameObject m_messagePrefab;
    public Transform m_killFeedParent;
    private Queue<Transform> m_messageQueue = new Queue<Transform>();
    public int m_numberOfMessages = 4;
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < 4; i++)
        {
            m_messageQueue.Enqueue(Instantiate(m_messagePrefab, m_killFeedParent).transform);
        }

    }

    public void AddMessage(string p_message)
    {
        Transform newMessage = m_messageQueue.Dequeue();
        newMessage.GetComponent<KillFeed_Message>().ChangeMessage(p_message);
        newMessage.SetAsFirstSibling();
        m_messageQueue.Enqueue(newMessage);
    }
}
