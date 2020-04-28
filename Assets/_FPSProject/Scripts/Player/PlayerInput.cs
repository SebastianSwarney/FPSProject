﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Photon.Pun;

public class PlayerInput : MonoBehaviour
{
    public int m_playerId;

    public float m_sensitivity;
    private float m_currentSensitivity;

    private PlayerController m_playerController;
    private Player m_playerInputController;

    private bool m_lockLooking;

    private EquipmentController m_equipmentController;

    public static PlayerInput Instance;
    private void Awake()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Instance = this;
        }
        m_currentSensitivity = m_sensitivity;
    }
    private void Start()
    {
        m_equipmentController = GetComponent<EquipmentController>();
        m_playerController = GetComponent<PlayerController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);

        ReadSettings();
    }

    public void ChangeCursorState(bool p_activeState)
    {
        Cursor.visible = !false;

        if (!p_activeState)
        {

            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Update()
    {
        GetInput();
    }

    private void ReadSettings()
    {
        //m_sensitivity = PlayerPrefs.GetFloat("sensitivity");
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
            m_playerController.SetLookInput(lookInput, m_currentSensitivity);
        }

        if (m_playerInputController.GetButtonDown("Jump"))
        {
            m_playerController.OnJumpInputDown();
        }
        if (m_playerInputController.GetButtonUp("Jump"))
        {
            m_playerController.OnJumpInputUp();
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
            m_equipmentController.OnAimDown();
        }

        if (m_playerInputController.GetButtonUp("Aim"))
        {
            m_equipmentController.OnAimUp();
        }

        if (m_playerInputController.GetButtonDown("DoubleAim"))
        {
            m_equipmentController.OnDoubleAimDown();
        }

        if (m_playerInputController.GetButtonDown("Equip"))
        {
            m_equipmentController.OnEquipDown();
        }
        if (m_playerInputController.GetButtonDown("Reload"))
        {
            m_equipmentController.OnReloadDown();
        }

        if(m_playerInputController.GetAxis("SwapWeapons") != 0)
        {
            m_equipmentController.OnSwapDown();
        }
    }

    public void ChangeSensitivity(float m_multiplier)
    {
        m_currentSensitivity = m_sensitivity * m_multiplier;
    }
}