using UnityEngine;

namespace Edelweiss.DecalSystem
{
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class DecalsMeshRenderer : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private MeshFilter m_MeshFilter;

		[HideInInspector]
		[SerializeField]
		private MeshRenderer m_MeshRenderer;

		public MeshFilter MeshFilter
		{
			get
			{
				if (m_MeshFilter == null)
				{
					m_MeshFilter = GetComponent<MeshFilter>();
				}
				return m_MeshFilter;
			}
		}

		public MeshRenderer MeshRenderer
		{
			get
			{
				if (m_MeshRenderer == null)
				{
					m_MeshRenderer = GetComponent<MeshRenderer>();
				}
				return m_MeshRenderer;
			}
		}
	}
}
