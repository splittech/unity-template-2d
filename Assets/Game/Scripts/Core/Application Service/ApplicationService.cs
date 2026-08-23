namespace Game.Core
{
    public class ApplicationService
    {
        private readonly GameLogger _logger;

        public ApplicationService(ILoggingService loggingService)
        {
            _logger = loggingService.GetLogger(LoggingChannel.ApplicationService);
        }

        public void QuitGame()
        {
            _logger.Log("Quit game.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
