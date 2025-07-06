using UnityEngine;

public static class DebugUtill
{
    public static Color ColorFromInt(uint hexValue)
    {
        // 0xRRGGBB ���� ����
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

        // ���� ���� ����ȭ
        Vector3 n = normal.normalized;

        // ���� ����: ������ ���� �������� ���� ���� ����
        Vector3 reference = (Mathf.Abs(Vector3.Dot(n, Vector3.up)) > 0.99f) 
            ? (Vector3.forward)
            : (Vector3.up);

        // ũ�ν� ���� (������ ����)
        Vector3 tangent   = Vector3.Cross(n, reference).normalized;
        Vector3 bitangent = Vector3.Cross(n, tangent).normalized;

        // ����
        Debug.DrawRay(position, n * size, hexColor);

        // �¿�(���� ����) ���ڰ�
        Debug.DrawLine(position - tangent   * size, position + tangent   * size, hexColor);
        Debug.DrawLine(position - bitangent * size, position + bitangent * size, hexColor);
    }
}
