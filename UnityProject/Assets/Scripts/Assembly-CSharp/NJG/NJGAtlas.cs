using System;
using System.Collections.Generic;
using UnityEngine;

public class NJGAtlas : MonoBehaviour
{
	[Serializable]
	public class Sprite
	{
		public int id;

		public string name;

		public Rect uvs;

		public bool initialized;

		private Vector2 mPos;

		private Vector2 mSize;

		public Vector2 position
		{
			get
			{
				if (mPos == Vector2.zero)
				{
					mPos = new Vector2(uvs.x, uvs.y);
				}
				return mPos;
			}
		}

		public Vector2 size
		{
			get
			{
				if (mSize == Vector2.zero)
				{
					mSize = new Vector2(width, height);
				}
				return mSize;
			}
		}

		public float width
		{
			get
			{
				return uvs.width;
			}
		}

		public float height
		{
			get
			{
				return uvs.height;
			}
		}
	}

	public Shader shader;

	public int size = 2048;

	public int padding = 1;

	public Sprite[] sprites;

	public Texture2D texture;

	public Material spriteMaterial;

	private List<string> mNames = new List<string>();

	public Sprite GetSprite(int id)
	{
		return sprites[id];
	}

	public Sprite GetSprite(string spriteName)
	{
		int i = 0;
		for (int num = sprites.Length; i < num; i++)
		{
			if (sprites[i] != null && !string.IsNullOrEmpty(sprites[i].name) && sprites[i].name == spriteName)
			{
				return sprites[i];
			}
		}
		return null;
	}

	public List<string> GetListOfSprites()
	{
		mNames.Clear();
		int i = 0;
		for (int num = sprites.Length; i < num; i++)
		{
			mNames.Add(sprites[i].name);
		}
		return mNames;
	}

	public List<string> GetListOfSprites(string match)
	{
		if (string.IsNullOrEmpty(match))
		{
			return GetListOfSprites();
		}
		List<string> list = new List<string>();
		int i = 0;
		for (int num = sprites.Length; i < num; i++)
		{
			Sprite sprite = sprites[i];
			if (sprite != null && !string.IsNullOrEmpty(sprite.name) && string.Equals(match, sprite.name, StringComparison.OrdinalIgnoreCase))
			{
				list.Add(sprite.name);
				return list;
			}
		}
		string[] array = match.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		int k = 0;
		for (int num2 = sprites.Length; k < num2; k++)
		{
			Sprite sprite2 = sprites[k];
			if (sprite2 == null || string.IsNullOrEmpty(sprite2.name))
			{
				continue;
			}
			string text = sprite2.name.ToLower();
			int num3 = 0;
			for (int l = 0; l < array.Length; l++)
			{
				if (text.Contains(array[l]))
				{
					num3++;
				}
			}
			if (num3 == array.Length)
			{
				list.Add(sprite2.name);
			}
		}
		return list;
	}

	public void CreateSprite(GameObject go, Rect uvRect, Color color)
	{
		MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
		if (meshFilter == null)
		{
			meshFilter = (MeshFilter)go.AddComponent(typeof(MeshFilter));
		}
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		mesh.Clear();
		MeshRenderer meshRenderer = (MeshRenderer)go.GetComponent(typeof(MeshRenderer));
		if (meshRenderer == null)
		{
			meshRenderer = (MeshRenderer)go.AddComponent(typeof(MeshRenderer));
		}
		meshRenderer.GetComponent<Renderer>().sharedMaterial = spriteMaterial;
		float num = (float)texture.width * 0.5f;
		float num2 = (float)texture.height * 0.5f;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(0f - num, 0f - num2, 0f),
			new Vector3(0f - num, num2, 0f),
			new Vector3(num, num2, 0f),
			new Vector3(num, 0f - num2, 0f)
		};
		int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(uvRect.x, uvRect.y),
			new Vector2(uvRect.x, uvRect.y + uvRect.height),
			new Vector2(uvRect.x + uvRect.width, uvRect.y + uvRect.height),
			new Vector2(uvRect.x + uvRect.width, uvRect.y)
		};
		Color[] colors = new Color[4] { color, color, color, color };
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.uv = uv;
		mesh.triangles = triangles;
		meshFilter.sharedMesh = mesh;
	}

	public void ChangeSprite(Mesh mesh, Rect uvRect)
	{
		mesh.uv = new Vector2[4]
		{
			new Vector2(uvRect.x, uvRect.y),
			new Vector2(uvRect.x, uvRect.y + uvRect.height),
			new Vector2(uvRect.x + uvRect.width, uvRect.y + uvRect.height),
			new Vector2(uvRect.x + uvRect.width, uvRect.y)
		};
	}

	public void ChangeColor(Mesh mesh, Color color)
	{
		mesh.colors = new Color[4] { color, color, color, color };
	}
}
