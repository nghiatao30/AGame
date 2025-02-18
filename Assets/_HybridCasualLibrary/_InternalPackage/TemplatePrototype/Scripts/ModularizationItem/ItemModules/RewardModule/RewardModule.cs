using UnityEngine;

public abstract class RewardModule : ItemModule, IReward
{
    public virtual void GrantReward()
    {

    }
}

public interface IReward
{
    void GrantReward();
}
