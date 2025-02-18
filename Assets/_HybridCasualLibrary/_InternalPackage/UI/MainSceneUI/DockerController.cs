using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HyrphusQ.Events;

public class DockerController : MonoBehaviour
{
    protected static ButtonType s_CurrentSelectedButtonType = ButtonType.Main;

    [SerializeField]
    protected EventCode anyTabButtonClickedEventCode;

    protected IDockerButton selectedDockerButton = null;
    protected IDockerButton[] dockerButtons = null;

    public virtual IDockerButton SelectedDockerButton
    {
        get => selectedDockerButton;
        set
        {
            if (selectedDockerButton == value || value == null)
                return;
            var previousSelectedDockerButton = selectedDockerButton;
            selectedDockerButton = value;
            s_CurrentSelectedButtonType = value.ButtonType;
            OnButtonClick(previousSelectedDockerButton, selectedDockerButton);
        }
    }

    protected virtual void Awake()
    {
        dockerButtons = GetComponentsInChildren<IDockerButton>();
        foreach (var dockerButton in dockerButtons)
        {
            dockerButton.Button.onClick.AddListener(() => OnSelectedButtonChanged(dockerButton));
        }
    }

    protected virtual void Start()
    {
        foreach (var dockerButton in dockerButtons)
        {
            if (s_CurrentSelectedButtonType == dockerButton.ButtonType)
            {
                SelectedDockerButton = dockerButton;
                break;
            }
        }
    }

    protected virtual void OnSelectedButtonChanged(IDockerButton dockerButton)
    {
        SelectedDockerButton = dockerButton;
    }

    protected virtual void OnButtonClick(IDockerButton previousButton, IDockerButton currentButton)
    {
        currentButton?.Select();
        previousButton?.Deselect();

        previousButton?.DockerTab.Hide();
        currentButton?.DockerTab.Show();

        GameEventHandler.Invoke(anyTabButtonClickedEventCode);
        GameEventHandler.Invoke(currentButton.EventCode);
    }

    public virtual IDockerButton GetDockerButtonOfType(ButtonType buttonType)
    {
        foreach (var dockerButton in dockerButtons)
        {
            if (dockerButton != null && dockerButton.ButtonType == buttonType)
                return dockerButton;
        }
        return null;
    }

    public virtual Button GetButtonOfType(ButtonType buttonType)
    {
        var dockerButton = GetDockerButtonOfType(buttonType);
        if (dockerButton == null)
            return null;
        return dockerButton.Button;
    }

    public virtual void SelectManuallyButtonOfType(ButtonType buttonType)
    {
        var button = GetButtonOfType(buttonType);
        if (button == null)
            return;
        button.onClick.Invoke();
    }
}