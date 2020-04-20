using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
	public bool m_isFullscreen;

	public Vector2Int m_screenSize;

	public float m_sensitivity;

	[Header("UI Theme Settings")]
	public Color m_buttonBackgroundColor;

	public Image[] m_buttonBackgroundImages;

	public Color m_textColor;

	private PlayerController m_player;

	private void Start()
	{
		m_player = GetComponent<PlayerController>();
	}

	[ContextMenu("Change Colors")]
	public void ChangeColors()
	{
		foreach (Image backgroundImages in m_buttonBackgroundImages)
		{
			backgroundImages.color = m_buttonBackgroundColor;
		}

		Text[] textComponents = GetComponentsInChildren<Text>();

		foreach (Text text in textComponents)
		{
			text.color = m_textColor;
		}

		//Text[] textComponents1 = FindObjectsOfType<Text>();
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
