using System;
using System.IO;
using UnityEngine;

public class SkinsManager
{
	public static string _PathBase
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	private static void _WriteImageAtPathToGal(string pathToImage)
	{
		try
		{
		}
		catch (Exception ex)
		{
			Debug.Log("Exception in _ScreenshotWriteToAlbum: " + ex);
		}
	}

	public static void SaveTextureToGallery(Texture2D t, string nm)
	{
		string pathToImage = Path.Combine(_PathBase, nm);
		_WriteImageAtPathToGal(pathToImage);
	}

	public static bool SaveTextureWithName(Texture2D t, string nm, bool writeToGallery = true)
	{
		string text = Path.Combine(_PathBase, nm);
		try
		{
			byte[] bytes = t.EncodeToPNG();
			File.WriteAllBytes(text, bytes);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		if (writeToGallery)
		{
			_WriteImageAtPathToGal(text);
		}
		return true;
	}

	public static Texture2D TextureForName(string nm)
	{
		Texture2D texture2D = new Texture2D(64, 32);
		try
		{
			byte[] data = File.ReadAllBytes(Path.Combine(_PathBase, nm));
			texture2D.LoadImage(data);
			return texture2D;
		}
		catch (Exception message)
		{
			Debug.Log(message);
			return texture2D;
		}
	}

	public static bool DeleteTexture(string nm)
	{
		try
		{
			File.Delete(Path.Combine(_PathBase, nm));
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		return true;
	}
}
