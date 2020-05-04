using UnityEngine;
using UnityEngine.Rendering;
//可以通过menu中的Assets/Rendering/Custom Render Pipeline创建对应Asset
[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline();
    }
}
