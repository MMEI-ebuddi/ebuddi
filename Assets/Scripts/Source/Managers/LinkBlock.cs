using System;
using System.Diagnostics;

namespace TMSupport
{
	public class TMLinkBlockNode
	{
		protected TMLinkBlockNode()
		{
			m_Next = -1;
			m_Prev = -1;
		}
		public object Next { get { return m_Next; } }
		internal object m_Next;
		public object Prev { get { return m_Prev; } }
		internal object m_Prev;

		public bool IsValidLinkBlock()
		{
			return (!(m_Prev is int || m_Next is int));
		}
	}
	public enum LinkType
	{
		Head = 0,
		Tail
	}

	public class TMLinkBlock<T> where T : TMLinkBlockNode, new()
	{
		public T	ActiveNodes { get { return m_ActiveNodes; } }
		private T	m_ActiveNodes = null;
		private T	m_ActiveNodesTail = null;
		public T	FreeNodes { get { return m_FreeNodes; } }
		private T	m_FreeNodes = null;
		int			m_NumAllocated;
		public int	NumActive { get { return m_NumActive; } }
		int			m_NumActive;
		public int	NumFree { get { return m_NumFree; } }
		int			m_NumFree;
		int			m_AllocExtra;
		int			m_NumReused;

//		const int	m_ExtraAlloc = 5;

		public TMLinkBlock(int AllocationSize, int AllocExtra)	//bool AllocExtraAllocations)	// = true
		{
 			m_NumActive = 0;
			m_ActiveNodes = null;
			m_ActiveNodesTail = null;
			AllocateBlock(AllocationSize);
			m_AllocExtra = AllocExtra;
//			m_AllodExtraAllocations = AllocExtraAllocations;
		}

		private void AllocateBlock(int AllocCount)
		{
			if (AllocCount == 0)
				return;

			T		NewNode = null;
			T		LastNode = null;

			for (int loop = 0; loop < AllocCount; loop++)
			{
				NewNode = new T();

				if (loop == 0)
				{
					m_FreeNodes = NewNode;
					NewNode.m_Prev = null;
				}

				if (LastNode != null)
				{
					LastNode.m_Next = NewNode;
					NewNode.m_Prev = LastNode;
				}
				LastNode = NewNode;
			}
 			if(LastNode != null)
 				LastNode.m_Next = null;
			m_NumAllocated += AllocCount;
			m_NumFree += AllocCount;
		}

		public T Reuse(T ReuseNode, LinkType LinkageType)
		{
			return (ReuseInternal(ReuseNode, LinkageType, true));
		}

		T ReuseInternal(T ReuseNode, LinkType LinkageType, bool ForceCheck = false)
		{
			// Make sure the reuse node is in the freelist
			if(ForceCheck)
			{
				T	NewNode = m_FreeNodes;

				while (NewNode != ReuseNode && NewNode != null)
				{
					NewNode = NewNode.m_Next as T;
				}
				if (NewNode == null)
					return (null);
			}

			if (ReuseNode.m_Prev != null)
				((ReuseNode.m_Prev) as T).m_Next = ReuseNode.m_Next as T;
			if (ReuseNode.m_Next != null)
				((ReuseNode.m_Next) as T).m_Prev = ReuseNode.m_Prev as T;

			if(ReuseNode == m_FreeNodes)
				m_FreeNodes = m_FreeNodes.m_Next as T;

			if (LinkageType == LinkType.Head)
			{
				if (m_ActiveNodes == null)
				{
					m_ActiveNodes = ReuseNode;
					m_ActiveNodesTail = m_ActiveNodes;
					ReuseNode.m_Next = null;
				}
				else
				{
					m_ActiveNodes.m_Prev = ReuseNode;
					ReuseNode.m_Next = m_ActiveNodes;
					m_ActiveNodes = ReuseNode;
				}
				ReuseNode.m_Prev = null;
			}
			else
			{
				if (m_ActiveNodesTail == null)
				{
					m_ActiveNodes = ReuseNode;
					m_ActiveNodesTail = m_ActiveNodes;
					ReuseNode.m_Prev = null;
				}
				else
				{
					m_ActiveNodesTail.m_Next = ReuseNode;
					ReuseNode.m_Prev = m_ActiveNodesTail;
					m_ActiveNodesTail = ReuseNode;
				}
				ReuseNode.m_Next = null;
			}
			m_NumFree--;
			m_NumActive++;
			m_NumReused++;

			return (ReuseNode);
		}

		public T Allocate()
		{
			return(Allocate(LinkType.Head));
		}
		public T AllocateEnd()
		{
			return(Allocate(LinkType.Tail));
		}
		public T Allocate(LinkType LinkageType)
		{
			T NewNode = null;

			if (m_FreeNodes == null)
			{
				if (m_AllocExtra > 0)
				{
					// Run out of free nodes, so allocate some more and link in
					AllocateBlock(m_AllocExtra);
				}
				else
					return (null);
			}

			// Allocate from free
			NewNode = m_FreeNodes;
			m_FreeNodes = m_FreeNodes.m_Next as T;

			if(LinkageType == LinkType.Head)
			{
				if (m_ActiveNodes == null)
				{
					m_ActiveNodes = NewNode;
					m_ActiveNodesTail = m_ActiveNodes;
					NewNode.m_Next = null;
				}
				else
				{
					m_ActiveNodes.m_Prev = NewNode;
					NewNode.m_Next = m_ActiveNodes;
					m_ActiveNodes = NewNode;
				}
				NewNode.m_Prev = null;
			}
			else
			{
				if (m_ActiveNodesTail == null)
				{
					m_ActiveNodes = NewNode;
					m_ActiveNodesTail = m_ActiveNodes;
					NewNode.m_Prev = null;
				}
				else
				{
					m_ActiveNodesTail.m_Next = NewNode;
					NewNode.m_Prev = m_ActiveNodesTail;
					m_ActiveNodesTail = NewNode;
				}
				NewNode.m_Next = null;
			}
			m_NumFree--;
			m_NumActive++;
			if (m_NumFree > m_NumAllocated)
			{
//				int parp = 1;
			}
			return (NewNode);
		}

		public T Add(T NewNode, LinkType LinkageType = LinkType.Head)
		{
			if(LinkageType == LinkType.Head)
			{
				if (m_ActiveNodes == null)
				{
					m_ActiveNodes = NewNode;
					m_ActiveNodesTail = m_ActiveNodes;
					NewNode.m_Next = null;
				}
				else
				{
					m_ActiveNodes.m_Prev = NewNode;
					NewNode.m_Next = m_ActiveNodes;
					m_ActiveNodes = NewNode;
				}
				NewNode.m_Prev = null;
			}
			else
			{
				if (m_ActiveNodesTail == null)
				{
					m_ActiveNodes = NewNode;
					m_ActiveNodesTail = m_ActiveNodes;
					NewNode.m_Prev = null;
				}
				else
				{
					m_ActiveNodesTail.m_Next = NewNode;
					NewNode.m_Prev = m_ActiveNodesTail;
					m_ActiveNodesTail = NewNode;
				}
				NewNode.m_Next = null;
			}
			m_NumActive++;
			m_NumAllocated++;
			return (NewNode);
		}

		public void ReleaseAll()
		{
			T		Node = m_ActiveNodes;
			T		NextNode;

			while (Node != null)
			{
				NextNode = Node.m_Next as T;

				if (m_FreeNodes != null)
					m_FreeNodes.m_Prev = Node;
				Node.m_Next = m_FreeNodes;
				Node.m_Prev = null;
				m_FreeNodes = Node;

				Node = NextNode;

				m_NumActive--;
				m_NumFree++;
			}
//			m_NumActive = 0;
			m_ActiveNodes = null;
			m_ActiveNodesTail = null;
		}

		public void Release(T Node)
		{
#if true
//			bool		BadNode = false;

			if(Node.m_Prev != null && Node.m_Prev is int)
			{
				Debug.Assert(false);
//				BadNode = true;
			}
			if(Node.m_Next != null && Node.m_Next is int)
			{
				Debug.Assert(false);
//				BadNode = true;
			}

			T		CheckNode = m_FreeNodes;
			int		Count = m_NumFree;

			while (CheckNode != null && Count > 0)
			{
				if (CheckNode == Node)
				{
					Debug.Assert(false);
				}
				CheckNode = CheckNode.m_Next as T;
				Count--;
			}
#endif
			if (Node == m_ActiveNodes)
				m_ActiveNodes = m_ActiveNodes.m_Next as T;
			if (Node == m_ActiveNodesTail)
				m_ActiveNodesTail = m_ActiveNodesTail.m_Prev as T;
//			else
			{
				if (Node.m_Prev != null)
					((Node.m_Prev) as T).m_Next = Node.m_Next;
				if (Node.m_Next != null)
					((Node.m_Next) as T).m_Prev = Node.m_Prev;
			}

			if(m_FreeNodes != null)
				m_FreeNodes.m_Prev = Node;
			Node.m_Next = m_FreeNodes;
			m_FreeNodes = Node;
			Node.m_Prev = null;

			m_NumFree++;
			m_NumActive--;

			if (m_NumFree > m_NumAllocated)
			{
				Debug.Assert(false);
			}
		}

		public bool IsActiveListGood()
		{
			T		Node = m_ActiveNodes;
			int		Count = m_NumActive;

			while (Node != null && Count > 0)
			{
				Node = Node.m_Next as T;
				Count--;
			}
			if (Count != 0 || Node != null)
				return (false);

			return (true);
		}

		public bool IsFreeListGood()
		{
			T		Node = m_FreeNodes;
			int		Count = m_NumFree;

			while (Node != null && Count > 0)
			{
				Node = Node.m_Next as T;
				Count--;
			}
			if (Count != 0 || Node != null)
				return (false);

			return (true);
		}

		public T FindFree(Type type)
		{
			T		Node = m_FreeNodes;
			int		Count = 0;

			Node = m_FreeNodes;
			while (Node != null)
			{
				if (Node.GetType() == type)
				{
					return (Node);
				}
				Node = Node.m_Next as T;
				Count++;
				if (Count > m_NumFree)
					Count = m_NumFree;
			}

			return (null);
		}
#if false
		public T Alloc(Type type)
		{
			T	NewNode = FindFree(type);

			if (NewNode == null)
			{
				NewNode = Activator.CreateInstance(type) as T;
				Add(NewNode, LinkType.Tail);
			}
			else
			{
				ReuseInternal(NewNode, LinkType.Tail);
			}

			return (NewNode);
		}
#else
		public T Alloc(Type type)
		{
			T		NewNode = null;
// BEGIN FindFree(type);			
			T		Node = m_FreeNodes;
			int		Count = 0;

			Node = m_FreeNodes;
			while (Node != null && NewNode == null)
			{
				if (Node.GetType() == type)
				{
					NewNode = Node;
					break;
				}
			
				Node = Node.m_Next as T;
				Count++;
				if (Count > m_NumFree)
					Count = m_NumFree;
			}
// END FindFree(type);			

			if (NewNode == null)
			{
				NewNode = Activator.CreateInstance(type) as T;
// BEGIN Add(NewNode, Support.LinkType.Tail);
				LinkType LinkageType = LinkType.Tail;
				
				if(LinkageType == LinkType.Head)
				{
					if (m_ActiveNodes == null)
					{
						m_ActiveNodes = NewNode;
						m_ActiveNodesTail = m_ActiveNodes;
						NewNode.m_Next = null;
					}
					else
					{
						m_ActiveNodes.m_Prev = NewNode;
						NewNode.m_Next = m_ActiveNodes;
						m_ActiveNodes = NewNode;
					}
					NewNode.m_Prev = null;
				}
				else
				{
					if (m_ActiveNodesTail == null)
					{
						m_ActiveNodes = NewNode;
						m_ActiveNodesTail = m_ActiveNodes;
						NewNode.m_Prev = null;
					}
					else
					{
						m_ActiveNodesTail.m_Next = NewNode;
						NewNode.m_Prev = m_ActiveNodesTail;
						m_ActiveNodesTail = NewNode;
					}
					NewNode.m_Next = null;
				}
				m_NumActive++;
				m_NumAllocated++;
			}
			else
			{
				ReuseInternal(NewNode, LinkType.Tail, false);
			}

			return (NewNode);
		}
#endif
	}
}
