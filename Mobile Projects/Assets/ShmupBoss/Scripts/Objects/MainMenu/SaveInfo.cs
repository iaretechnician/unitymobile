namespace ShmupBoss
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SaveInfo
    {
        public int SelectedPlayerIndex;
        public int AccumulatedCoins;

        public SavedLevel[] SavedLevels;

        public SaveInfo(int levelsNumber)
        {
            if(levelsNumber < 1)
            {
                return;
            }

            SavedLevel[] savedLevels = new SavedLevel[levelsNumber];

            for (int i =0; i < levelsNumber; i++)
            {
                savedLevels[i] = new SavedLevel();
            }

            savedLevels[0].Status = LevelStatus.Ongoing;

            SavedLevels = savedLevels;
        }
    }
}