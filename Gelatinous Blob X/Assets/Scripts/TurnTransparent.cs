using UnityEngine;
using System.Collections.Generic;

public class TurnTransparent : MonoBehaviour {

    private MeshRenderer mr;
    private List<int> matIndex;
    private Color[] originalColors;

	public void SetTransparent(bool isTransparent)
    {
        if(mr == null)
        {
            mr = GetComponent<MeshRenderer>();
            matIndex = new List<int>();
            originalColors = new Color[mr.materials.Length];
            for (int i = 0; i < mr.materials.Length; ++i)
            {
                if (!IsRenderingModeTransparent(mr.materials[i]))
                {
                    matIndex.Add(i);
                    originalColors[i] = mr.materials[i].color;
                }
            }
        }

        if(isTransparent)
        {
            foreach (int i in matIndex)
            {
                SetRenderingModeToTransparent(mr.materials[i]);
                mr.materials[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0.5f);
            }
        }
        else
        {
            foreach (int i in matIndex)
            {
                SetRenderingModeToOpaque(mr.materials[i]);
                mr.materials[i].color = originalColors[i];
            }
        }
    }

    public static bool IsRenderingModeTransparent(Material material)
    {
        return material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON");
    }

    public static void SetRenderingModeToTransparent(Material material)
    {
    	material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
    }

    public static void SetRenderingModeToOpaque(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }
}
