using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    public bool shouldRender = false;

    public int GridSizeX;
    public int GridSizeY;
    public int GridSizeZ;

    public float LargeStep;

    public float StartX;
    public float StartY;
    public float StartZ;

    private Material _lineMaterial;

    public Color MainColor = new Color(0f, 1f, 0f, 1f);

    void CreateLineMaterial()
    {
        if (!_lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnPostRender()
    {
        CreateLineMaterial();
        // set the current material
        _lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        if (shouldRender)
        {
            GL.Color(MainColor);

            //Layers
            for (float j = 0; j <= GridSizeY; j += LargeStep)
            {
                //X axis lines
                for (float i = 0; i <= GridSizeZ; i += LargeStep)
                {
                    GL.Vertex3(StartX, StartY + j, StartZ + i);
                    GL.Vertex3(StartX + GridSizeX, StartY + j, StartZ + i);
                }

                //Z axis lines
                for (float i = 0; i <= GridSizeX; i += LargeStep)
                {
                    GL.Vertex3(StartX + i, StartY + j, StartZ);
                    GL.Vertex3(StartX + i, StartY + j, StartZ + GridSizeZ);
                }
            }

            //Y axis lines
            for (float i = 0; i <= GridSizeZ; i += LargeStep)
            {
                for (float k = 0; k <= GridSizeX; k += LargeStep)
                {
                    GL.Vertex3(StartX + k, StartY, StartZ + i);
                    GL.Vertex3(StartX + k, StartY + GridSizeY, StartZ + i);
                }
            }
        }

        GL.End();
    }
}
