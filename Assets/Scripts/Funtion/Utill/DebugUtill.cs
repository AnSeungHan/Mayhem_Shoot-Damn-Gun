using UnityEngine;

public static class DebugUtill
{
    public static Color ColorFromInt(uint hexValue)
    {
        // 0xRRGGBB 형식 가정
        float r = ((hexValue >> 16) & 0xFF) / 255f;
        float g = ((hexValue >> 8) & 0xFF) / 255f;
        float b = (hexValue & 0xFF) / 255f;

        return new Color(r, g, b, 1f);
    }

    public static void DrawLine(Vector3 start, Vector3 end, uint color)
    {
        Debug.DrawLine(start, end, ColorFromInt(color));
    }

    public static void DrawPoint(Vector3 position, uint color)
    {
        Color hexColor = ColorFromInt(color);
        float size = 0.1f;

        Debug.DrawLine(position + Vector3.left * size, position + Vector3.right * size, hexColor);
        Debug.DrawLine(position + Vector3.up   * size, position + Vector3.down  * size, hexColor);
    }
}
