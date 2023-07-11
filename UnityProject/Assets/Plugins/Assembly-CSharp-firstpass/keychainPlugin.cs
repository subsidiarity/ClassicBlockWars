using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class keychainPlugin
{
	[DllImport("__Internal")]
	private static extern bool createKeychainValue(string pass, string iden);

	[DllImport("__Internal")]
	private static extern bool updateKeychainValue(string pass, string iden);

	[DllImport("__Internal")]
	private static extern void deleteKeychainValue(string iden);

	[DllImport("__Internal")]
	private static extern string getKeychainValue(string iden);

	public static int getKCValue(string id)
	{
		int num = 0;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (getKeychainValue(id) != null)
			{
				return int.Parse(getKeychainValue(id));
			}
			return 0;
		}
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			return 0;
		}
		throw new NotSupportedException(Application.platform.ToString());
	}

	public static bool createKCValue(int val, string id)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = createKeychainValue(val.ToString(), id);
		}
		else if (Application.platform != 0)
		{
			throw new NotSupportedException(Application.platform.ToString());
		}
		return result;
	}

	public static bool createKCValue(string val, string id)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = createKeychainValue(val, id);
		}
		else if (Application.platform != 0)
		{
			throw new NotSupportedException(Application.platform.ToString());
		}
		return result;
	}

	public static bool updateKCValue(int val, string id)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = updateKeychainValue(val.ToString(), id);
		}
		else if (Application.platform != 0)
		{
			throw new NotSupportedException(Application.platform.ToString());
		}
		return result;
	}

	public static bool updateKCValue(string val, string id)
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = updateKeychainValue(val, id);
		}
		else if (Application.platform != 0)
		{
			throw new NotSupportedException(Application.platform.ToString());
		}
		return result;
	}

	public static void deleteKCValue(string id)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			deleteKeychainValue(id);
		}
		else if (Application.platform != 0)
		{
			throw new NotSupportedException(Application.platform.ToString());
		}
	}
}
