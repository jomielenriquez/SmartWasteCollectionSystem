using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:MultiCheckBox")]
    public class CustomMultiCheckBoxHelper : TagHelper
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool? IsHidden { get; set; }
        public string KeyProperty { get; set; } = "Id";     // Default key property
        public string ValueProperty { get; set; } = "Name"; // Default value property
        public List<object> Items { get; set; } = new();
        public List<object> SelectedItems { get; set; }
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

            output.Content.AppendHtml("<div class=\"col-sm-8\">");

            foreach (var item in Items)
            {
                var keyProp = item.GetType().GetProperty(KeyProperty, BindingFlags.Public | BindingFlags.Instance);
                var valueProp = item.GetType().GetProperty(ValueProperty, BindingFlags.Public | BindingFlags.Instance);

                if (keyProp != null && valueProp != null)
                {
                    var key = keyProp.GetValue(item)?.ToString();
                    var value = valueProp.GetValue(item)?.ToString();
                    
                    var isRoleSelected = SelectedItems.Any(x =>
                    {
                        var selectedKeyProp = x.GetType().GetProperty(KeyProperty, BindingFlags.Public | BindingFlags.Instance);
                        if (selectedKeyProp != null)
                        {
                            var selectedKey = selectedKeyProp.GetValue(x)?.ToString();
                            return selectedKey == key;
                        }
                        return false;
                    });

                    var isChecked = isRoleSelected ? "checked" : "";
                    output.Content.AppendHtml($@"<input type='checkbox' 
                               class='form-check-input {key} SysCoreTextBoxClass' 
                               id='{key}' 
                               name='{Id}' 
                               value='{key}' 
                               {isChecked} /> {value} <br/>
                    ");
                }
            }
            output.Content.AppendHtml("</div>");
        }
    }
}
