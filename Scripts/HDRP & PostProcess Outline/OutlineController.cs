using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public RenderTexture RenderTexture;

    // Update is called once per frame
    void Update()
    {
        Graphics.SetRenderTarget(RenderTexture);
        GL.Clear(true, true, Color.black);
    }
}