using System;
using UnityEngine;

[Serializable]
public class ExampleJava : MonoBehaviour
{
	private string stringToEdit;

	public Texture2D Texture2D1;

	public Texture2D Texture2D2;

	public Texture2D Texture2D3;

	private Texture2D DrawTexture;

	public ExampleJava()
	{
		stringToEdit = string.Empty;
	}

	public virtual void Start()
	{
		DrawTexture = Texture2D3;
	}

	public virtual void OnGUI()
	{
		GUI.Box(new Rect(220f, 10f, 200f, 200f), "JavaScript - String");
		if (GUI.Button(new Rect(230f, 40f, 180f, 30f), "Save"))
		{
			Save.SaveString("JavaString", stringToEdit);
		}
		if (GUI.Button(new Rect(230f, 80f, 180f, 30f), "Load"))
		{
			stringToEdit = Load.LoadString("JavaString");
		}
		if (GUI.Button(new Rect(230f, 120f, 180f, 30f), "Clear"))
		{
			stringToEdit = string.Empty;
		}
		stringToEdit = GUI.TextField(new Rect(230f, 170f, 180f, 20f), stringToEdit, 25);
		GUI.Box(new Rect(220f, 220f, 200f, 220f), "JavaScript - SaveTexture2D");
		GUI.DrawTexture(new Rect(230f, 250f, 50f, 50f), Texture2D1);
		if (GUI.Button(new Rect(300f, 260f, 110f, 25f), "Use Texture2D"))
		{
			DrawTexture = Texture2D1;
		}
		GUI.DrawTexture(new Rect(230f, 310f, 50f, 50f), Texture2D2);
		if (GUI.Button(new Rect(300f, 320f, 110f, 25f), "Use Texture2D"))
		{
			DrawTexture = Texture2D2;
		}
		GUI.DrawTexture(new Rect(230f, 370f, 50f, 50f), DrawTexture);
		if (GUI.Button(new Rect(300f, 370f, 50f, 25f), "Save"))
		{
			Save.SaveTexture2D("C#Texture2D", DrawTexture);
		}
		if (GUI.Button(new Rect(360f, 370f, 50f, 25f), "Load"))
		{
			DrawTexture = Load.LoadTexture2D("C#Texture2D");
		}
		if (GUI.Button(new Rect(300f, 400f, 110f, 25f), "Clear"))
		{
			DrawTexture = Texture2D3;
		}
	}

	public virtual void Main()
	{
	}
}
