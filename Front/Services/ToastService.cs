namespace Front.Services
{
    public class ToastService
    {
        public event Action<string, string> OnShowToast;

        public void ShowSuccess(string message)
        {
            OnShowToast?.Invoke(message, "success");
        }

        public void ShowError(string message)
        {
            OnShowToast?.Invoke(message, "error");
        }

        public void ShowInfo(string message)
        {
            OnShowToast?.Invoke(message, "info");
        }

        public void ShowWarning(string message)
        {
            OnShowToast?.Invoke(message, "warning");
        }
    }
}
