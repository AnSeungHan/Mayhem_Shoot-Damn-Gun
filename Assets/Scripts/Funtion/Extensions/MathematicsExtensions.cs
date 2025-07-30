using Unity.Mathematics;

public static class MathematicsExtensions
{
    public struct Float3
    { 
        public static readonly float3 up        = new float3( 0,  1,  0);
        public static readonly float3 down      = new float3( 0, -1,  0);
        public static readonly float3 left      = new float3(-1,  0,  0);
        public static readonly float3 right     = new float3( 1,  0,  0);
        public static readonly float3 forward   = new float3( 0,  0,  1);
        public static readonly float3 backward  = new float3( 0,  0, -1);

        public static readonly float3[] directions = new float3[]
        {
            Float3.forward,                     // forward
            new float3(1, 0, 1).normalize(),    // forward-right
            Float3.right,                       // right
            new float3(1, 0, -1).normalize(),   // back-right
            Float3.backward,                    // back
            new float3(-1, 0, -1).normalize(),  // back-left
            Float3.left,                        // left
            new float3(-1, 0, 1).normalize(),   // forward-left
        };
    }

    public static float3 normalize(this float3 vec3)
    {
        return math.normalize(vec3);
    }

    public struct Float2
    { 
        public static readonly float2 up        = new float2( 0,  1);
        public static readonly float2 down      = new float2( 0, -1);
        public static readonly float2 left      = new float2(-1,  0);
        public static readonly float2 right     = new float2( 1,  0);
    }

    public static float2 normalize(this float2 vec2)
    {
        return math.normalize(vec2);
    }

    public static bool IsZero(this float2 vec2)
    {
        return vec2.x == 0f && vec2.y == 0f;
    }

    public static float3 ToFloat3_XZ(this float2 vec2)
    {
        return new float3(vec2.x, 0f, vec2.y);
    }

    public struct HexColor
    {
        public static readonly uint red         = 0xFF0000;
        public static readonly uint green       = 0x00FF00;
        public static readonly uint blue        = 0x0000FF;
        public static readonly uint yellow      = 0xFFFF00;
    }
}
