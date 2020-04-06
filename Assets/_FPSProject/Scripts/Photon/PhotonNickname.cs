using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PhotonNickname : MonoBehaviour
{
    public UnityEngine.UI.Text m_nicknameText;
    public TMPro.TextMeshProUGUI m_firstPersonNicknameText;

    private PhotonView m_pv;

    private void Awake()
    {
        m_pv = GetComponent<PhotonView>();
        m_nicknameText.text = m_pv.Owner.NickName;
        m_firstPersonNicknameText.text = m_pv.Owner.NickName;
    }
}
