using UnityEngine;

public class LTRect
{
	public Rect _rect;

	public float alpha;

	public float rotation;

	public Vector2 pivot;

	public bool rotateEnabled;

	public bool rotateFinished;

	public bool alphaEnabled;

	public Rect rect
	{
		get
		{
			if (rotateEnabled)
			{
				if (rotateFinished)
				{
					rotateFinished = false;
					rotateEnabled = false;
					_rect.x += pivot.x;
					_rect.y += pivot.y;
					pivot = Vector2.zero;
					GUI.matrix = Matrix4x4.identity;
				}
				else
				{
					Matrix4x4 identity = Matrix4x4.identity;
					identity.SetTRS(pivot, Quaternion.Euler(0f, 0f, rotation), Vector3.one);
					GUI.matrix = identity;
				}
			}
			else if (alphaEnabled)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			}
			return _rect;
		}
		set
		{
			_rect = value;
		}
	}

	public LTRect(float x, float y, float width, float height)
	{
		_rect = new Rect(x, y, width, height);
		alpha = 1f;
		rotation = 0f;
		rotateEnabled = (alphaEnabled = false);
	}

	public LTRect(float x, float y, float width, float height, float alpha)
	{
		_rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		rotation = 0f;
		rotateEnabled = (alphaEnabled = false);
	}

	public LTRect(float x, float y, float width, float height, float alpha, float rotation)
	{
		_rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		this.rotation = rotation;
		rotateEnabled = (alphaEnabled = false);
		if (rotation != 0f)
		{
			rotateEnabled = true;
			resetForRotation();
		}
	}

	public void resetForRotation()
	{
		if (pivot == Vector2.zero)
		{
			pivot = new Vector2(_rect.x + _rect.width * 0.5f, _rect.y + _rect.height * 0.5f);
			_rect.x += 0f - pivot.x;
			_rect.y += 0f - pivot.y;
		}
	}
}
