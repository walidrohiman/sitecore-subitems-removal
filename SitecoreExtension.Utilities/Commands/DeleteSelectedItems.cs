using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;
using Sitecore;

namespace SitecoreExtension.Utilities.Commands
{
    public class DeleteSelectedItems : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            if (context.Items.Length != 1)
                return;
            Item obj = context.Items[0];
            Context.ClientPage.Start(this, "Run", new NameValueCollection()
            {
                ["id"] = obj.ID.ToString(),
                ["language"] = obj.Language.ToString(),
                ["version"] = obj.Version.ToString()
            });
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Item itemNotNull = Client.GetItemNotNull(args.Parameters["id"], Language.Parse(args.Parameters["language"]), Sitecore.Data.Version.Parse(args.Parameters["version"]));
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                {
                    string load = $"item:refreshchildren(id={args.Parameters["id"]},language={args.Parameters["language"]},version={args.Parameters["version"]})";
                    Context.ClientPage.ClientResponse.Timer(load, 1);
                }
            }
            else
            {
                UrlString urlString = new UrlString(UIUtil.GetUri("control:DeleteSelectedChildren"));
                urlString.Append("id", args.Parameters["id"]);
                urlString.Append("la", args.Parameters["language"]);
                urlString.Append("vs", args.Parameters["version"]);
                SheerResponse.ShowModalDialog(urlString.ToString(), "1200px", "700px", string.Empty, true);
                args.WaitForPostBack();
            }
        }
    }
}
