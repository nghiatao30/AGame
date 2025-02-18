/// <summary>
/// Define all type of Currency in game.
/// </summary>
public enum CurrencyType
{
    /// <summary>
    /// Soft currency, players can easily earn through game progression (usually in core-game loop)
    /// </summary>
    Standard,
    /// <summary>
    /// Hard currency, players can earn through real money, or advanced reward (gacha pack, advanced mission, etc.)
    /// </summary>
    Premium,
    /// <summary>
    /// Point represents for the proficiency of players, players can earn when they win a game and lose them when lose a game (usually in core-game loop)
    /// </summary>
    Medal,
    /// <summary>
    /// RVTicket is used for passing the watching ads to get rewards
    /// </summary>
    RVTicket,
}