using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames.UnpackAnimation
{
    public class SpiralShapeParticleEmitter : MonoBehaviour
    {
        [System.Serializable]
        public class ParticleCluster
        {
            public float m_DelayTime = 0.05f;
            public float m_CircleRadius = 1f;
            public int m_NumOfParticles = 10;
            public float m_StartSize = 1;
            public float m_StartLifetime = 1f;
        }
        [SerializeField]
        private ParticleSystem m_SpiralDotParticle;
        [SerializeField]
        private List<ParticleCluster> m_ParticleClusters;

        private IEnumerator PlayFX_CR()
        {
            m_SpiralDotParticle.Stop();
            m_SpiralDotParticle.Play();
            for (int i = 0; i < m_ParticleClusters.Count; i++)
            {
                if (!Mathf.Approximately(m_ParticleClusters[i].m_DelayTime, 0f))
                    yield return new WaitForSeconds(m_ParticleClusters[i].m_DelayTime);
                var circleRadius = m_ParticleClusters[i].m_CircleRadius;
                var particleStartSize = m_ParticleClusters[i].m_StartSize;
                var particleStartLifetime = m_ParticleClusters[i].m_StartLifetime;
                var numOfParticles = m_ParticleClusters[i].m_NumOfParticles;
                var angleInDeg = 360f / numOfParticles;
                for (int j = 0; j < numOfParticles; j++)
                {
                    var x = Mathf.Cos(angleInDeg * j * Mathf.Deg2Rad) * circleRadius;
                    var y = Mathf.Sin(angleInDeg * j * Mathf.Deg2Rad) * circleRadius;
                    var emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = new Vector3(x, 0f, y);
                    emitParams.startSize = particleStartSize;
                    emitParams.startLifetime = particleStartLifetime;
                    m_SpiralDotParticle.Emit(emitParams, 1);
                }
            }
        }

        [Button]
        public void PlayFX()
        {
            StopAllCoroutines();
            StartCoroutine(PlayFX_CR());
        }
    }
}