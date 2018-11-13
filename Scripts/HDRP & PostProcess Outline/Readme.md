# HRDP & PostProcess Outline

Developed and tested with Unity 2018.3.0b9, HDRP 4.1.0, PostProcessing 2.0.17

1. Create a RenderTexture somewhere in your Assets
2. Create a GameObject with the OutlineController.cs Script and assign the RenderTexture
3. Add the HoverableObject.cs to each object you want to be hoverable and assign the RenderTexture. (You have to implement the IHoverable interface yourself, this is not in scope of this snippet)
4. Add the Custom Effect to the PostProcessing Volume of your choice and link the RenderTexture
5. Pray ;)