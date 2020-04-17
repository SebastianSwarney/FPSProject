using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameMode_CaptureTheFlag : MonoBehaviour
{
    public static GameMode_CaptureTheFlag Instance;

    public List<UITest> m_score;
    private PhotonView m_photonView;
    [System.Serializable]
    public class UITest
    {
        public int m_score;
        public TMPro.TextMeshProUGUI m_scoreText;
    }

    private void Awake()
    {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    public void AddPoints(int p_teamNum)
    {
        m_score[p_teamNum - 1].m_score++;
        m_photonView.RPC("RPC_DisplayScore", RpcTarget.All, p_teamNum, m_score[p_teamNum-1].m_score);

    }

    [PunRPC]
    private void RPC_DisplayScore(int p_Team, int p_score)
    {
        m_score[p_Team - 1].m_score = p_score;
        m_score[p_Team - 1].m_scoreText.text = p_score.ToString();
    }



}
