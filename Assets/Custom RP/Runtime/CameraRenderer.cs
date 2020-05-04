using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    
    //无光照着色器
    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    ScriptableRenderContext context;
    Camera camera;
    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;
        PrepaerBuffer();
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }
    void Setup()
    {
        //设置摄像机VP矩阵与其他一些性质，
        //在清除之前调用，保证使用到正确的清除方法，而非覆盖的着色器
        context.SetupCameraProperties(camera);
        //clearFlags:
        //1:Skybox 2:Color 3:Depth 4:Nothing
        CameraClearFlags flags = camera.clearFlags;
        //清除之前绘制的内容，目标为帧缓存时不影响，目标为特定渲染纹理时可能导致混合
        //另外这个方法被一个profile性能采样包裹，因此并列的写保证采样器合并
        buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color?
                camera.backgroundColor.linear : Color.clear);
        //启动profile性能采样
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }
    void Submit()
    {
        //关闭profile性能采样
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        //draw命令在Submit前只被缓存
        context.Submit();
    }
    void ExecuteBuffer()
    {
        //执行本身不会清除缓冲区
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    void DrawVisibleGeometry()
    {
        //opaque => skybox => transparent
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(
            unlitShaderTagId, sortingSettings
            );
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
            );
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
            );
    }
    CullingResults cullingResults;
    bool Cull()
    {
        //在裁剪前先获取裁剪参数
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) 
        {
            //p可能是个很大的结构，写ref防止复制
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }
}
