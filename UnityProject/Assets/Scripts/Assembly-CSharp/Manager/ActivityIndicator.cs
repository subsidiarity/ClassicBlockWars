using UnityEngine;

[ExecuteInEditMode]
public class ActivityIndicator : MonoBehaviour
{
	public static bool activEnabled;

	public static ActivityIndicator thisScript;

	public Texture2D texture;

	public float angle;

	public Vector2 size = new Vector2(128f, 128f);

	private Vector2 pos = new Vector2(0f, 0f);

	private Rect rect;

	private Vector2 pivot;

	private float rotSpeed = 180f;

	private void Awake()
	{
		if (CompilationSettings.GraphicsOverideDisabled)
		{
			return;
		}

		if (Device.isWeakDevice)
		{
			Screen.SetResolution(Mathf.RoundToInt((float)Screen.width * 0.75f), Mathf.RoundToInt((float)Screen.height * 0.75f), true);
		}
		if (Device.isWeakDevice)
		{
			QualitySettings.SetQualityLevel(0, true);
		}
		else
		{
			QualitySettings.SetQualityLevel(1, true);
		}
	}

	private void Start()
	{
		thisScript = this;
		Object.DontDestroyOnLoad(base.gameObject);
		size *= (float)Screen.width / 768f;
		UpdateSettings();
		Invoke("UpdateSettings", 0.5f);
	}

	private void UpdateSettings()
	{
		pos = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
		rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
		pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
	}

	private void OnGUI()
	{
		if (activEnabled)
		{
			GUI.depth = 0;
			angle = rotSpeed * Time.realtimeSinceStartup;
			angle = (int)angle % 360;
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(angle, pivot);
			GUI.DrawTexture(rect, texture);
			GUI.matrix = matrix;
		}
	}
}
