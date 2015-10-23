namespace SimpleAudit.Models
{
	using System;

	public class FieldInformation
	{
		public Guid FieldId { get; set; }
		public string FieldName { get; set; }
		public string OldValue { get; set; }
		public string NewValue { get; set; }
	}
}
