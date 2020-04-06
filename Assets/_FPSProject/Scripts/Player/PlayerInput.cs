using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Photon.Pun;

public class PlayerInput : MonoBehaviour
{
    public int m_playerId;

    private PlayerController m_playerController;
    private Player m_playerInputController;

    private bool m_lockLooking;

    private EquipmentController m_equipmentController;


    private void Start()
    {
        m_equipmentController = GetComponent<EquipmentController>();
        m_playerController = GetComponent<PlayerController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);
    }

    private void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        Vector2 movementInput = new Vector2(m_playerInputController.GetAxis("MoveHorizontal"), m_playerInputController.GetAxis("MoveVertical"));
        m_playerController.SetMovementInput(movementInput);

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_lockLooking = !m_lockLooking;
        }

        if (!m_lockLooking)
        {
            Vector2 lookInput = new Vector2(m_playerInputController.GetAxis("LookHorizontal"), m_playerInputController.GetAxis("LookVertical"));
            m_playerController.SetLookInput(lookInput);
        }

        if (m_playerInputController.GetButtonDown("Jump"))
        {
            m_playerController.OnJumpInputDown();
        }
        if (m_playerInputController.GetButtonUp("Jump"))
        {
            m_playerController.OnJumpInputUp();
        }

        if (m_playerInputController.GetButtonDown("WallRide"))
        {

        }

        if (m_playerInputController.GetButtonUp("WallRide"))
        {

        }

        if (m_playerInputController.GetButtonDown("Crouch"))
        {
            m_playerController.OnCrouchInputDown();
        }
        if (m_playerInputController.GetButtonUp("Crouch"))
        {
            //m_playerController.OnCrouchInputUp();
        }

        if (m_playerInputController.GetButton("Fire"))
        {
            m_equipmentController.OnShootInputDown();
        }

        if (m_playerInputController.GetButtonUp("Fire"))
        {
            m_equipmentController.OnShootInputUp();
        }

        if (m_playerInputController.GetButtonDown("Aim"))
        {

            

        }

        if (m_playerInputController.GetButtonUp("Aim"))
        {
            
        }
    }
}