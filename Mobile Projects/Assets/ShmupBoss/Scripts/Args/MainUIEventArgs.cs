namespace ShmupBoss
{
    /// <summary>
    /// System event arguments which store what type of event just occurred in the main menu, 
    /// there are only two options: if a button was pressed or a toggle was pressed, this 
    /// helps in playing different sounds for each type of press.
    /// </summary>
    public class MainUIEventArgs : System.EventArgs
    {
        public MainUIEvent UIEvent;

        public MainUIEventArgs(MainUIEvent mainUiEvent)
        {
            UIEvent = mainUiEvent;
        }
    }
}