using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
	public bool m_isFullscreen;

	public Vector2Int m_screenSize;

	public float m_sensitivity;

	private PlayerController m_player;

	private void Start()
	{
		m_player = GetComponent<PlayerController>();
	}

	[ContextMenu("Validate Settings")]
	public void SubmitSettings()
	{
		PlayerPrefs.SetFloat("sensitivity", m_sensitivity);

		m_player.m_cameraProperties.m_mouseSensitivity = PlayerPrefs.GetFloat("sensitivity");
	}

	public void ChangeDisplaySetting(int p_settingIndex)
	{
		if (p_settingIndex == 0)
		{
			//Debug.Log("Windowed");
		}
		else if (p_settingIndex == 1)
		{
			//Debug.Log("Fullscreen");
		}
		else if (p_settingIndex == 2)
		{
			//Debug.Log("Boarderless Windowed");
		}
	}
}
