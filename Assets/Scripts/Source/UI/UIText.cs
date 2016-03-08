using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//namespace TMSupport
//{
	public class UIText : Text
	{
		public string		m_TextIdent;

		new void Awake()
		{
			Text		TextElement = GetComponent<Text>();

			TextElement.text = MediaNodeManager.GetText(m_TextIdent);

			base.Awake();
		}
	}
//}
