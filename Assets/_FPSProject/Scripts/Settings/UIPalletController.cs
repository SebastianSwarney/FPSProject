using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIPalletController : MonoBehaviour
{
	public bool m_liveUpdate;

	[Header("UI Theme Settings")]
	public UIPalletData m_currentUIPallet;

	[Header("Base Element Tags")]
	public string m_settingDescriptionTextTag;
	public string m_baseElementTag;

	[Header("Base Button Tags")]
	public string m_buttonBackgroundTag;
	public string m_buttonOutlineTag;

	[Header("Basic Button Tags")]
	public string m_basicButtonTextTag;

	[Header("Setting Button Tags")]
	public string m_settingButtonTextTag;
	public string m_settingButtonSelectTag;
	public string m_settingButtonDeselectTag;
	public string m_settingButtonArrowTag;

	[Header("Slider Tags")]
	public string m_sliderInputTextTag;
	public string m_sliderHandleTag;
	public string m_sliderFillTag;
	public string m_sliderBackgroundTag;

	private void Update()
	{
		if (m_liveUpdate)
		{
			ValidatePallet();
		}
	}

	[ContextMenu("Validate Pallet")]
	public void ValidatePallet()
	{
		ChangeGlobalText();
		ChangeBaseElements();
		ChangeBaseButtons();
		ChangeBasicButtons();
		ChangeSettingButtons();
		ChangeSliders();
	}

	private void ChangeGlobalText()
	{
		Text[] textComponents = FindObjectsOfType<Text>();

		foreach (Text text in textComponents)
		{
			text.font = m_currentUIPallet.m_font;
			text.color = m_currentUIPallet.m_textColor;
		}
	}

	private void ChangeBaseElements()
	{
		ChangeTextSizeByTag(m_settingDescriptionTextTag, m_currentUIPallet.m_settingDescriptionTextSize);
		ChangeUIImageByTag(m_baseElementTag, m_currentUIPallet.m_baseElementMainColor);
	}

	private void ChangeBaseButtons()
	{
		ChangeUIImageByTag(m_buttonBackgroundTag, m_currentUIPallet.m_buttonBackgroundColor);
		ChangeUIImageByTag(m_buttonOutlineTag, m_currentUIPallet.m_buttonOutlineColor);
	}

	private void ChangeBasicButtons()
	{
		ChangeTextSizeByTag(m_basicButtonTextTag, m_currentUIPallet.m_basicButtonTextSize);
	}

	private void ChangeSettingButtons()
	{
		ChangeTextSizeByTag(m_settingButtonTextTag, m_currentUIPallet.m_settingButtonTextSize);
		ChangeUIImageByTag(m_settingButtonSelectTag, m_currentUIPallet.m_settingButtonSelectColor);
		ChangeUIImageByTag(m_settingButtonDeselectTag, m_currentUIPallet.m_settingButtonDeselectColor);
		ChangeUIImageByTag(m_settingButtonArrowTag, m_currentUIPallet.m_settingButtonArrowColor);
	}

	private void ChangeSliders()
	{
		ChangeTextSizeByTag(m_sliderInputTextTag, m_currentUIPallet.m_sliderInputTextSize);
		ChangeUIImageByTag(m_sliderHandleTag, m_currentUIPallet.m_sliderHandleColor);
		ChangeUIImageByTag(m_sliderFillTag, m_currentUIPallet.m_sliderFillColor);
		ChangeUIImageByTag(m_sliderBackgroundTag, m_currentUIPallet.m_sliderBackgroundColor);
	}

	private void ChangeUIImageByTag(string p_tag, Color p_imageColor)
	{
		Image[] images = FindUIImagesWithTag(p_tag);

		foreach (Image image in images)
		{
			image.color = p_imageColor;
		}
	}

	private Image[] FindUIImagesWithTag(string p_tag)
	{
		GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(p_tag);

		List<Image> foundImages = new List<Image>();

		foreach (GameObject gameObject in foundObjects)
		{
			foundImages.Add(gameObject.GetComponent<Image>());
		}
		return foundImages.ToArray();
	}

	private void ChangeTextSizeByTag(string p_tag, int p_textSize)
	{
		Text[] foundText = FindTextWithTag(p_tag);

		foreach (Text text in foundText)
		{
			text.fontSize = p_textSize;
		}
	}

	private Text[] FindTextWithTag(string p_tag)
	{
		GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(p_tag);

		List<Text> foundText = new List<Text>();

		foreach (GameObject gameObject in foundObjects)
		{
			foundText.Add(gameObject.GetComponent<Text>());
		}
		return foundText.ToArray();
	}
}
