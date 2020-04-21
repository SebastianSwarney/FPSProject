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

	[Header("Button Tags")]
	public string m_buttonMainTag;
	public string m_buttonOutlineTag;
	public string m_buttonArrowTag;

	[Header("Element Tags")]
	public string m_elementMainTag;

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
		ChangeText();
		ChangeButtons();
		ChangeElements();
	}

	private void ChangeText()
	{
		Text[] textComponents = FindObjectsOfType<Text>();

		foreach (Text text in textComponents)
		{
			text.font = m_currentUIPallet.m_font;
			text.color = m_currentUIPallet.m_textColor;
		}
	}

	private void ChangeButtons()
	{
		ChangeUIImageByTag(m_buttonMainTag, m_currentUIPallet.m_buttonMainColor);
		ChangeUIImageByTag(m_buttonOutlineTag, m_currentUIPallet.m_buttonOutlineColor);
		ChangeUIImageByTag(m_buttonArrowTag, m_currentUIPallet.m_buttonArrowColor);
	}

	private void ChangeElements()
	{
		ChangeUIImageByTag(m_elementMainTag, m_currentUIPallet.m_elementMainColor);
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
}
