using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TMSupport
{
	public class UIDialog : MonoBehaviour
	{
		public Text		m_Header;
		public Text		m_Text;
		public Image	m_Image;
		public Button	m_CloseButton;

		internal void SetText(string Text)
		{
			if (m_Text != null)
				m_Text.text = Text;

			Show();
		}

		internal void SetTextAndHeaderAndImage(string Text, string Header, Sprite Image)
		{
			if (m_Text != null)
				m_Text.text = Text;

			if (m_Header != null)
				m_Header.text = Header;

			if (m_Image != null)
				m_Image.sprite = Image;

			Show();
		}

		internal void DisableCloseButton(bool Disable = true)
		{
			if(m_CloseButton != null)
			{
				m_CloseButton.gameObject.SetActive(!Disable);
			}
		}

		public virtual void Show() {
			this.gameObject.SetActive(true);
		}

		public virtual void Hide() {
			this.gameObject.SetActive(false);
		}
	}
}
