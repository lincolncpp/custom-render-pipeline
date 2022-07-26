using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline {

	CameraRenderer renderer = new CameraRenderer();

	public CustomRenderPipeline() {
		GraphicsSettings.useScriptableRenderPipelineBatching = true;
	}

	protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
		for(int i = 0;i < cameras.Length; i++) {
			renderer.Render(context, cameras[i]);
		}
	}
}
