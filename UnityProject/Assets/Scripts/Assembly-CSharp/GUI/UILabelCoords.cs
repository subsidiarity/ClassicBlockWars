using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Label Coords")]
[RequireComponent(typeof(UILabel))]
public class UILabelCoords : MonoBehaviour
{
	public string format = "X:{0},Y:{1}";

	private UILabel label;

	private string mContent;

	private void Awake()
	{
		label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!(UIMiniMap.instance == null) && (bool)UIMiniMap.instance.target)
		{
			int num = (int)UIMiniMap.instance.target.position.x;
			int num2 = (int)((NJGMap.instance.orientation != 0) ? UIMiniMap.instance.target.position.y : UIMiniMap.instance.target.position.z);
			mContent = string.Format(format, num, num2);
			if (label.text != mContent)
			{
				label.text = mContent;
			}
		}
	}
}
