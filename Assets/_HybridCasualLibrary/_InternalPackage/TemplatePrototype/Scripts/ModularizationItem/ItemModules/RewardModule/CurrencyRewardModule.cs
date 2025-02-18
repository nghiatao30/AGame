using UnityEngine;

public class CurrencyRewardModule : RewardModule
{
    public CurrencyType CurrencyType;
    [SerializeField] int amount;
    public int Amount { get => amount; set => amount = value; }

    public IResourceLocationProvider resourceLocationProvider { get; set; }

    public override void GrantReward()
    {
        if (CurrencyManager.Instance == null)
            return;
        if (Equals(resourceLocationProvider, null))
        {
            CurrencyManager.Instance.GetCurrencySO(CurrencyType).AcquireWithoutLogEvent(Amount);
        }
        else
        {
            CurrencyManager.Instance.GetCurrencySO(CurrencyType).Acquire(Amount, resourceLocationProvider.GetLocation(), resourceLocationProvider.GetItemId());
        }
    }
}