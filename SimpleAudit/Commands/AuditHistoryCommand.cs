namespace SimpleAudit.Commands
{
	using Sitecore.Shell.Framework.Commands;

	public class AuditHistoryCommand : Command
	{
		public override void Execute(CommandContext context)
		{
			Sitecore.Context.ClientPage.ClientResponse.Alert("Test");
		}
	}
}
