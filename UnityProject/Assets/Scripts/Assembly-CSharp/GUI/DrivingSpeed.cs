using System;
using UnityEngine;

public class DrivingSpeed : MonoBehaviour
{
	public WheelCollider myWheelCollider;

	public int textBoxMarginSide;

	public int textBoxMarginBottom;

	public int shadowOffset = 2;

	public int textBoxWidth;

	public int textBoxHeight;

	public Texture2D backgroundImage;

	private GUIStyle style = new GUIStyle();

	private GUIStyle styleShadow = new GUIStyle();

	public Font myFont;

	public int fontSize = 16;

	private float currentSpeed;

	private int leftRect;

	private int topRect;

	private int widthRect;

	private int heightRect;

	private void Start()
	{
		style.font = myFont;
		style.normal.textColor = Color.cyan;
		style.fontSize = fontSize;
		styleShadow.font = myFont;
		styleShadow.normal.textColor = Color.black;
		styleShadow.fontSize = fontSize;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		leftRect = Screen.width - textBoxWidth - textBoxMarginSide;
		topRect = Screen.height - textBoxHeight - textBoxMarginBottom;
		widthRect = textBoxWidth;
		heightRect = textBoxHeight;
		currentSpeed = (float)Math.PI * 2f * myWheelCollider.radius * myWheelCollider.rpm * 60f / 1000f;
		currentSpeed = Mathf.Abs(Mathf.Round(currentSpeed));
		string text = string.Format("{0:000}", currentSpeed);
		Rect position = new Rect(Screen.width - 120, Screen.height - 60, 100f, 50f);
		GUI.DrawTexture(position, backgroundImage, ScaleMode.StretchToFill);
		Rect screenRect = new Rect(leftRect + shadowOffset, topRect + shadowOffset, widthRect, heightRect);
		GUILayout.BeginArea(screenRect);
		GUILayout.EndArea();
		Rect screenRect2 = new Rect(leftRect, topRect, widthRect, heightRect);
		GUILayout.BeginArea(screenRect2);
		GUILayout.Label(text, style);
		GUILayout.EndArea();
	}
}
