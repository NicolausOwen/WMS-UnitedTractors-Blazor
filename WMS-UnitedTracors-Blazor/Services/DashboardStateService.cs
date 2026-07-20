namespace WMS_UnitedTracors_Blazor.Services
{
    public class DashboardStateService
    {
        public event Action? OnDashboardUpdated;

        public void NotifyDashboardUpdated()
        {
            OnDashboardUpdated?.Invoke();
        }
    }
}
