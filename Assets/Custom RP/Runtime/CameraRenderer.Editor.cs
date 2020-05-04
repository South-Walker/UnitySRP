using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    partial void DrawUnsupportedShaders();
    partial void DrawGizmos();
    partial void PrepareForSceneWindow();
    partial void PrepaerBuffer();
#if UNITY_EDITOR
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    static Material errorMaterial;
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }
    string SampleName { get; set; }
    partial void DrawUnsupportedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial =
                new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var drawingSettings = new DrawingSettings(
            legacyShaderTagIds[0], new SortingSettings(camera)
        )
        {
            overrideMaterial = errorMaterial
        };
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
        );
    }
    partial void PrepareForSceneWindow()
    {
        if (camera.cameraType == CameraType.SceneView) 
        {
            //在Scene视图中不省略UI界面
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
    partial void PrepaerBuffer()
    {
        //防止command buffer同名合并
        buffer.name = SampleName = camera.name;
    }
#else
    const string SampleName = bufferName;
#endif
}
