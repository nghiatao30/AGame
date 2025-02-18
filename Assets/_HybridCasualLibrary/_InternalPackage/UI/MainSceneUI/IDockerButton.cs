using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.UI;

public interface IDockerButton : ISelectionResponse
{
    public ButtonType ButtonType { get; }
    public EventCode EventCode { get; }
    public DockerTab DockerTab { get; }
    public Button Button { get; }
}