using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
	public enum ScreenMode { Windowed, Fullscreen, Borderless_Windowed }

	[Header("Screen Mode Settings")]
	public ScreenMode m_currentScreenMode;
	public SettingButton m_screenModeButton;

	public enum ScreenResolution { z1920_x_1080, z720_x_480, z640_x_360 }

	[Header("Resolution Settings")]
	public ScreenResolution m_currentResolution;
	public SettingButton m_resolutionButton;
	private Vector2Int m_screenResolution;

	[Header("Sensitivity")]
	public float m_sensitivity;
	public SettingSlider m_sensitivitySlider;

	private void Start()
	{
		GetValuesFromSaves();
		SetupButtonsandSliders();
	}

	#region Setup Code
	private void GetValuesFromSaves()
	{
		m_currentScreenMode = (ScreenMode)PlayerPrefs.GetInt("screenMode");
		m_currentResolution = (ScreenResolution)PlayerPrefs.GetInt("screenResolution");

		m_sensitivity = PlayerPrefs.GetFloat("sensitivity");
	}

	private void SetupButtonsandSliders()
	{
		m_sensitivitySlider.InitializeSlider(m_sensitivity);

		InitializeSettingButton(typeof(ScreenMode), (int)m_currentScreenMode, m_screenModeButton);
		InitializeSettingButton(typeof(ScreenResolution), (int)m_currentResolution, m_resolutionButton);
	}

	private void InitializeSettingButton(Type p_settingType, int p_optionInSettings, SettingButton p_button)
	{
		string[] optionTypes = Enum.GetNames(p_settingType);

		for (int i = 0; i < optionTypes.Length; i++)
		{
			string oldText = optionTypes[i];
			string replaceUnderscore = oldText.Replace("_", " ");
			string replaceZ = replaceUnderscore.Replace("z", "");
			optionTypes[i] = replaceZ;
		}

		p_button.InitializeButton(p_optionInSettings, optionTypes);
	}
	#endregion

	#region Submit Settings Code
	public void SubmitSettingsToPerfs()
	{
		SubmitScreenSettings();
		PlayerPrefs.SetFloat("sensitivity", m_sensitivity);	
	}

	private void SubmitScreenSettings()
	{
		PlayerPrefs.SetInt("screenMode", (int)m_currentScreenMode);
		PlayerPrefs.SetInt("screenResolution", (int)m_currentResolution);

		switch (m_currentScreenMode)
		{
			case ScreenMode.Windowed:

				Screen.SetResolution(m_screenResolution.x, m_screenResolution.y, FullScreenMode.Windowed);

				break;

			case ScreenMode.Fullscreen:

				Screen.SetResolution(m_screenResolution.x, m_screenResolution.y, FullScreenMode.ExclusiveFullScreen);

				break;

			case ScreenMode.Borderless_Windowed:

				Screen.SetResolution(m_screenResolution.x, m_screenResolution.y, FullScreenMode.FullScreenWindow);

				break;
		}
	}
	#endregion

	#region Value Change Functions
	public void ChangeSensitivity(float p_sensitivityProgress)
	{
		m_sensitivity = p_sensitivityProgress;
	}

	public void ChangeDisplaySetting(int p_settingIndex)
	{
		m_currentScreenMode = (ScreenMode)p_settingIndex;
	}

	public void ChangeScreenResolution(int p_settingIndex)
	{
		m_currentResolution = (ScreenResolution)p_settingIndex;

		switch (m_currentResolution)
		{
			case ScreenResolution.z1920_x_1080:

				m_screenResolution = new Vector2Int(1920, 1080);

				break;
			case ScreenResolution.z720_x_480:

				m_screenResolution = new Vector2Int(720, 480);

				break;
			case ScreenResolution.z640_x_360:

				m_screenResolution = new Vector2Int(640, 360);

				break;
		}
	}
	#endregion
}
