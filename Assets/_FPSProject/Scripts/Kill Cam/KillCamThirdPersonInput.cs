using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class KillCamThirdPersonInput : MonoBehaviour
{
    public int m_playerId;

    private KillCamThirdPersonController m_controller;

    private Player m_playerInputController;

    private bool m_lockLooking;

    private void Start()
    {
        m_controller = GetComponent<KillCamThirdPersonController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);
    }

    private void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        Vector3 movementInput = new Vector3(m_playerInputController.GetAxis("ThirdPersonSideways"), m_playerInputController.GetAxis("ThirdPersonForward"), m_playerInputController.GetAxis("ThirdPersonVertical"));
        m_controller.SetMovementInput(movementInput);

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_lockLooking = !m_lockLooking;
        }

        if (!m_lockLooking)
        {
            Vector2 lookInput = new Vector2(m_playerInputController.GetAxis("ThirdPersonLookHorizontal"), m_playerInputController.GetAxis("ThirdPersonLookVertical"));
            m_controller.SetLookInput(lookInput);
        }
    }
}
