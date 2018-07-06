using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Services
{
	public delegate string FormatMessageHandler(string format, params object[] args);

	public interface ILoggerService
	{
		void Debug(Action<FormatMessageHandler> onFormatNeeded);
	}
}
