using UnityEngine;

// TODO: This is not used.
[ExecuteInEditMode]
public class DrawLineToNext : MonoBehaviour
{
	public bool show;

	private Color color = Color.cyan;

	private AIDriverController aiDriverController;

	public void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		color = base.GetComponent<Renderer>().sharedMaterial.color;
		string text = base.gameObject.name;
		int num = text.LastIndexOf("_");
		string text2 = text.Substring(0, num);
		string s = text.Substring(num + 1);
		int result;
		if (!int.TryParse(s, out result))
		{
			return;
		}
		string text3 = text2 + "_" + (result + 1);
		Transform transform = base.gameObject.transform.parent.Find(text3);
		if (transform != null)
		{
			if (show)
			{
				transform.GetComponent<Renderer>().sharedMaterial.color = base.GetComponent<Renderer>().sharedMaterial.color;
				Debug.DrawLine(base.transform.position, transform.position, color);
			}
			DrawLineToNext component = transform.gameObject.GetComponent<DrawLineToNext>();
			if (component != null)
			{
				component.show = show;
			}
		}
	}
}
