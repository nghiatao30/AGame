using HyrphusQ.Events;
using UnityEngine;

public class HighestAchievedPPrefFloatTracker : PPrefFloatVariable
{
    [SerializeField] PPrefFloatVariable toBeTrackedVariable;

    private void OnEnable()
    {
        TrackHigherAchieved();
        toBeTrackedVariable.onValueChanged += HandleTrackedVariableChanged;
    }

    private void OnDisable()
    {
        toBeTrackedVariable.onValueChanged -= HandleTrackedVariableChanged;
    }

    private void HandleTrackedVariableChanged(ValueDataChanged<float> valueDataChanged)
    {
        TrackHigherAchieved();
    }

    private void TrackHigherAchieved()
    {
        if (value < toBeTrackedVariable.value)
        {
            value = toBeTrackedVariable.value;
        }
    }
}
