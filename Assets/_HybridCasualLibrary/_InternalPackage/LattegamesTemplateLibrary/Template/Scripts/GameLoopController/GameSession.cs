namespace LatteGames{
public class GameSession
{
    private readonly LevelController levelController;
    private readonly LevelAsset levelAsset;
    public GameSession(LevelAsset levelAsset, LevelController levelController){
        this.levelAsset = levelAsset;
        this.levelController = levelController;
    }
    public LevelController LevelController { get => levelController; }
    public LevelAsset LevelAsset { get => levelAsset; }
}
}