using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:ActionItems")]
    public class CustomActionItemsHelper : TagHelper
    {
        public enum TemplateType
        {
            None,
            Default
        }
        public required string Caption { get; set; }
        public TemplateType Template { get; set; }
        public string Id { get; set; }
        public string Table { get; set; }
        public required string Controller { get; set; }
        public string DeleteAction { get; set; }
        public string? EditAction { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("style", "margin-bottom: 10px;");

            //Header part
            output.Content.AppendHtml($@"<nav aria-label=""breadcrumb"">
                    <ol class=""breadcrumb"">
                        <li class=""breadcrumb-item active"" aria-current=""page"">{this.Caption}</li>
                    </ol>
                </nav>");

            output.Content.AppendHtml("<div>");
            // buttons
            if (this.Template == TemplateType.Default)
            {
                if (!string.IsNullOrEmpty(EditAction))
                {
                    output.Content.AppendHtml($"<a href=\"/{Controller}/{EditAction}\" type=\"button\" class=\"btn btn-link btn-sm\"><i class=\"fa-solid fa-plus\"></i> Add</a>");
                }
                output.Content.AppendHtml("<button id=\"" + Id + "Delete\" type=\"button\" class=\"btn btn-link btn-sm\"><i class=\"fa-solid fa-trash\"></i> Delete</button>");
            }

            output.Content.AppendHtml(output.GetChildContentAsync().Result);
            output.Content.AppendHtml("</div>");

            // Include JavaScript file
            output.Content.AppendHtml("<script src=\"/lib/taghelper/js/ActionItems.js\"></script>");
            output.Content.AppendHtml("<script>");
            output.Content.AppendHtml("$(function(){");
            output.Content.AppendHtml("const " + Id + "JsActionItems = new SysCoreActionItems('" + Id + "','" + Table + "','" + Controller + "','" + DeleteAction + "');");
            output.Content.AppendHtml("})");
            output.Content.AppendHtml("</script>");
        }
    }
}
