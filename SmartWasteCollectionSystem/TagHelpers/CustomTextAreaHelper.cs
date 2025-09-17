using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:TextArea")]
    public class CustomTextAreaHelper : TagHelper
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Placeholder { get; set; }
        public string Value { get; set; }
        public string? Pattern { get; set; }
        public bool? IsHidden { get; set; }
        public bool IsRequired { get; set; } = false;
        public bool IsHelperEnabled { get; set; } = false;
        public string HelperMessage { get; set; } = "";
        public string? Step { get; set; }
        public bool? ReadOnly { get; set; } = false;
        public bool HasLabel { get; set; } = true;
        public string TextboxClass { get; set; } = "col-sm-8";
        public int? Rows { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            string attribute = "SysCoreTextBoxDivClass mb-3 row";
            if (IsHidden ?? false)
            {
                attribute += " hidden";
            }
            output.Attributes.SetAttribute("class", attribute);

            if (HasLabel)
            {
                output.Content.AppendHtml("<label for=\""
                    + Id
                    + "\" class=\"form-label mt-0 " + Id + "SysCoreTextBoxLabelClass col-sm-4 col-form-label\">"
                    + Label + "</label>");
            }

            if (ReadOnly ?? false)
            {
                output.Content.AppendHtml($@"<div class=""{TextboxClass}""><label class=""col-form-label"">{Value}</label></div>");
            }
            else
            {
                var isRequired = IsRequired ? "required" : "";
                output.Content.AppendHtml("<div class=\"" + TextboxClass + "\"><textarea " + (!string.IsNullOrEmpty(Step) ? "step=\"" + Step + "\"" : "") + " type=\""
                    + Type
                    + "\" class=\"form-control " + Id
                    + "SysCoreTextBoxClass\" id=\""
                    + Id
                    + "\" name=\"" + Id
                    + (!string.IsNullOrEmpty(Pattern) ? "\" pattern=\"" + Pattern : "")
                    + "\" placeholder=\"" + Placeholder + "\" " + isRequired + " "
                    + (Rows.HasValue ? "rows='" + Rows + "' ": "") + "> " 
                    + Value +"</textarea></div>"
                    + (IsHelperEnabled ? "<div id=\"emailHelp\" class=\"form-text\">" + HelperMessage + "</div>" : ""));
            }

        }
    }
}
