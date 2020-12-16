using BiangStudio.Singleton;

public class GameStateManager : TSingletonBaseManager<GameStateManager>
{
    private GameState state = GameState.Default;

    public void SetState(GameState newState)
    {
        if (state != newState)
        {
            switch (state)
            {
                case GameState.Waiting:
                {
                    break;
                }
                case GameState.Playing:
                {
                    break;
                }
                case GameState.ESC:
                {
                    break;
                }
            }

            state = newState;
            switch (state)
            {
                case GameState.Waiting:
                {
                    Pause();
                    break;
                }
                case GameState.Playing:
                {
                    Resume();
                    break;
                }
                case GameState.ESC:
                {
                    Pause();
                    break;
                }
            }
        }
    }

    public GameState GetState()
    {
        return state;
    }

    private void Pause()
    {
    }

    private void Resume()
    {
    }
}

public enum GameState
{
    Default,
    Waiting,
    Playing,
    ESC,
}