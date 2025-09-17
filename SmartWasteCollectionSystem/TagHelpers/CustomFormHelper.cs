using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:Form")]
    public class CustomFormHelper : TagHelper
    {
        public required string Controller { get; set; }
        public required string Action { get; set; }
        public string Id { get; set; }
        public string? SubmitTag { get; set; }
        public string? SubmitIcon { get; set; } = "fa-floppy-disk";
        public string? CancelRedirect { get; set; }
        public string? Enctype { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "SysCoreSearchFormDivClass");

            output.Content.AppendHtml("<form id=\"" + Id + "\" " + (!string.IsNullOrEmpty(Enctype) ? "enctype=\"" + Enctype + "\"" : "") + " method='post' action='/"
                + Controller
                + "/"
                + Action + "'>");

            output.Content.AppendHtml("<fieldset>");

            output.Content.AppendHtml(output.GetChildContentAsync().Result);

            if (!string.IsNullOrEmpty(SubmitTag))
            {
                output.Content.AppendHtml("<div class=\"SysCoreTextBoxDivClass\"><button type=\"submit\" class=\"btn btn-primary mt-2 mb-2 me-2\"><i class=\"fa-solid " + SubmitIcon + "\"></i> " + (SubmitTag ?? "Submit") + "</button>");
            }
            if (!string.IsNullOrEmpty(CancelRedirect))
            {
                output.Content.AppendHtml("<a href=\"" + CancelRedirect + "\" class=\"btn btn-danger mt-2 mb-2\"><i class=\"fa-solid fa-xmark\"></i> Cancel</a></div>");
            }
            output.Content.AppendHtml("</fieldset></form>");
        }
    }
}
