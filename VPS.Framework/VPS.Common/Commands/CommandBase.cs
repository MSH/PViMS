namespace VPS.Common.Commands
{
	public abstract class CommandBase
	{
		public virtual string FriendlyName
		{
			get { return this.GetType().Name; }
		}
	}
}