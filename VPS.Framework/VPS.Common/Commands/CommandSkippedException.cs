using System;

namespace VPS.Common.Commands
{
	public class CommandSkippedException : Exception
	{
		public CommandSkippedException(string message): base(message)
		{
		}
	}
}