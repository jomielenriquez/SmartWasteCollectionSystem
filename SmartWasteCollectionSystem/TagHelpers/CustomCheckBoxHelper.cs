using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:CheckBox")]
    public class CustomCheckBoxHelper : TagHelper
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool? IsHidden { get; set; }
        public bool IsChecked { get; set; } = false; // Added property for checked state

        public bool IsDisabled { get; set; } = false;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            string attribute = "SysCoreTextBoxDivClass mb-3 row";
            if (IsHidden ?? false)
            {
                attribute += " hidden";
            }
            output.Attributes.SetAttribute("class", attribute);

            output.Content.AppendHtml("<label for=\""
                + Id
                + "\" class=\"form-label mt-0 col-sm-4 col-form-label" + Id + "SysCoreTextBoxLabelClass\">"
                + Label + "</label>");

            var isChecked = IsChecked ? "checked" : "";
            var isDisabled = IsDisabled ? "disabled" : "";
            output.Content.AppendHtml($@"<div class=""col-sm-8"">
                <input type='checkbox' 
                       class='form-check-input {Id} SysCoreTextBoxClass' 
                       id='{Id}' 
                       name='{Id}' 
                       value='true' 
                       {isChecked} {isDisabled} /></div>
            ");

        }
    }
}
