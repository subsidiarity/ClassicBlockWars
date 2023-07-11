using System.Collections.Generic;
using UnityEngine;

public class SplineCalculation
{
	public static List<Vector3> NewCatmullRom(List<Transform> nodes, int slices, bool loop)
	{
		List<Vector3> list = new List<Vector3>();
		if (nodes.Count >= 2)
		{
			list.Add(GetPosition(nodes[0]));
			int num = nodes.Count - 1;
			for (int i = 0; (!loop && i < num) || (loop && i <= num); i++)
			{
				int index = ((i != 0) ? (i - 1) : ((!loop) ? i : num));
				int index2 = i;
				int num2 = ((i != num) ? (i + 1) : ((!loop) ? i : 0));
				int index3 = ((num2 != num) ? (num2 + 1) : ((!loop) ? num2 : 0));
				int num3 = slices + 1;
				for (int j = 1; j <= num3; j++)
				{
					list.Add(CatmullRom(GetPosition(nodes[index]), GetPosition(nodes[index2]), GetPosition(nodes[num2]), GetPosition(nodes[index3]), j, num3));
				}
			}
		}
		return list;
	}

	private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
	{
		float num = elapsedTime / duration;
		float num2 = num * num;
		float num3 = num2 * num;
		return previous * (-0.5f * num3 + num2 - 0.5f * num) + start * (1.5f * num3 + -2.5f * num2 + 1f) + end * (-1.5f * num3 + 2f * num2 + 0.5f * num) + next * (0.5f * num3 - 0.5f * num2);
	}

	private static Vector3 GetPosition(Transform t)
	{
		return t.position;
	}
}
