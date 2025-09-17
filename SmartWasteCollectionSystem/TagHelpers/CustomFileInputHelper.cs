using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWasteCollectionSystem.TagHelpers
{
    [HtmlTargetElement("Custom:FileInput")]
    public class CustomFileInputHelper : TagHelper
    {
        public bool IsImage { get; set; } = false;
        public string DisplayImage { get; set; } = "/Images/NoImageAvailable.png";
        public string Id { get; set; }
        public string? Name { get; set; }
        public string Label { get; set; }
        public string Accept { get; set; } = ".jpg,.jpeg,.png";
        public bool IsRequired { get; set; } = false;
        public string Value { get; set; }
        public string DisplayFileName { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "SysCoreTextBoxDivClass mb-3 row");

            if (IsImage)
            {
                output.Content.AppendHtml($@"<div class=""col"" style=""align-items: center; text-align: center; margin-bottom: 20px"">
                    <img id=""{Id}Preview"" src=""{DisplayImage}"" alt=""Selected Image"" class=""preview"" style=""width:250px"">

                    <script>
                        $(document).ready(function () {{
                            $(""#{Id}"").change(function (event) {{
                                let file = event.target.files[0];

                                if (file) {{
                                    let reader = new FileReader();
                                    reader.onload = function (e) {{
                                        $(""#{Id}Preview"").attr(""src"", e.target.result);
                                    }};
                                    reader.readAsDataURL(file);
                                }}
                            }});
                        }});
                    </script>
                </div>");
            }

            output.Content.AppendHtml($@"<div class=""SysCoreTextBoxDivClass mb-3 row"">
                <label for=""{Id}"" class=""form-label mt-0 {Id}SysCoreTextBoxLabelClass col-sm-4 col-form-label"">{Label}</label>
                <div class=""col-sm-8"">
                    <div id=""{Id}View"">
                        <a href=""#"" data-url=""/File/LoadPartial/{Value}"" class=""open-modal"">{DisplayFileName}</a>
                        <button type=""button"" id=""{Id}UploadNew"" class=""btn btn-outline-primary btn-sm""><i class=""fa-solid fa-upload""></i> Upload New</button>
                    </div>
                    
                    <div id=""{Id}Upload"" style=""display:none"">
                        <input 
                            type=""file"" 
                            class=""form-control"" 
                            id=""{Id}"" 
                            name=""{(!string.IsNullOrEmpty(Name) ? Name : Id)}"" 
                            accept=""{Accept}"" 
                            style=""margin-bottom: 20px;"" {(IsRequired ? "required" : "")}/>
                        <button type=""button"" id=""{Id}Cancel"" class=""btn btn-outline-danger btn-sm""><i class=""fa-solid fa-xmark""></i> Cancel</button>
                    </div>
                </div>
                <script>
                    $(document).ready(function () {{
                        $(""#{Id}UploadNew"").click(function (event) {{
                            $(""#{Id}View"").toggle(false);
                            $(""#{Id}Upload"").toggle(true);
                        }});
                        $(""#{Id}Cancel"").click(function (event) {{
                            $(""#{Id}View"").toggle(true);
                            $(""#{Id}Upload"").toggle(false);
                        }});
                    }});
                </script>
            </div>");
        }
    }
}
