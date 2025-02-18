using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
using UnityEngine.UI;

public class DockerTab : ComposeCanvasElementVisibilityController
{
    [SerializeField]
    protected ButtonType buttonType;
    protected Dictionary<GraphicRaycaster, bool> raycasterActiveStateDictionary = new Dictionary<GraphicRaycaster, bool>();

    public ButtonType ButtonType => buttonType;

    public override void Show()
    {
        base.Show();
        if (raycasterActiveStateDictionary.Count <= 0)
            return;
        var raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        foreach (var raycaster in raycasters)
        {
            if (raycasterActiveStateDictionary.TryGetValue(raycaster, out bool activeState))
            {
                raycaster.enabled = activeState;
            }
        }
    }

    public override void ShowImmediately()
    {
        base.ShowImmediately();
        if (raycasterActiveStateDictionary.Count <= 0)
            return;
        var raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        foreach (var raycaster in raycasters)
        {
            if (raycasterActiveStateDictionary.TryGetValue(raycaster, out bool activeState))
            {
                raycaster.enabled = activeState;
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
        raycasterActiveStateDictionary.Clear();
        var raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        foreach (var raycaster in raycasters)
        {
            raycasterActiveStateDictionary.Add(raycaster, raycaster.enabled);
            raycaster.enabled = false;
        }
    }

    public override void HideImmediately()
    {
        base.HideImmediately();
        raycasterActiveStateDictionary.Clear();
        var raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        foreach (var raycaster in raycasters)
        {
            raycasterActiveStateDictionary.Add(raycaster, raycaster.enabled);
            raycaster.enabled = false;
        }
    }
}