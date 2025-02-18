using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class Requirement
{
    public abstract bool IsMeetRequirement();
    public abstract void ExecuteRequirement();

    public static List<Requirement> GetAvailableLeafRequirements(Requirement requirementRoot)
    {
        if (requirementRoot == null)
            return null;
        if (!(requirementRoot is CompositeRequirement))
            return new List<Requirement>() { requirementRoot };
        var compositeRequirement = requirementRoot as CompositeRequirement;
        var op = compositeRequirement.op;
        var requirements = compositeRequirement.requirements;
        var resultRequirements = new List<Requirement>();
        for (int i = 0; i < requirements.Count; i++)
        {
            var requirement = requirements[i];
            var requirementsInLeaf = GetAvailableLeafRequirements(requirement);
            if (requirementsInLeaf == null)
                continue;
            if (op == CompositeRequirement.Operator.And)
            {
                resultRequirements.AddRange(requirementsInLeaf);
            }
            else if (requirement.IsMeetRequirement())
            {
                resultRequirements.AddRange(requirementsInLeaf);
                break;
            }
        }
        return resultRequirements.Count <= 0 ? GetAvailableLeafRequirements(requirements[0]) : resultRequirements;
    }
}

[System.Serializable]
public class CompositeRequirement : Requirement
{
    public CompositeRequirement()
    {

    }
    public CompositeRequirement(Operator op, List<Requirement> requirements)
    {
        m_Operator = op;
        m_Requirements = requirements;
    }

    public enum Operator
    {
        And,
        Or,
    }

    [SerializeField]
    protected Operator m_Operator = Operator.And;
    [SerializeReference]
    protected List<Requirement> m_Requirements;

    public Operator op => m_Operator;
    public List<Requirement> requirements => m_Requirements;

    public override bool IsMeetRequirement()
    {
        if (op == Operator.And)
        {
            return requirements.All(item => item.IsMeetRequirement());
        }
        else
        {
            return requirements.Any(item => item.IsMeetRequirement());
        }
    }

    public override void ExecuteRequirement()
    {
        if (op == Operator.And)
        {
            requirements.ForEach(item => item.ExecuteRequirement());
        }
        else
        {
            var firstOrDefault = requirements.Find(item => item.IsMeetRequirement());
            if (firstOrDefault == null)
            {
                Debug.LogError("Bruhhh???");
                return;
            }
            firstOrDefault.ExecuteRequirement();
        }
    }
}