using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:SearchForm")]
    public class CustomSearchFormHelper : TagHelper
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Id { get; set; }
        public string? Method { get; set; } = "post";
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "SysCoreSearchFormDivClass");

            output.Content.AppendHtml("<div class=\"accordion mb-3\"><div class=\"accordion-item\">");
            output.Content.AppendHtml("<h2 class=\"accordion-header\" id=\"" + Id + "AccordionHeader\">");
            output.Content.AppendHtml("<button class=\"accordion-button collapsed\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#" + Id + "Collapse\" aria-expanded=\"false\" aria-controls=\"" + Id + "Collapse\">Search</button></h2>");
            output.Content.AppendHtml("<div id=\"" + Id + "Collapse\" class=\"accordion-collapse collapse\" aria-labelledby=\"" + Id + "AccordionHeader\" data-bs-parent=\"#accordionExample\" style=\"\">");
            output.Content.AppendHtml("<div class=\"accordion-body\">");

            output.Content.AppendHtml("<form id=\"" + Id + "\" method='" + Method + "' action='/"
                + Controller
                + "/"
                + Action + "'>");

            output.Content.AppendHtml("<fieldset>");

            output.Content.AppendHtml(output.GetChildContentAsync().Result);

            output.Content.AppendHtml("<button type=\"submit\" class=\"btn btn-primary mt-2 mb-2\"><i class=\"bi bi-search\"></i> Search</button>");
            output.Content.AppendHtml("</fieldset></form></div>\r\n    </div>\r\n  </div></div>");
        }
    }
}
