using System;
using System.Diagnostics;
namespace SqlDataProvider.BaseClass
{
	public static class ApplicationLog
	{
		public static void WriteError(string message)
		{
			ApplicationLog.WriteLog(TraceLevel.Error, message);
		}
		private static void WriteLog(TraceLevel level, string messageText)
		{
			try
			{
				EventLogEntryType type;
				if (level == TraceLevel.Error)
				{
					type = EventLogEntryType.Error;
				}
				else
				{
					type = EventLogEntryType.Error;
				}
				string text = "Application";
				if (!EventLog.SourceExists(text))
				{
					EventLog.CreateEventSource(text, "BIZ");
				}
				EventLog eventLog = new EventLog(text, ".", text);
				eventLog.WriteEntry(messageText, type);
			}
			catch
			{
			}
		}
	}
}
