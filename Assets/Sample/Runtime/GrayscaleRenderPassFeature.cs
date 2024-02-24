using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Utility;

public class GrayscaleRenderPassFeature : ScriptableRendererFeature
{
    private class GrayscaleRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private Grayscale _grayscale;

        private readonly ProfilingSampler _profilingSampler = new("Grayscale");

        public bool Setup(Shader shader)
        {
            if (_material == null)
            {
                if (shader == null)
                {
                    Debug.LogWarning("Could not load Grayscale shader. Please make sure Grayscale.shader is present.");
                    return false;
                }

                _material = CoreUtils.CreateEngineMaterial(shader);
            }
            return true;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
                return;

            // VolumeManager를 통해 현재 스택을 가져옵니다.
            // Grayscale 컴포넌트를 가져와서 _grayscale에 할당합니다.
            // _grayscale의 intensity 값을 _material에 전달합니다.
            VolumeStack stack = VolumeManager.instance.stack;
            _grayscale = stack.GetComponent<Grayscale>();
            _material.SetFloat(ShaderConstants.Intensity, _grayscale.Intensity.value);

            // CommandBuffer를 가져옵니다.
            // 렌더 대상을 _material을 사용하여 그레이스케일로 렌더링합니다.
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                Blit(cmd, ref renderingData, _material);
            }

            // CommandBuffer를 실행합니다.
            // CommandBuffer를 비웁니다. (재사용을 위해)
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void Cleanup()
        {
            CoreUtils.Destroy(_material);
        }

        private static class ShaderConstants
        {
            internal static readonly int Intensity = Shader.PropertyToID("_Intensity");
        }
    }

    private Shader _shader;
    private GrayscaleRenderPass _grayscaleRenderPass;
    private UniversalRendererData _universalRendererData;

    public override void Create()
    {
        name = "Grayscale";
        _grayscaleRenderPass = new GrayscaleRenderPass();
        _grayscaleRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        _shader = Shader.Find("Hidden/Universal Render Pipeline/PostProcess/Grayscale");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!_universalRendererData)
            _universalRendererData = URPRendererUtility.GetUniversalRendererData();

        if (!URPRendererUtility.IsPostProcessEnabled(_universalRendererData, ref renderingData))
            return;

        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        if (_grayscaleRenderPass.Setup(_shader))
            renderer.EnqueuePass(_grayscaleRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _grayscaleRenderPass?.Cleanup();
        }
    }
}
