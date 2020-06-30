using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public class Lighting
{
    const string bufferName = "Lighting";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    CullingResults cullingResults;
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        //SetupDirectionalLight();
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional)
            {
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= maxDirLightCount)
                    break;
            }
        }
        buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionId, dirLightDirections);
    }
    const int maxDirLightCount = 4;

    static int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirections");
    static Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirections = new Vector4[maxDirLightCount];
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visibleLight">ref 装箱，防止复制</param>
    void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        //平行光方向是z轴正方向，符号代表是指向光源的
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);

    }
}
