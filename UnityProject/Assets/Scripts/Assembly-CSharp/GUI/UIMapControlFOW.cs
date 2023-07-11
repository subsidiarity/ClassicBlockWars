using NJG;
using UnityEngine;

public class UIMapControlFOW : MonoBehaviour
{
	public KeyCode enableKey = KeyCode.F;

	public KeyCode resetKey = KeyCode.Z;

	private NJGFOW fow;

	private NJGMapBase map;

	private void Start()
	{
		map = NJGMapBase.instance;
		fow = NJGFOW.instance;
	}

	private void Update()
	{
		if (fow == null || map == null)
		{
			return;
		}
		if (Input.GetKeyDown(enableKey))
		{
			map.fow.enabled = !map.fow.enabled;
			if (map.fow.enabled)
			{
				NJGFOW.instance.Init();
			}
		}
		if (Input.GetKeyDown(resetKey))
		{
			fow.ResetFOW();
		}
	}
}
