using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class SettingButtonEvent : UnityEvent<int> { }

public class SettingButton : MonoBehaviour
{

	public string[] m_options;
	
	public GameObject m_settingCounterObject;
	public Transform m_settingCoutnerParent;

	public Image m_selectedOptionColor;
	public Image m_deselectedOptionColor;

	public SettingButtonEvent m_onValueChanged;

	private Text m_currentSettingDisplayText;

	private int m_currentOption;
	private Image[] m_settingCounterObjects;
	


	private void Start()
	{
		m_currentSettingDisplayText = GetComponentInChildren<Text>();

		CreateSettingDisplay();

		ChangeDisplay(m_currentOption);
	}

	public void CreateSettingDisplay()
	{
		List<Image> tempList = new List<Image>();

		foreach (string option in m_options)
		{
			GameObject newObject =  ObjectPooler.instance.NewObject(m_settingCounterObject, Vector3.zero, Quaternion.identity);
			newObject.transform.SetParent(m_settingCoutnerParent);
			newObject.transform.SetParent(m_settingCoutnerParent);

			tempList.Add(newObject.GetComponent<Image>());
		}

		m_settingCounterObjects = tempList.ToArray();
	}

	public void ChangeSelection(int p_selectionDir)
	{
		if (p_selectionDir < 0)
		{
			m_currentOption--;

			if (m_currentOption < 0)
			{
				m_currentOption = m_options.Length - 1;
			}
		}
		else
		{
			m_currentOption++;

			if (m_currentOption > m_options.Length - 1)
			{
				m_currentOption = 0;
			}
		}

		ChangeDisplay(m_currentOption);
	}

	public void ChangeDisplay(int p_displayIndex)
	{
		m_currentSettingDisplayText.text = m_options[p_displayIndex];

		foreach (Image image in m_settingCounterObjects)
		{
			image.color = m_deselectedOptionColor.color;
		}

		m_settingCounterObjects[p_displayIndex].color = m_selectedOptionColor.color;
		m_onValueChanged.Invoke(p_displayIndex);
	}
}
