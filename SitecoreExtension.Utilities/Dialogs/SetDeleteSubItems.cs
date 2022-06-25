using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Pages;
using Sitecore.Configuration;
using System;
using Sitecore.Data;
using Sitecore.SecurityModel;
using Checkbox = Sitecore.Shell.Applications.ContentEditor.Checkbox;

namespace SitecoreExtension.Utilities.Dialogs
{
    public class SetDeleteSubItems : DialogForm
    {
        protected MultilistEx SubItems;

        protected Checkbox checkbox;

        protected Database database = Factory.GetDatabase("master");
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            Assert.IsNotNull(itemFromQueryString, "Item not found.");

            //MultiList
            SubItems.ItemLanguage = Context.Language.Name;
            SubItems.Database = "master";
            SubItems.Source = itemFromQueryString.Paths.FullPath;
            SubItems.ItemID = itemFromQueryString.ID.ToString();
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));

            var arrayItem = SubItems.GetValue().Split('|');

            foreach (var item in arrayItem)
            {
                Item sample = database.GetItem(new ID(item));

                using (new SecurityDisabler())
                {
                    if (checkbox.Checked)
                    {
                        sample.Delete();
                    }
                    else
                    {
                        sample.Recycle();
                    }
                }
            }

            base.OnOK(sender, args);
        }
    }
}
