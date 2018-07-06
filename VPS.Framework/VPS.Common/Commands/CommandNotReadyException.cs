using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Commands
{
	public class CommandNotReadyException : Exception
	{
		public CommandNotReadyException(string message) : base(message)
		{
		}
	}
}
