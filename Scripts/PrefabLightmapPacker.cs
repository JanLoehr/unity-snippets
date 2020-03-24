using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

public class PrefabLightmapPacker : MonoBehaviour
{
    [MenuItem("Tools/Custom/Prefab Lightmap Packer")]
    public static void PackLightmap()
    {
        List<Renderer> renderers = new List<Renderer>();

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            renderers.AddRange(Selection.gameObjects[i].GetComponentsInChildren<Renderer>());
        }

        foreach (var renderer in renderers)
        {
            Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            Vector4 lightmapOffsetAndScale = renderer.lightmapScaleOffset;

            Vector2[] modifiedUV2s = mesh.uv2;
            for (int i = 0; i < mesh.uv2.Length; i++)
            {
                modifiedUV2s[i] = new Vector2(mesh.uv2[i].x * lightmapOffsetAndScale.x +
                lightmapOffsetAndScale.z, mesh.uv2[i].y * lightmapOffsetAndScale.y +
                lightmapOffsetAndScale.w);
            }
            mesh.uv3 = modifiedUV2s;

            mesh.UploadMeshData(true);
        }
    }
}

#endif
