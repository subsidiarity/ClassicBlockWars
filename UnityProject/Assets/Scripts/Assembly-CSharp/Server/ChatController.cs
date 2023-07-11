using UnityEngine;

public class ChatController : MonoBehaviour
{
	public static ChatController thisScript;

	// TMP private TouchScreenKeyboard mKeyboard;

	private string mText = string.Empty;

	public int maxChars = 40;

	public bool isHold = true;

	public AudioClip sendChatClip;

	public UILabel poleVvoda;

	public GameObject holdButton;

	public GameObject holdButtonOn;

	private void Awake()
	{
		thisScript = this;
	}

	private void Update()
	{
		/*if (mKeyboard == null)
		{
			return;
		}*/
		return; // TMP
		/*string text = mKeyboard.text;
		if (mText != text)
		{
			mText = string.Empty;
			foreach (char c in text)
			{
				if (c != 0)
				{
					mText += c;
				}
			}
			if (maxChars > 0 && mKeyboard.text.Length > maxChars)
			{
				mKeyboard.text = mKeyboard.text.Substring(0, maxChars);
			}
			if (mText != text)
			{
				mKeyboard.text = mText;
			}
		}
		poleVvoda.text = mText;
		if (mKeyboard.done && !mKeyboard.wasCanceled)
		{
			if (!poleVvoda.text.Equals(string.Empty))
			{
				postMessageToChat();
			}
			Debug.Log("pressDone " + mText);
			if (isHold)
			{
				mKeyboard.active = true;
			}
			else
			{
				closeChat();
			}
		}
		else if (mKeyboard.wasCanceled && !isHold)
		{
			closeChat();
		}
		if (mKeyboard != null && !mKeyboard.active)
		{
			closeChat();
		}*/
	}

	public void postMessageToChat()
	{
		Debug.Log("post to chat:" + settings.tekName + ":" + poleVvoda.text);
		if (settings.soundEnabled)
		{
			NGUITools.PlaySound(sendChatClip);
		}
		GameController.thisScript.addMessageToListOnline(settings.tekName + ": " + poleVvoda.text);
	}

	public void createChat()
	{
		if (poleVvoda != null)
		{
			poleVvoda.text = string.Empty;
		}
		base.enabled = true;
		// TMP mKeyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false);
	}

	public void closeChat()
	{
		Debug.Log("closeChat");
		/*if (mKeyboard != null)
		{
			mKeyboard.active = false;
			mKeyboard = null;
		}
		GameController.thisScript.showWindowGame();
		base.enabled = false;*/ // TMP
	}

	private void OnDestroy()
	{
		closeChat();
	}
}
