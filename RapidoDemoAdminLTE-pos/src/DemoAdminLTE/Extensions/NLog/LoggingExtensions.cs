using NLog;

namespace DemoAdminLTE.Extensions
{
    public static class LoggingHelper
    {
        public static void ToDatabase(this Logger logger, int userId, string action, string message)
        {
            var theEvent = new LogEventInfo(LogLevel.Trace, logger.Name, message);

            theEvent.Properties["UserId"] = userId;
            theEvent.Properties["Action"] = action;

            logger.Log(theEvent);
        }

    }
}