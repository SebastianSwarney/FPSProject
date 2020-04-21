using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
	[Header("Screen Settings")]
	public FullScreenMode m_screenMode;
	public Vector2Int m_screenResolution;

	[Header("Sensitivity")]
	public float m_minSensitvity;
	public float m_maxSensitvity;
	public float m_sensitivity;

	[ContextMenu("Validate Settings")]
	public void SubmitSettings()
	{
		PlayerPrefs.SetFloat("sensitivity", m_sensitivity);
		Screen.SetResolution(m_screenResolution.x, m_screenResolution.y, m_screenMode);
	}

	public void ChangeSensitivity(float p_sensitivityProgress)
	{
		float currentSens = Mathf.Lerp(m_minSensitvity, m_maxSensitvity, p_sensitivityProgress);
		m_sensitivity = currentSens;
	}

	public void ChangeScreenSize(int p_settingIndex)
	{
		if (p_settingIndex == 0)
		{
			m_screenResolution = new Vector2Int(1920, 1080);
		}
		else if (p_settingIndex == 1)
		{
			m_screenResolution = new Vector2Int(1920 / 2, 1080 / 2);
		}
	}

	public void ChangeDisplaySetting(int p_settingIndex)
	{
		if (p_settingIndex == 0)
		{
			//Debug.Log("Windowed");
			m_screenMode = FullScreenMode.ExclusiveFullScreen;
		}
		else if (p_settingIndex == 1)
		{
			//Debug.Log("Fullscreen");
			m_screenMode = FullScreenMode.Windowed;
		}
		else if (p_settingIndex == 2)
		{
			//Debug.Log("Boarderless Windowed");
			m_screenMode = FullScreenMode.FullScreenWindow;
		}
	}
}
