using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GrayscaleUI : MonoBehaviour
{
    [SerializeField] Material grayscaleMat;
    [SerializeField] List<Graphic> excludeList = new();

    List<Graphic> graphics = null;
    List<Material> baseMats = new();

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        graphics = new(GetComponentsInChildren<Graphic>(true));
        foreach (var exclude in excludeList)
        {
            graphics.Remove(exclude);
        }
        baseMats.Clear();
        foreach (var graphic in graphics)
        {
            baseMats.Add(graphic.material);
        }
    }

    public void SetGrayscale(bool shouldBeGrayscale)
    {
        if (graphics == null) Init();
        for (int i = 0; i < graphics.Count; i++)
        {
            graphics[i].material = shouldBeGrayscale ? grayscaleMat : baseMats[i];
        }
    }
}
