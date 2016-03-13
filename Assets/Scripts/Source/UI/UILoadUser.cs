using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TMSupport
{
	public class UILoadUser : MonoBehaviour
	{
		public InputField		m_UserName;
		public Button			m_BeginButton;
		public Text				m_ProfileNotFoundText;

		void OnEnable()
		{
			m_UserName.text = "";
			m_ProfileNotFoundText.gameObject.SetActive(false);
		}

		void Update()
		{
		}

		public void Back()
		{
			m_ProfileNotFoundText.gameObject.SetActive(false);
			TMMessenger.Send("ENTRYEXITED".GetHashCode());
			UIManager.Instance.CloseUI(UIManager.eUIReferenceName.LoadUser);
		}

		public void Close()
		{
			if(m_UserName.text.Length > 0)
			{
				if(UserProfileManager.Instance.DoesUserProfileExist(m_UserName.text))
				{
					m_ProfileNotFoundText.gameObject.SetActive(false);
					if (UserProfile.Load(m_UserName.text + ".xml"))
					{
						TMMessenger.Send("LOAD_USER_COMPLETE".GetHashCode());
						UIManager.Instance.CloseUI(UIManager.eUIReferenceName.LoadUser);

					}
				}
				else
					m_ProfileNotFoundText.gameObject.SetActive(true);
			}
		}
	}
}
