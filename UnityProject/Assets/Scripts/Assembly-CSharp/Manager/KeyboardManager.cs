using UnityEngine;

public class KeyboardManager
{
    public static float weaponSwitchLastTime;
    public static float carEnterExitLastTime;

    public static void Update(PlayerBehavior playerScript)
    {
        if (!CompilationSettings.UseKeyboard)
        {
            return;
        }

        if (Input.GetButton("Enter Exit Car")
        && CompilationSettings.KeyboardCarEnterExitDelay + carEnterExitLastTime < Time.time
        && ((GameController.thisScript.carScript != null
        && GameController.thisScript.butOutCar.active)
        || GameController.thisScript.butInCar.active))
        {
            carEnterExitLastTime = Time.time;
            playerScript.TryGetIntoOrOutVehicle();
        }

        if (Input.GetKeyDown(KeyCode.Q)
        && CompilationSettings.KeyboardWeaponSwitchDelay
        + weaponSwitchLastTime < Time.time)
        {
            weaponSwitchLastTime = Time.time;
            playerScript.weaponManager.switchToNext();
        }
    }

    public static Vector2 GetMovmentVector()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        x = Mathf.Clamp(x + Input.GetAxis("Horizontal (Keyboard)"), -1.0f, 1.0f);
        y = Mathf.Clamp(y + Input.GetAxis("Vertical (Keyboard)"), -1.0f, 1.0f);

        return new Vector2(x, y);
    }
}