namespace Front.Services
{
    public class NotificationService
    {
        public event Func<string, string, Task> OnNotificationReceived;

        public async Task NotifyAsync(string title, string message)
        {
            if (OnNotificationReceived != null)
            {
                await OnNotificationReceived.Invoke(title, message);
            }
        }

        public async Task NotifyOrderStatusAsync(int orderId, string status)
        {
            var statusMessage = status switch
            {
                "Processing" => "Votre commande est en cours de préparation",
                "Shipped" => "Votre commande a été expédiée",
                "Delivered" => "Votre commande a été livrée",
                "Cancelled" => "Votre commande a été annulée",
                _ => "Mise à jour de statut"
            };

            await NotifyAsync($"Commande #{orderId}", statusMessage);
        }
    }
}
