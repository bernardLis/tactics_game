using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Lis.Core
{
    public class ObjectShaders : MonoBehaviour
    {
        private Shader _dissolveShader;
        private GameManager _gameManager;
        private Shader _grayScaleShader;

        private Shader _litShader;
        private Shader _particlesUnlitShader;
        private Shader _sepiaToneShader;


        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void LitShader()
        {
            if (_litShader == null)
                _litShader = GameManager.Instance.GameDatabase.LitShader;
            if (_particlesUnlitShader == null)
                _particlesUnlitShader = GameManager.Instance.GameDatabase.ParticlesUnlitShader;

            List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
            foreach (Renderer r in renderers)
            {
                if (r is SpriteRenderer) continue;
                if (r is ParticleSystemRenderer) continue;

                Material mat = r.material;

                if (_gameManager == null) _gameManager = GameManager.Instance;
                if (_gameManager.GameDatabase.KeepShadersMaterials.Contains(mat.shader)) continue;

                Texture2D tex = mat.mainTexture as Texture2D;
                mat.shader = _litShader;
                mat.SetTexture("_Base_Texture", tex);

                mat.shader = _particlesUnlitShader;
            }
        }

        public void GrayScale()
        {
            if (_grayScaleShader == null)
                _grayScaleShader = GameManager.Instance.GameDatabase.GrayScaleShader;

            List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
            foreach (Renderer r in renderers)
            {
                if (r is SpriteRenderer) continue;
                if (r is ParticleSystemRenderer) continue;

                Material mat = r.material;

                if (_gameManager == null) _gameManager = GameManager.Instance;
                if (_gameManager.GameDatabase.KeepShadersMaterials.Contains(mat.shader)) continue;

                Texture2D tex = mat.mainTexture as Texture2D;
                mat.shader = _grayScaleShader;
                mat.SetTexture("_Base_Texture", tex);
            }
        }

        public void Dissolve(float time, bool isReverse)
        {
            if (_dissolveShader == null)
                _dissolveShader = GameManager.Instance.GameDatabase.DissolveShader;

            // btw. if you think about resetting shaders, it looks awful. 
            // Maybe if you came up with a way to transition from shader to shader it would make sense  
            List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
            foreach (Renderer r in renderers)
            {
                if (r is SpriteRenderer) continue;
                if (r is ParticleSystemRenderer) continue;

                Material mat = r.material;

                if (_gameManager == null) _gameManager = GameManager.Instance;
                if (_gameManager.GameDatabase.KeepShadersMaterials.Contains(mat.shader)) continue;

                mat.EnableKeyword("_NORMALMAP");
                mat.EnableKeyword("_METALLICGLOSSMAP");

                if (mat.shader != _dissolveShader) SetShader(mat);

                float startValue = isReverse ? 1f : 0f;
                float endValue = isReverse ? 0f : 1f;

                mat.SetFloat("_Dissolve", startValue);
                DOTween.To(x => mat.SetFloat("_Dissolve", x), startValue, endValue, time);
            }
        }

        private void SetShader(Material originalMat)
        {
            Vector2 texScale = originalMat.mainTextureScale; // tiling
            Texture2D tex = originalMat.mainTexture as Texture2D;
            Texture2D metallicMap = null;
            Texture2D normalMap = null;

            if (originalMat.HasProperty("_MetallicGlossMap"))
                metallicMap = originalMat.GetTexture("_MetallicGlossMap") as Texture2D;
            if (originalMat.HasProperty("_BumpMap"))
                normalMap = originalMat.GetTexture("_BumpMap") as Texture2D; // does not work

            originalMat.shader = _dissolveShader;
            originalMat.SetTexture("_Base_Texture", tex);
            if (metallicMap != null)
                originalMat.SetTexture("_R_Metallic_G_Occulsion_A_Smoothness", metallicMap);
            if (normalMap != null)
                originalMat.SetTexture("_BumpMap", normalMap);
            originalMat.SetVector("_Tiling", texScale);
        }
    }
}