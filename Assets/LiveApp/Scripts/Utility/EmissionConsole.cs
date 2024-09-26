using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EmissionConsole : MonoBehaviour
{
    [SerializeField]
    [ColorUsage(false, true)]
    Color HDR_UI;

    [SerializeField]
    Color SDR_Cursor;


    void Update()
    {
#if UNITY_EDITOR
        ApplyShaderColor(HDR_UI, "UI_Under", "Shader Graphs/Sprite_Lit", "_Emission");
        ApplySpriteColor(SDR_Cursor, "Cursor");
#endif
    }

    void ApplyShaderColor(Color color, string targetName, string shaderPass, string propName)
    {
        Renderer[] renderers = GameObject.Find(targetName).GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            if (renderer.sharedMaterial.shader == Shader.Find(shaderPass))
            {
                renderer.sharedMaterial.SetColor(propName, color);
            }
        }
    }

    void ApplySpriteColor(Color color, string targetName)
    {
        SpriteRenderer[] renderers = GameObject.Find(targetName).GetComponentsInChildren<SpriteRenderer>();

        foreach (var renderer in renderers)
        {
            renderer.color = color;
        }
    }
}
