public class Device
{
	public static bool isWeakDevice
	{
		get
		{
			return CompilationSettings.DeviceIsWeakOveride;
		}
	}
}
