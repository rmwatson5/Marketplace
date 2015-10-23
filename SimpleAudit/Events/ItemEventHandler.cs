namespace SimpleAudit.Events
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SimpleAudit.Models;

	using Sitecore.Data.Fields;
	using Sitecore.Data.Items;
	using Sitecore.Events;

	public class ItemEventHandler
	{
		private AuditLogger auditLogger;
		protected AuditLogger AuditLogger => this.auditLogger ?? (this.auditLogger = new AuditLogger());

		protected const string EnableAuditing = "Enable Auditing";
		public void OnItemSaving(object sender, EventArgs args)
		{
			var changedItem = Event.ExtractParameter<Item>(args, 0);
			if (changedItem == null || Sitecore.Context.Database?.DataManager.GetWorkflowInfo(changedItem) == null)
			{
				return;
			}

			var origionalItem = changedItem.Database.GetItem(changedItem.ID, changedItem.Language, changedItem.Version);
			var changedFields = this.GetChangedFields(changedItem, origionalItem);
			if (!changedFields.Any())
			{
				return;
			}

			var itemInfo = this.GetItemInformation(changedItem, changedFields);
			this.AuditLogger.LogAudit(itemInfo);
		}

		protected IList<FieldInformation> GetChangedFields(Item changedItem, Item origionalItem)
		{
			changedItem.Fields.ReadAll();
			var fieldNames = changedItem.Fields.Where(this.FieldHasAuditingEnabled).Select(field => field.Name);

			var changedFields = fieldNames.Where(fieldName => changedItem[fieldName] != origionalItem[fieldName]);

			return changedFields.Select(changedFieldName => new FieldInformation
			{
				FieldId = changedItem.Fields[changedFieldName].ID.Guid,
				FieldName = changedFieldName,
				OldValue = origionalItem[changedFieldName],
				NewValue = changedItem[changedFieldName]
			}).ToList();
		}

		protected bool FieldHasAuditingEnabled(Field field)
		{
			var fieldItem = field.Database.Items.GetItem(field.ID);
            CheckboxField auditingField = fieldItem?.Fields[EnableAuditing];
			return auditingField != null && auditingField.Checked;
		}

		protected ItemInformation GetItemInformation(Item changedItem, IList<FieldInformation> changedFields)
		{
			return new ItemInformation
			{
				DateModified = DateTime.UtcNow,
				ItemId = changedItem.ID.Guid,
				UserName = changedItem.Statistics.UpdatedBy,
				FieldsModified = changedFields
			};
		}
	}
}
