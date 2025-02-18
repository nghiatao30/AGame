using UnityEngine;
using System.Collections.Generic;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaCardTemplates", menuName = "LatteGames/Gacha/Card/GachaCardTemplates", order = -100)]
    public class GachaCardTemplates : ScriptableObject
    {
        [SerializeReference] protected List<GachaCard> templates = new();

        public virtual T Generate<T>(params object[] _params) where T : GachaCard
        {
            var foundedTemplate = templates.Find(template => template is T);
            if (foundedTemplate == null) return null;
            return foundedTemplate.Clone(_params) as T;
        }
    }
}
