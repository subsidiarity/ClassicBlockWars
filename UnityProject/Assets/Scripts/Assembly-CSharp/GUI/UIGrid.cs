using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1
	}

	public enum Sorting
	{
		None = 0,
		Alphabetic = 1,
		Horizontal = 2,
		Vertical = 3,
		Custom = 4
	}

	public delegate void OnReposition();

	public Arrangement arrangement;

	public Sorting sorting;

	public UIWidget.Pivot pivot;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool animateSmoothly;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	[SerializeField]
	[HideInInspector]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!mInitDone)
		{
			Init();
		}
		bool flag = animateSmoothly;
		animateSmoothly = false;
		Reposition();
		animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(BetterList<Transform> list)
	{
		list.Sort(SortByName);
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (!mInitDone)
		{
			Init();
		}
		mReposition = false;
		Transform transform = base.transform;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (sorting != 0 || sorted)
		{
			BetterList<Transform> betterList = new BetterList<Transform>();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if ((bool)child && (!hideInactive || NGUITools.GetActive(child.gameObject)))
				{
					betterList.Add(child);
				}
			}
			if (sorting == Sorting.Alphabetic)
			{
				betterList.Sort(SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				betterList.Sort(SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				betterList.Sort(SortVertical);
			}
			else
			{
				Sort(betterList);
			}
			int j = 0;
			for (int size = betterList.size; j < size; j++)
			{
				Transform transform2 = betterList[j];
				if (NGUITools.GetActive(transform2.gameObject) || !hideInactive)
				{
					float z = transform2.localPosition.z;
					Vector3 vector = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z));
					if (animateSmoothly && Application.isPlaying)
					{
						SpringPosition.Begin(transform2.gameObject, vector, 15f).updateScrollView = true;
					}
					else
					{
						transform2.localPosition = vector;
					}
					num3 = Mathf.Max(num3, num);
					num4 = Mathf.Max(num4, num2);
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < transform.childCount; k++)
			{
				Transform child2 = transform.GetChild(k);
				if (NGUITools.GetActive(child2.gameObject) || !hideInactive)
				{
					float z2 = child2.localPosition.z;
					Vector3 vector2 = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z2) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z2));
					if (animateSmoothly && Application.isPlaying)
					{
						SpringPosition.Begin(child2.gameObject, vector2, 15f).updateScrollView = true;
					}
					else
					{
						child2.localPosition = vector2;
					}
					num3 = Mathf.Max(num3, num);
					num4 = Mathf.Max(num4, num2);
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		if (pivot != 0)
		{
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(pivot);
			float num5;
			float num6;
			if (arrangement == Arrangement.Horizontal)
			{
				num5 = Mathf.Lerp(0f, (float)num3 * cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-num4) * cellHeight, 0f, pivotOffset.y);
			}
			else
			{
				num5 = Mathf.Lerp(0f, (float)num4 * cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-num3) * cellHeight, 0f, pivotOffset.y);
			}
			for (int l = 0; l < transform.childCount; l++)
			{
				Transform child3 = transform.GetChild(l);
				if (NGUITools.GetActive(child3.gameObject) || !hideInactive)
				{
					SpringPosition component = child3.GetComponent<SpringPosition>();
					if (component != null)
					{
						component.target.x -= num5;
						component.target.y -= num6;
						continue;
					}
					Vector3 localPosition = child3.localPosition;
					localPosition.x -= num5;
					localPosition.y -= num6;
					child3.localPosition = localPosition;
				}
			}
		}
		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(transform, true);
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}
}
