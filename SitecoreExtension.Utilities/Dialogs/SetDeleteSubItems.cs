using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Pages;
using Sitecore.Configuration;
using System;
using Sitecore.Data;
using System.Collections.Generic;
using Sitecore.SecurityModel;

namespace SitecoreExtension.Utilities.Dialogs
{
    public class SetDeleteSubItems : DialogForm
    {
        protected TreeList treeList;

        protected Database database = Factory.GetDatabase("master");
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            Assert.IsNotNull(itemFromQueryString, "Item not found.");
            treeList.SetValue(itemFromQueryString[FieldIDs.Branches]);
            treeList.ItemLanguage = Context.Language.Name;

            treeList.Source = itemFromQueryString.Paths.FullPath;

            List<string> arrayItems = new List<string>();

            foreach (Item child in itemFromQueryString.Children)
            {
                arrayItems.Add(child.Name);
            }

            treeList.ExcludeItemsForDisplay = itemFromQueryString.Name;
            treeList.IncludeItemsForDisplay = string.Join(",", arrayItems);
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));

            var arrayItem = treeList.GetValue().Split('|');

            foreach (var item in arrayItem)
            {
                Item sample = database.GetItem(new ID(item));

                using (new SecurityDisabler())
                {
                    sample.Recycle();
                }
            }

            base.OnOK(sender, args);
        }
    }
}
