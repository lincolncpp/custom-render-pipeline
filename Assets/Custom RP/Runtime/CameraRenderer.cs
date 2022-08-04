using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {

	private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");


	private ScriptableRenderContext context;
	private Camera camera;

	private const string bufferName = "Render Camera";
	private CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};

	private CullingResults cullingResults;

	public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing) {
		this.context = context;
		this.camera = camera;

		PrepareBuffer();
		PrepareForSceneWindow();
		if(!Cull()) {
			return;
		}

		Setup();
		DrawUnsupportedShaders();
		DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
		DrawGizmos();
		Submit();
	}

	private void Setup() {
		context.SetupCameraProperties(camera);
		CameraClearFlags flags = camera.clearFlags;
		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth, 
			flags <= CameraClearFlags.Color, 
			flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear
		);
		buffer.BeginSample(sampleName);
		ExecuteBuffer();
	}

	private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing) {
		// Drawing opaques
		SortingSettings sortingSettings = new SortingSettings(camera) { 
			criteria = SortingCriteria.CommonOpaque
		};
		DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings) {
			enableDynamicBatching = useDynamicBatching,
			enableInstancing = useGPUInstancing
		};
		FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
		context.DrawSkybox(camera);

		// Drawing transparent
		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	}

	private void Submit() {
		buffer.EndSample(sampleName);
		ExecuteBuffer();
		context.Submit();
	}

	private void ExecuteBuffer() {
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}

	private bool Cull() {
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			cullingResults = context.Cull(ref p);

			return true;
		}
		return false;
	}
}
