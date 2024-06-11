using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Lis.Core.Utilities
{
    [ExecuteAlways]
    public class AutoLoadPipelineAsset : MonoBehaviour
    {
        [SerializeField] UniversalRenderPipelineAsset m_PipelineAsset;

        bool m_overrodeQualitySettings;
        RenderPipelineAsset m_PreviousPipelineAsset;

        void OnEnable()
        {
            UpdatePipeline();
        }

        void OnDisable()
        {
            ResetPipeline();
        }

        void UpdatePipeline()
        {
            if (m_PipelineAsset)
            {
                if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline != m_PipelineAsset)
                {
                    m_PreviousPipelineAsset = QualitySettings.renderPipeline;
                    QualitySettings.renderPipeline = m_PipelineAsset;
                    m_overrodeQualitySettings = true;
                }
                else if (GraphicsSettings.renderPipelineAsset != m_PipelineAsset)
                {
                    m_PreviousPipelineAsset = GraphicsSettings.renderPipelineAsset;
                    GraphicsSettings.renderPipelineAsset = m_PipelineAsset;
                    m_overrodeQualitySettings = false;
                }
            }
        }

        void ResetPipeline()
        {
            if (m_PreviousPipelineAsset)
            {
                if (m_overrodeQualitySettings)
                    QualitySettings.renderPipeline = m_PreviousPipelineAsset;
                else
                    GraphicsSettings.renderPipelineAsset = m_PreviousPipelineAsset;
            }
        }
    }
}