using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Texture3dConverter : EditorWindow
{
    private Texture2D _texture;
    private Vector2Int _rowsAndColumns = new Vector2Int(1, 1);
    private Vector2Int _lastRowsColsValue;
    private int _tileCount;

    private int _resultPixelSize = 32;

    private TextureFormat _resultTextureFormat = TextureFormat.RGBA32;

    [MenuItem("Tools/Texture 3D Converter")]
    private static void ShowWindow()
    {
        var window = GetWindow<Texture3dConverter>();
        window.titleContent = new GUIContent("Texture 3D Converter");
        window.Show();
    }

    private void OnGUI()
    {
        //Input///////////////////

        EditorGUILayout.LabelField("Input Parameters");
        EditorGUILayout.Space(10);

        _texture = (Texture2D)EditorGUILayout.ObjectField("Texture", _texture, typeof(Texture2D), false);

        _rowsAndColumns = EditorGUILayout.Vector2IntField("Rows and Columns", _rowsAndColumns);

        if (_lastRowsColsValue.magnitude != _rowsAndColumns.magnitude)
        {
            _tileCount = _rowsAndColumns.x * _rowsAndColumns.y;
            _lastRowsColsValue = _rowsAndColumns;
        }

        _tileCount = EditorGUILayout.IntField("Tile number", _tileCount);

        if (_tileCount < 0)
        {
            _tileCount = 0;
        }

        //Output///////////////

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Output Parameters");

        EditorGUILayout.Space(10);

        _resultPixelSize = EditorGUILayout.IntField("Pixel Size", _resultPixelSize);

        _resultTextureFormat = (TextureFormat)EditorGUILayout.EnumFlagsField("Texture Format", _resultTextureFormat);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Make Texture3D"))
        {
            if (Mathf.ClosestPowerOfTwo(_resultPixelSize) != _resultPixelSize)
            {
                Debug.LogWarning("Texture 3D Converter: Pixel size should be power of 2!");
            }
            else
            {
                MakeTexture3D();

                Debug.Log("Texture 3D Converter: Done =)");
            }
        }
    }

    private void MakeTexture3D()
    {
        // Configure the texture
        int size = _resultPixelSize;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, _tileCount, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * _tileCount];

        //Prepare Tex2D Data
        int texWidth = size * _rowsAndColumns.y;
        int texHeight = size * _rowsAndColumns.x;

        string path = AssetDatabase.GetAssetPath(_texture);
        string tempPath = path.Substring(0, path.LastIndexOf('/')) + "/temp_resize." + path.Split('.')[path.Split('.').Length - 1];

        AssetDatabase.CopyAsset(path, tempPath);
        MakeTextureReadable(tempPath);

        Texture2D resizedTex = (Texture2D)AssetDatabase.LoadAssetAtPath(tempPath, typeof(Texture2D));
        resizedTex = Resize(resizedTex, texWidth, texHeight);

        Color[] pixelData = resizedTex.GetPixels(0, 0, resizedTex.width, resizedTex.height);

        // Populate the color array
        int currentRow = _rowsAndColumns.y - 1, currentColumn = 0;
        int pixelIndex = 0;

        for (int z = 0; z < _tileCount; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    pixelIndex = currentColumn * size + currentRow * size * texWidth + y * texWidth + x;

                    colors[zOffset + yOffset + x] = pixelData[pixelIndex];
                }
            }

            currentColumn++;

            if (currentColumn >= _rowsAndColumns.x)
            {
                currentColumn = 0;
                currentRow--;
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, path.Substring(0, path.IndexOf('.')) + ".asset");
        AssetDatabase.DeleteAsset(tempPath);
    }

    private void MakeTextureReadable(string tempPath)
    {
        var tImporter = AssetImporter.GetAtPath(tempPath) as TextureImporter;
        if (tImporter != null)
        {
            tImporter.isReadable = true;

            AssetDatabase.ImportAsset(tempPath);
            AssetDatabase.Refresh();
        }
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
}