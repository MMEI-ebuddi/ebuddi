using System.Collections.Generic;

namespace TMSupport
{
	public delegate void MessageCallback(TMMessageNode Message);

	public class TMMessageNode : TMLinkBlockNode
	{
		public int		Message;
		public int		Sender;
		public int		Parent;
		public object	Custom;
		public bool		Busy;
        public BaseActionScriptableObject ActionSO;

		public TMMessageNode()
		{
			Busy = false;
		}

		public void Send()
		{
			TMMessenger.Instance.SendMessage(this);
		}
	}

	public class TMMessenger
	{



		static TMMessenger			sInstance;
		public static TMMessenger	Instance { get { if (sInstance == null) { sInstance = new TMMessenger(); } return sInstance; } }

		internal List<MessageCallback> m_MessageCallbackList = new List<MessageCallback>();

		TMMessageNode		m_StandardMessage = new TMMessageNode();

		TMLinkBlock<TMMessageNode> m_Manager = new TMLinkBlock<TMMessageNode>(10, 5);		// Start off with 10 but allow for extra allocations (in blocks of 5)

		TMMessenger()
		{
			sInstance = this;
		}

		public void AddToMessageQueue(MessageCallback myDelegate)
		{
			if (m_MessageCallbackList.IndexOf(myDelegate) != -1)
			{
			}
			else
				m_MessageCallbackList.Add(myDelegate);
		}
		public void RemoveFromMessageQueue(MessageCallback myDelegate)
		{
			m_MessageCallbackList.Remove(myDelegate);
		}
		public void RemoveFromMessageQueue(object DelegateTarget)
		{
			for(int loop = 0; loop < m_MessageCallbackList.Count; loop++)
			{
				if(m_MessageCallbackList[loop].Target == DelegateTarget)
				{
					m_MessageCallbackList.RemoveAt(loop);
					return;
				}
			}
		}

		public static void Send(int MessageIdent)
		{
			Instance.PrivateSend(MessageIdent, 0, null, null);
		}
        public static void Send(int MessageIdent, BaseActionScriptableObject so)
        {
            Instance.PrivateSend(MessageIdent, 0, null, so);
        }

		public static void Send(int MessageIdent, int SenderIdent)
		{
			Instance.PrivateSend(MessageIdent, SenderIdent, null,null);
		}
		public static void Send(int MessageIdent, int SenderIdent, object Custom, int ParentIdent = 0)
		{
			Instance.PrivateSend(MessageIdent, SenderIdent, Custom, null, ParentIdent);
		}
		private void PrivateSend(int MessageIdent, int SenderIdent, object CustomObject, BaseActionScriptableObject scriptableObject, int ParentIdent = 0)
		{
			TMMessageNode		NodeToUse = m_StandardMessage;

			if (m_StandardMessage.Busy == true)
				NodeToUse = Allocate();

			NodeToUse.Message = MessageIdent;
			NodeToUse.Sender = SenderIdent;
			NodeToUse.Custom = CustomObject;
			NodeToUse.Parent = ParentIdent;
            NodeToUse.ActionSO = scriptableObject;
			NodeToUse.Busy = true;
			SendMessage(NodeToUse);
			NodeToUse.Busy = false;
		}

		public void SendMessage(TMMessageNode Message)
		{
			// NOTE: cannot use a foreach here as this callback could setup other callbacks
			for (int loop = 0; loop < m_MessageCallbackList.Count; loop++)
			{
				m_MessageCallbackList[loop](Message);
			}
			if(Message != m_StandardMessage && Message.IsValidLinkBlock())
				m_Manager.Release(Message);
		}

		public TMMessageNode Allocate()
		{
			return (m_Manager.Allocate());
		}
	}
}
