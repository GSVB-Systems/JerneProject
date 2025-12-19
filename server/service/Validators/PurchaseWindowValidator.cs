namespace service.Validators

{
    public static class PurchaseWindowValidator
    {
        // Returns true if the provided moment (in given timezone) falls inside the maintenance window.
        public static bool IsWithinMaintenanceWindow(DateTime? momentUtc = null, TimeZoneInfo timezone = null)
        {
            timezone ??= TimeZoneInfo.Local;
            // Interpret momentUtc as UTC if provided, otherwise use current UTC
            var utc = (momentUtc?.ToUniversalTime()) ?? DateTime.UtcNow;
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, timezone);

            var dow = local.DayOfWeek;
            var tod = local.TimeOfDay;

            // Window: Saturday from 17:00 (inclusive) through the end of Sunday (all Sunday)
            var isSaturdayAfter17 = dow == DayOfWeek.Saturday && tod >= TimeSpan.FromHours(17);
            var isSunday = dow == DayOfWeek.Sunday;

            return isSaturdayAfter17 || isSunday;
        }

        // Throws BoardPurchaseNotAllowedException when inside the maintenance window.
        public static void EnsurePurchaseAllowed(DateTime? momentUtc = null, TimeZoneInfo timezone = null)
        {
            if (IsWithinMaintenanceWindow(momentUtc, timezone))
            {
                throw new service.Exceptions.BoardPurchaseNotAllowedException();
            }
        }
    }
}