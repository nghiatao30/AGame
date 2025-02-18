[EventCode]
public enum IAPEventCode
{
    /// <summary>
    /// This event occurs when an IAP product purchase is initiated
    /// </summary>
    OnPurchaseItemStarted,
    /// <summary>
    /// This event is raised when the IAP product is purchased
    /// <para> <typeparamref name="LG_IAPButton"/>: _IAPButton </para>
    /// </summary>
    OnPurchaseItemCompleted,
    /// <summary>
    /// This event is raised when the purchase of the IAP product fails
    /// </summary>
    OnPurchaseItemFailed,
    /// <summary>
    /// This event is raised when completing restoring purchases
    /// </summary>
    OnRestorePurchasesCompleted,
    /// <summary>
    /// This event is raised when processing purchase
    /// <para> <typeparamref name="string"/>: productID </para>
    /// <para> <typeparamref name="LG_IAPButton"/>: _IAPButton </para>
    /// </summary>
    OnProcessPurchase,
    /// <summary>
    /// This event is raised when someone calls to jump to Standard currency pack sector
    /// </summary>
    OnJumpToStandardCurrencyPackSector,
    /// <summary>
    /// This event is raised when someone calls to jump to Premium currency pack sector
    /// </summary>
    OnJumpToPremiumCurrencyPackSector,
    /// <summary>
    /// This event is raised when the purchase processing is canceled
    /// </summary>
    OnPurchaseItemCanceled,
    /// <summary>
    /// This event is raised when someone calls to jump to RV Ticket pack sector
    /// </summary>
    OnJumpToRVTicketPackSector,
}