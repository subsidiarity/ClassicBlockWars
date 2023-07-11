using System;
using UnityEngine;

public sealed class Storager
{
	public static void createBool(bool wht, string id)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (wht)
			{
				keychainPlugin.createKCValue(1, id);
			}
			else
			{
				keychainPlugin.createKCValue(0, id);
			}
			break;
		case RuntimePlatform.Android:
			if (wht)
			{
				CryptoPlayerPrefs.SetInt(id, 1);
			}
			else
			{
				CryptoPlayerPrefs.SetInt(id, 0);
			}
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	public static void createInt(int val, string id)
	{
		bool flag = false;
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			keychainPlugin.createKCValue(val.ToString(), id);
			break;
		case RuntimePlatform.Android:
			CryptoPlayerPrefs.SetInt(id, val);
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	public static void setBool(bool wht, string id)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (wht)
			{
				keychainPlugin.updateKCValue(1, id);
			}
			else
			{
				keychainPlugin.updateKCValue(0, id);
			}
			break;
		case RuntimePlatform.Android:
			if (wht)
			{
				CryptoPlayerPrefs.SetInt(id, 1);
			}
			else
			{
				CryptoPlayerPrefs.SetInt(id, 0);
			}
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	public static bool getBool(string id)
	{
		bool result = false;
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			result = keychainPlugin.getKCValue(id) == 1;
			break;
		case RuntimePlatform.Android:
			result = Convert.ToBoolean(CryptoPlayerPrefs.GetInt(id));
			break;
		}
		return result;
	}

	public static bool setInt(int val, string id)
	{
		bool result = false;
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (!keychainPlugin.createKCValue(val.ToString(), id))
			{
				keychainPlugin.updateKCValue(val.ToString(), id);
				result = true;
			}
			else
			{
				result = true;
			}
			break;
		case RuntimePlatform.Android:
			CryptoPlayerPrefs.SetInt(id, val);
			result = true;
			break;
		}
		return result;
	}

	public static int getInt(string id)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			return keychainPlugin.getKCValue(id);
		case RuntimePlatform.Android:
			return CryptoPlayerPrefs.GetInt(id);
		default:
			return 0;
		}
	}

	public static void unset(string id)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			keychainPlugin.deleteKCValue(id);
			break;
		case RuntimePlatform.Android:
			CryptoPlayerPrefs.DeleteKey(id);
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}
}
