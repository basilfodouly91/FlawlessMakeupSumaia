using FlawlessMakeupSumaia.API.Models;

namespace FlawlessMakeupSumaia.API.Services
{
    public interface IEmailService
    {
        Task SendOrderNotificationToAdminAsync(Order order);
    }
}

