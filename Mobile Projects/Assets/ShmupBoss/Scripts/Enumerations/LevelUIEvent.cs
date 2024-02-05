namespace ShmupBoss
{
    /// <summary>
    /// <para>The type of event that occured in the level UI:</para>
    /// <br>Pause Button Pressed</br>
    /// <br>Resume Button Pressed</br>
    /// <br>Main Menu Button Pressed</br>
    /// <br>Retry Button Pressed</br>
    /// <br>Next Level Button Pressed</br>
    /// <br>Game Over Canvas Appear</br>
    /// <br>Level Complete Appear</br>
    /// </summary>
    public enum LevelUIEvent
    {
        PauseButtonPressed,
        ResumeButtonPressed,
        MainMenuButtonPressed,
        RetryButtonPressed,
        NextLevelButtonPressed,
        GameOverCanvasAppear,
        LevelCompleteAppear
    }
}