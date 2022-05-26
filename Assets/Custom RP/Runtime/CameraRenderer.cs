using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer {

	private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

	private ScriptableRenderContext context;
	private Camera camera;

	private const string bufferName = "Render Camera";
	private CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};

	private CullingResults cullingResults;

	public void Render(ScriptableRenderContext context, Camera camera) {
		this.context = context;
		this.camera = camera;

		if(!Cull()) {
			return;
		}

		Setup();
		DrawVisibleGeometry();
		Submit();
	}

	private void Setup() {
		context.SetupCameraProperties(camera);
		buffer.ClearRenderTarget(true, true, Color.clear);
		buffer.BeginSample(bufferName);
		ExecuteBuffer();
	}

	private void DrawVisibleGeometry() {
		// Drawing opaques
		SortingSettings sortingSettings = new SortingSettings(camera) { 
			criteria = SortingCriteria.CommonOpaque
		};
		DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
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
		buffer.EndSample(bufferName);
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
