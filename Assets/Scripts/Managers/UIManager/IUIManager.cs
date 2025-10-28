namespace Manager {
    public interface IUIManager : IManager {
        /// <summary>
        /// Check if the current mouse cursor is over an UI-Element
        /// </summary>
        /// <returns></returns>
        bool IsMouseOverUI();
    }
}