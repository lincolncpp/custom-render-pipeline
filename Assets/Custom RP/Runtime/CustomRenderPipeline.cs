using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline {

	CameraRenderer renderer = new CameraRenderer();
	bool useDynamicBatching;
	bool useGPUInstancing;

	public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher) {
		this.useDynamicBatching = useDynamicBatching;
		this.useGPUInstancing = useGPUInstancing;
		GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
	}

	protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
		for(int i = 0;i < cameras.Length; i++) {
			renderer.Render(context, cameras[i], useDynamicBatching, useGPUInstancing);
		}
	}
}
