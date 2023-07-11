using UnityEngine;

public class FlurryWrapper : MonoBehaviour
{
	private static bool isEnabled = true;

	public static string EV_POST_TO_FACEBOOK = "Post to FB";

	public static string EV_POST_TO_TWITTER = "Post to TW";

	public static string EV_OPEN_SHOP_M = "Open Shop_Menu";

	public static string EV_OPEN_SHOP_G = "Open Shop_Game";

	public static string EV_LAUNCH_FREEPLAY = "Freeplay";

	public static string EV_LAUNCH_ONLINE = "Launch Online";

	public static string EV_LAUNCH_HERO_ROOM = "Open Hero Room";

	public static string EV_BUY_SKIN = "Buy Skin ";

	public static string EV_LAUNCH_MISSION = "Launch Mission ";

	public static string EV_BUY_ITEM = "Buy ";

	public static string EV_LAUNCH_QUICKGAME = "Launch Quickgame";

	public static string EV_LAUNCH_CREATEGAME = "Launch CreateGame";

	public static string EV_LAUNCH_ROOM = "Launch Room";

	public static void LogEvent(string e)
	{
	}
}
