using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template
{
    public class SceneSoundLibrary : MonoBehaviour
    {

        [SerializeField]
        bool m_SelfInitialize=true;
        [SerializeField]
        List<SoundLibrarySO> m_SceneSFXLibrarySO;
        [SerializeField]
        List<SoundLibrarySO> m_SceneBGMLibrarySO;

        public List<SoundLibrarySO> SceneSFXLibrarySO => m_SceneSFXLibrarySO;
        public List<SoundLibrarySO> SceneBGMLibrarySO => m_SceneBGMLibrarySO;

        private void Start()
        {
            if (m_SelfInitialize)
                Initialize();
        }
        
        public void Initialize()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.AddSceneSoundLibrary(this);
        }

        private void OnDestroy()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.RemoveSceneSoundLibrary(this);
        }
    }
}