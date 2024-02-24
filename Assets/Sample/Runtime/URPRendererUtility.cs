using System.Reflection;

namespace UnityEngine.Rendering.Universal.Utility
{
    public static class URPRendererUtility
    {
        /// <summary>
        /// 현재 설정된 렌더 파이프라인 에셋을 가져옵니다.
        /// </summary>
        /// <returns>설정된 렌더 파이프라인 에셋을 반환합니다. 에셋이 없는 경우 null을 반환합니다.</returns>
        public static RenderPipelineAsset GetRenderPipelineAsset()
        {
            return GraphicsSettings.renderPipelineAsset;
        }

        /// <summary>
        /// 현재 설정된 렌더 파이프라인 에셋의 스크립트 가능한 렌더러 데이터를 배열로 가져옵니다.
        /// </summary>
        /// <returns>스크립트 가능한 렌더러 데이터의 배열을 반환합니다. 에셋이 설정되어 있지 않거나, 렌더러 데이터 배열이 비어있는 경우 null을 반환합니다.</returns>
        public static ScriptableRendererData[] GetScriptableRendererData()
        {
            RenderPipelineAsset pipelineAsset = GetRenderPipelineAsset();
            if (!pipelineAsset) return null;

            FieldInfo propertyInfo = pipelineAsset.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            ScriptableRendererData[] scriptableRendererData = propertyInfo.GetValue(pipelineAsset) as ScriptableRendererData[];
            return scriptableRendererData;
        }

        /// <summary>
        /// 현재 설정된 렌더 파이프라인 에셋에서 주어진 rendererListIndex의 UniversalRendererData를 가져옵니다.
        /// </summary>
        /// <param name="rendererListIndex">UniversalRendererData를 검색할 렌더러 목록의 인덱스입니다. 기본값은 0입니다.</param>
        /// <returns>주어진 rendererListIndex 의 UniversalRendererData 가 반환됩니다. 에셋이 설정되어있지 않거나, 렌더러 데이터 배열이 비어있거나, 주어진 rendererListIndex가 범위를 벗어날 경우 null을 반환합니다.</returns>
        public static UniversalRendererData GetUniversalRendererData(int rendererListIndex = 0)
        {
            ScriptableRendererData[] scriptableRendererData = GetScriptableRendererData();
            if (scriptableRendererData == null || scriptableRendererData.Length <= 0) return null;

            UniversalRendererData universalRendererData =
                scriptableRendererData[rendererListIndex] as UniversalRendererData;
            return universalRendererData;
        }

        /// <summary>
        /// 포스트 프로세스 옵션이 활성화되어 있는지 확인합니다.
        /// </summary>
        /// <param name="universalRendererData">포스트 프로세스 데이터가 포함된 UniversalRendererData입니다.</param>
        /// <param name="renderingData">현재 프레임의 RenderingData입니다.</param>
        /// <returns>포스트 프로세스가 활성화되어 있으면 True이고, 그렇지 않으면 False입니다.</returns>
        public static bool IsPostProcessEnabled(UniversalRendererData universalRendererData, ref RenderingData renderingData)
        {
            // RendererData 에셋의 Post-processing 활성화 체크
            if (!universalRendererData ||
                !universalRendererData.postProcessData) return false;

            // 카메라 오브젝트의 Post Processing 활성화 체크
            if (!renderingData.cameraData.postProcessEnabled)
                return false;

            return true;
        }
    }
}