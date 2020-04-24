using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    public EquipmentHud m_playerHud;
    private float m_hitTimer;
    [System.Serializable]
    public struct EquipmentHud
    {
        public Canvas m_equipmentHudCanvas;
        public UnityEngine.UI.Image m_healthBar;
        public TMPro.TextMeshProUGUI m_healthAmount;

        public TMPro.TextMeshProUGUI m_ammoAmount, m_clipAmount, m_clipTotal;

        public GameObject m_hitMarker;
        public float m_hitMarkerTime;

        public GameObject m_reloadingText;
    }

    public NameAndTeam m_nameAndTeam;
    [System.Serializable]
    public struct NameAndTeam
    {
        public TMPro.TextMeshProUGUI m_playerName;
        public TMPro.TextMeshProUGUI m_playerTeam;
    }

    public Canvas m_weaponLoadoutCanvas;


    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerHud(int p_healthAmount, Vector3Int p_ammoStats)
    {
        m_playerHud.m_healthBar.fillAmount = (float)p_healthAmount / 100f;
        m_playerHud.m_healthAmount.text = p_healthAmount.ToString();

        m_playerHud.m_ammoAmount.text = p_ammoStats.x.ToString();
        m_playerHud.m_clipAmount.text = p_ammoStats.y.ToString();
        m_playerHud.m_clipTotal.text = p_ammoStats.z.ToString();

    }

    public void UpdatePlayerNameAndTeam(string p_name, TeamTypes.TeamType p_team)
    {
        m_nameAndTeam.m_playerName.text = p_name;
        switch (p_team)
        {
            case TeamTypes.TeamType.Red:
                m_nameAndTeam.m_playerTeam.color = Color.red;
                m_nameAndTeam.m_playerTeam.text = "Red";
                break;
            case TeamTypes.TeamType.Blue:
                m_nameAndTeam.m_playerTeam.color = Color.blue;
                m_nameAndTeam.m_playerTeam.text = "Blue";
                break;
        }
    }

    public void ShowHitMarker()
    {
        m_hitTimer = 0;
    }

    private IEnumerator HitMarker()
    {
        while (true)
        {
            if (m_hitTimer < m_playerHud.m_hitMarkerTime)
            {
                m_playerHud.m_hitMarker.SetActive(true);
                m_hitTimer += Time.deltaTime;

            }
            else
            {
                m_playerHud.m_hitMarker.SetActive(false);
            }
            yield return null;
        }
    }


    public void PlayerDeathState(bool p_isDead)
    {

        m_playerHud.m_equipmentHudCanvas.gameObject.SetActive(!p_isDead);
        m_weaponLoadoutCanvas.gameObject.SetActive(p_isDead);
        ChangeCursorVisability(p_isDead);

    }

    private void ChangeCursorVisability(bool p_state)
    {
        PlayerInput.Instance.ChangeCursorState(p_state);
    }


    public void ChangeReloadingState(bool p_reloading)
    {
        m_playerHud.m_reloadingText.SetActive(p_reloading);
    }
}
