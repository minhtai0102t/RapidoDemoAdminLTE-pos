using System;
using System.Collections.Generic;

namespace DemoAdminLTE.Extensions.Alerts
{
    public class AlertsContainer : List<Alert>
    {
        public void Merge(AlertsContainer alerts)
        {
            if (alerts == this)
                return;

            AddRange(alerts);
        }
        public void Add(AlertType type, string message, int timeout, string icon)
        {
            Add(new Alert { Type = type, Message = message, Timeout = timeout, Icon = icon });
        }

        public void AddInfo(string message, int timeout = ALERTS.DEFAULT_TIMEOUT)
        {
            Add(new Alert { Type = AlertType.Info, Message = message, Timeout = timeout, Icon = "glyphicon glyphicon-info-sign" });
        }
        public void AddError(string message, int timeout = ALERTS.DEFAULT_TIMEOUT)
        {
            Add(new Alert { Type = AlertType.Danger, Message = message, Timeout = timeout, Icon = "glyphicon glyphicon-remove-sign" });
        }
        public void AddSuccess(string message, int timeout = ALERTS.DEFAULT_TIMEOUT)
        {
            Add(new Alert { Type = AlertType.Success, Message = message, Timeout = timeout, Icon = "glyphicon glyphicon-ok-sign" });
        }
        public void AddWarning(string message, int timeout = ALERTS.DEFAULT_TIMEOUT)
        {
            Add(new Alert { Type = AlertType.Warning, Message = message, Timeout = timeout, Icon = "glyphicon glyphicon-exclamation-sign" });
        }
    }
}
