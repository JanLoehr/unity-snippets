public static class TransformExtensions
{
    public static bool IsOnScreen(this Transform transform)
    {
        //Answer only if the cursor is actually on this client's screen
        Vector3 cursorScreenSpace = GetMainCam().WorldToScreenPoint(transform.position);

        if (cursorScreenSpace.z > 0
            && cursorScreenSpace.x > 0 && cursorScreenSpace.x < Screen.currentResolution.width
            && cursorScreenSpace.y > 0 && cursorScreenSpace.y < Screen.currentResolution.height)
        {
            return true;
        }

        return false;
    }

    private static Camera GetMainCam()
    {
        return Camera.main;
    }
}
