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

    public static void DrawPoint_Normal(Vector3 position, Vector3 normal, uint color)
    {
        Color hexColor = ColorFromInt(color);
        float size = 0.1f;

        // 법선 벡터 정규화
        Vector3 n = normal.normalized;

        // 기준 벡터: 법선과 거의 평행하지 않은 방향 선택
        Vector3 reference = (Mathf.Abs(Vector3.Dot(n, Vector3.up)) > 0.99f) 
            ? (Vector3.forward)
            : (Vector3.up);

        // 크로스 벡터 (법선에 수직)
        Vector3 tangent   = Vector3.Cross(n, reference).normalized;
        Vector3 bitangent = Vector3.Cross(n, tangent).normalized;

        // 법선
        Debug.DrawRay(position, n * size, hexColor);

        // 좌우(수직 방향) 십자가
        Debug.DrawLine(position - tangent   * size, position + tangent   * size, hexColor);
        Debug.DrawLine(position - bitangent * size, position + bitangent * size, hexColor);
    }
}
