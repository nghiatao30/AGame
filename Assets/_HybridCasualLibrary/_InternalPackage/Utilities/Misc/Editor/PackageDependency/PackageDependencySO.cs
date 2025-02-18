using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackageDependencySO : UnityEditor.SingletonSO<PackageDependencySO>
{
    [System.Serializable]
    public class Package
    {
        public Package(string packageName, string packageId)
        {
            this.packageName = packageName;
            this.packageId = packageId;
        }

        [field: SerializeField]
        public string packageName { get; set; }
        [field: SerializeField]
        public string packageId { get; set; }
    }

    [SerializeField]
    private List<Package> m_PackageDependencies = new List<Package>()
    {
        new Package("com.coffee.ui-particle", "https://github.com/mob-sakai/ParticleEffectForUGUI.git"),
        new Package("com.unity.textmeshpro", "com.unity.textmeshpro"),
    };

    public List<Package> packageDependencies => m_PackageDependencies;
}