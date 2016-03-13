using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TMSupport
{
	public class UIBasicEntry : MonoBehaviour
	{
		public Toggle			m_Male;
		public Toggle			m_Female;
		public InputField		m_UserName;
		public InputField		m_UserAge;

		public Button			m_BeginButton;

		void OnEnable()
		{
			m_Male.isOn = false;
            m_Female.isOn = true;
			m_UserName.text = "";
			m_UserAge.text = "";
		}

		void Update()
		{
		}

		public void Back()
		{
			TMMessenger.Send("ENTRYEXITED".GetHashCode());
			UIManager.Instance.CloseUI(UIManager.eUIReferenceName.BasicEntry);
		}

		public void Close()
		{
			if(m_UserName.text.Length > 0 && m_UserAge.text.Length > 0)
			{
				UserProfile.eSex		Sex = m_Male.isOn ? UserProfile.eSex.Male : UserProfile.eSex.Female;

				UserProfile.sCurrent.SetNameSexAge(m_UserName.text, Sex, Convert.ToInt32(m_UserAge.text));
				UserProfile.sCurrent.SceneComplete(0);

				//				OurSaveState.Instance.SetString("Name", m_UserName.text);
//				OurSaveState.Instance.SetInt("Age", Convert.ToInt32(m_UserAge.text));
				TMMessenger.Send("NEW_USER_COMPLETE".GetHashCode());
			}
		}
	}
}
