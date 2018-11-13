using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HoverOutlineRenderer), PostProcessEvent.AfterStack, "Custom/OutlineRenderer")]
public sealed class HoverOutlinePostEffect : PostProcessEffectSettings
{
    [Tooltip("The Color of the Lines to draw")]
    public ColorParameter OutlineColor = new ColorParameter { value = Color.red };

    public FloatParameter OutlineThicknessY = new FloatParameter { value = 0.0017f };
    public FloatParameter OutlineThicknessX = new FloatParameter { value = 0.001f };
    public FloatParameter OutlineStrength = new FloatParameter { value = 50 };

    public TextureParameter OutlineTexture = new TextureParameter { value = null };
}

public sealed class HoverOutlineRenderer : PostProcessEffectRenderer<HoverOutlinePostEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/OutlineShader"));

        sheet.properties.SetColor("_OutlineColor", settings.OutlineColor);
        sheet.properties.SetFloat("_OutlineThicknessY", settings.OutlineThicknessY);
        sheet.properties.SetFloat("_OutlineThicknessX", settings.OutlineThicknessX);
        sheet.properties.SetFloat("_OutlineStrength", settings.OutlineStrength);

        if (settings.OutlineTexture != null)
        {
            sheet.properties.SetTexture("_OutlineTex", settings.OutlineTexture);
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
