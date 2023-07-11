using System;

public class CompilationSettings
{
    // Removes the bad word filter from game rooms.
    public const bool NoBadWordFilter = true;

    public const bool UseKeyboard = true;

    // Clicking on the screen in game will lock the mouse to the screen. Escape can be used to do
    // unlock the mouse.
    public const bool MouseLock = true;

    public const bool CarUseKeyboard = true;
    public const bool CarUseMouse = true;

    // Clicking the screen will shoot the mouse.
    public const bool UseMouseShoot = true;

    // These will overide all sector loading distances with a custom amount.
    public const bool OverideSectorLoads = true;
    public const int SectorLoadDistance = 10000;
    public const int SectorUnloadDistance = 10005;

    // This will be used to automatically set graphics settings in game.
    public const bool DeviceIsWeakOveride = false;

    public const bool EdgeBugFix = true;

    // TODO: Comment
    public const bool UseMotionMovment = true;

    // TODO: Name this better
    // The game automatically sets the graphics level to low but this will stop the game from
    // overiding the graphical settings.
    public const bool GraphicsOverideDisabled = true;

    public const float KeyboardWeaponSwitchDelay = 0.15f;
    public const float KeyboardCarEnterExitDelay = 0.15f;

    // Default is 3.6f.
    public const float CarTurningSensitivity = 3.6f;

    // Default is 1.0f.
    public const float CarTurningMaximumSpeed = 0.95f;

    // Default is 1.0f.
    public const float CarTurningSpeedMultiplier = 1.125f;

    // Default is 10000.
    public const int StartingCash = 2147483647;

    // Goes from 0.0f to 1.0f.
    public const float PlayerStickMaxRotation = 0.2f;

    public const float PlayerStickMaxDeltaDistance = 15.0f;

    public const bool UseBetterCarExplosions = true;
}