using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:Dropdown")]
    public class CustomDropdownHelper : TagHelper
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public List<object> Items { get; set; } = new();

        public string KeyProperty { get; set; } = "Id";     // Default key property
        public string ValueProperty { get; set; } = "Name"; // Default value property

        public string SelectedValue { get; set; }
        public bool IsHidden { get; set; } = false;
        public string CssClass { get; set; } = "form-select";
        public bool IsRequired { get; set; } = false;
        public bool HasLabel { get; set; } = true;
        public string DropdownClass { get; set; } = "col-sm-8";
        public string PlaceHolder { get; set; } = "Please select";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            // Apply hidden class if necessary
            string cssClass = (IsHidden ? "SysCoreTextBoxDivClass hidden" : "SysCoreTextBoxDivClass") + " mb-3 row";
            output.Attributes.SetAttribute("class", cssClass);

            // Render label
            if (HasLabel)
            {
                output.Content.AppendHtml($@"
                    <label for='{Id}' class='form-label col-sm-4 col-form-label'>{Label}</label>
                ");
            }

            var isRequired = IsRequired ? "required" : "";
            // Render dropdown
            output.Content.AppendHtml($"<div class=\"{DropdownClass}\"><select id='{Id}' name='{Id}' class='{CssClass}' {isRequired}>");

            output.Content.AppendHtml($@"
                <option value=''>{PlaceHolder}</option>
            ");
            // Render dropdown items dynamically
            foreach (var item in Items)
            {
                var keyProp = item.GetType().GetProperty(KeyProperty, BindingFlags.Public | BindingFlags.Instance);
                var valueProp = item.GetType().GetProperty(ValueProperty, BindingFlags.Public | BindingFlags.Instance);

                if (keyProp != null && valueProp != null)
                {
                    var key = keyProp.GetValue(item)?.ToString();
                    var value = valueProp.GetValue(item)?.ToString();
                    var isSelected = key == SelectedValue ? "selected" : "";

                    output.Content.AppendHtml($@"
                        <option value='{key}' {isSelected}>{value}</option>
                    ");
                }
            }

            output.Content.AppendHtml("</select></div>");
        }
    }
}
