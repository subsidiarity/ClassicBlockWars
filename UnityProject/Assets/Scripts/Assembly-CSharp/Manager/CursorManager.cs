using UnityEngine;

public class CursorManager
{
    public static void UpdateMouseLock()
    {
        if (!CompilationSettings.MouseLock)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.lockCursor = false;
        }
        // TODO: This should have some kind of check if the player is clicking
        // a menu of something.
        else if (Input.GetMouseButtonUp(0))
        {
            Screen.lockCursor = true;
        }
    }

    public static Vector2 GetCameraDelta()
    {
        UpdateMouseLock();

        /* If the cursor is not locked no movment to the camera should be done. */
        if (!Screen.lockCursor)
        {
            return new Vector2(0, 0);
        }

        return new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
    }
}