using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageOptions
{
	public enum ValueType
	{
		None = 0,
		Object = 1,
		Text = 2,
		Numeric = 3,
		Vector2 = 4,
		Vector3 = 5
	}

	public List<string> message = new List<string>();

	public List<ValueType> type = new List<ValueType>();

	public List<UnityEngine.Object> obj = new List<UnityEngine.Object>();

	public List<string> text = new List<string>();

	public List<float> num = new List<float>();

	public List<Vector2> vect2 = new List<Vector2>();

	public List<Vector3> vect3 = new List<Vector3>();
}
