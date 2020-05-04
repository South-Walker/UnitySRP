using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
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
        Setup();
        DrawVisibleGeometry();
        Submit();
    }
    void Setup()
    {
        //设置摄像机VP矩阵与其他一些性质，
        //在清除之前调用，保证使用到正确的清除方法，而非覆盖的着色器
        context.SetupCameraProperties(camera);
        //清除之前绘制的内容，目标为帧缓存时不影响，目标为特定渲染纹理时可能导致混合
        //另外这个方法被一个profile性能采样包裹，因此并列的写保证采样器合并
        buffer.ClearRenderTarget(true, true, Color.clear);
        //启动profile性能采样
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }
    void Submit()
    {
        //关闭profile性能采样
        buffer.EndSample(bufferName);
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
        context.DrawSkybox(camera);
    }
}
