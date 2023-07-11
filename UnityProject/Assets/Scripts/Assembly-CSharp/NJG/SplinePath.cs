using System.Collections.Generic;
using UnityEngine;

public class SplinePath : MonoBehaviour
{
	public int steps = 5;

	public bool loop = true;

	public Color color = Color.white;

	[HideInInspector]
	public List<Transform> path;

	private Vector3[] pathPositions;

	[HideInInspector]
	public List<Vector3> sequence;

	private bool isLoaded;

	protected virtual void Awake()
	{
		FillSequence();
	}

	protected virtual void OnDrawGizmos()
	{
		FillSequence();
		DrawGizmos();
	}

	protected void DrawGizmos()
	{
		if (sequence == null)
		{
			return;
		}
		int i = 0;
		int count = sequence.Count;
		int num = 0;
		for (; i < count; i++)
		{
			if (i < count - 1)
			{
				Debug.DrawLine(sequence[i], sequence[i + 1], color);
			}
			else if (loop)
			{
				Debug.DrawLine(sequence[count - 1], sequence[0], color);
			}
		}
	}

	protected void FillSequence()
	{
		sequence = SplineCalculation.NewCatmullRom(path, steps, loop);
	}
}
