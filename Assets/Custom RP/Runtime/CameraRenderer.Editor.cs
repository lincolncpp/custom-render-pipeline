using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer {

	partial void PrepareBuffer();
	partial void PrepareForSceneWindow();
	partial void DrawGizmos();
	partial void DrawUnsupportedShaders();

#if UNITY_EDITOR

	private string sampleName { set; get; }

	private static ShaderTagId[] legacyShaderTagIds = {
		new ShaderTagId("Always"),
		new ShaderTagId("ForwardBase"),
		new ShaderTagId("PrepassBase"),
		new ShaderTagId("Vertex"),
		new ShaderTagId("VertexLMRGBM"),
		new ShaderTagId("VertexLM")
	};

	private static Material errorMaterial;

	partial void PrepareBuffer() {
		Profiler.BeginSample("Editor Only");
		buffer.name = sampleName = camera.name;
		Profiler.EndSample();
	}

	partial void PrepareForSceneWindow() {
		if (camera.cameraType == CameraType.SceneView) {
			ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
		}
	}

	partial void DrawGizmos() {
		if(Handles.ShouldRenderGizmos()) {
			context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
			context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
		}
	}

	partial void DrawUnsupportedShaders() {
		if (errorMaterial == null){
			errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
		}


		DrawingSettings drawingSettings = new DrawingSettings(
			legacyShaderTagIds[0],
			new SortingSettings(camera)
		) {
			overrideMaterial = errorMaterial
		};

		for(int i = 1;i < legacyShaderTagIds.Length; i++) {
			drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
		}

		FilteringSettings filteringSettings = FilteringSettings.defaultValue;
		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	}

#else

	private const string sampleName = bufferName;

#endif

}
