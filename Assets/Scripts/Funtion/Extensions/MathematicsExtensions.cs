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

    public static float3 normalize(this float3 vec)
    {
        return math.normalize(vec);
    }

    public struct Float2
    { 
        public static readonly float2 up        = new float2( 0,  1);
        public static readonly float2 down      = new float2( 0, -1);
        public static readonly float2 left      = new float2(-1,  0);
        public static readonly float2 right     = new float2( 1,  0);
    }

    public static float2 normalize(this float2 vec)
    {
        return math.normalize(vec);
    }

    public struct HexColor
    {
        public static readonly uint red         = 0xFF0000;
        public static readonly uint green       = 0x00FF00;
    }
}
