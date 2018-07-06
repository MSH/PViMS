using System;

namespace VPS.Common.Services
{
	public class DefaultDateService : DateService
	{
		private DateTime? _now;
		public override DateTime Now
		{
			get { return _now ?? DateTime.Now; }
		}

		public override void PushDate(DateTime date)
		{
			_now = date;
		}

		public override void PopDate()
		{
			_now = null;
		}
	}
}