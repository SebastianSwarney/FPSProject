using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PhotonPlayer_LeaveRoom : MonoBehaviour
{
    public KeyCode m_leaveKeycode;
    private bool m_roomLeft;
    private PhotonView m_photonView;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!m_photonView.IsMine) return;
        if (m_roomLeft) return;
        if (Input.GetKeyDown(m_leaveKeycode))
        {
            m_roomLeft = true;
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }
    }
}
