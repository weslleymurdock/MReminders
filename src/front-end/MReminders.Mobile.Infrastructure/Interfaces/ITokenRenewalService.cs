namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface ITokenRenewalService
{
    void StartTokenMonitoring();
    void StopTokenMonitoring(); 
}
