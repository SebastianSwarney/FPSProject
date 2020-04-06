using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    public GameObject m_myAvatar;
    private PhotonView m_photonView;
    public List<string> m_randomNames;
    void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        RandomSpawn();
    }


    private void RandomSpawn()
    {

        if (m_photonView.IsMine)
        {
            PhotonNetwork.NickName = m_randomNames[Random.Range(0, m_randomNames.Count)];
            m_myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", m_myAvatar.name), Vector3.zero, Quaternion.identity, 0);
            
            
        }
    }
}
