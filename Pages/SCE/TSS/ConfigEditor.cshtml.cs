using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PSHome_Surface_Support_Frontend.Infastructure.Services;
using PSHome_Surface_Support_Frontend.Infastructure.Data.Models;

namespace PSHome_Surface_Support_Frontend.Pages.SCE.TSS
{
    public class ConfigEditor : PageModel
    {
        private readonly XmlConfigService _xmlService;

        public ConfigEditor(XmlConfigService xmlService)
        {
            _xmlService = xmlService;
        }

        [BindProperty]
        public TSSConfiguration Config { get; set; }
        [BindProperty]
        public bool IsRetail { get; set; } 
        [BindProperty]
        public bool IncludeEnvironmentClosed { get; set; }
        [BindProperty]
        public bool EnableSecureCommercePoints { get; set; }
        [BindProperty]
        public bool UseRegionalServiceIds { get; set; }
        [BindProperty]
        public bool EnableHTTPGZipCompression { get; set; }
        [BindProperty]
        public bool EnableHTTPDeflateCompression { get; set; }
        [BindProperty]
        public HTTPCompressionSubsystems HTTPCompressionSubsystems { get; set; }
        [BindProperty]
        public string maxServiceIds { get; set; }
        [BindProperty]
        public string AdminObjectId { get; set; }
        [BindProperty]
        public bool disableBarSupport { get; set; }
        [BindProperty]
        public bool override182Sharc { get; set; }

        [BindProperty]
        public List<string> SelectedRegions { get; set; } = new List<string>();

        public List<string> AvailableRegions { get; set; } = new List<string>
        {
            "en-US",    // SCEA
            "en-GB",    // SCEE
            "fr-FR",    // SCEE
            "de-DE",    // SCEE
            "es-ES",    // SCEE
            "it-IT",    // SCEE
            "ja-JP",    // SCEJ
            "ko-KR",    // SCEAsia
            "zh-TW",    // SCEAsia
            "zh-HK",    // SCEAsia
            "en-SG"     // SCEAsia
        };

        public void OnGet()
        {
            Config = _xmlService.LoadTSSConfiguration(_xmlService);
        }

        public IActionResult OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            Task<List<string>> result = _xmlService.SaveTSSConfiguration(Config, 
                SelectedRegions, 
                IsRetail, 
                IncludeEnvironmentClosed,
                disableBarSupport,
                EnableHTTPGZipCompression,
                EnableHTTPDeflateCompression,
                EnableSecureCommercePoints,
                UseRegionalServiceIds,
                maxServiceIds,
                AdminObjectId,
                HTTPCompressionSubsystems,
                override182Sharc
                );

            foreach(var code in result.Result)
            {
                if (code.Contains("ERROR"))
                {
                    if(code.Contains("MULTIPLE ENCODINGS"))
                    {
                        TempData["ERROR"] = "Cannot use multiple HTTP encodings";
                    }
                    else if(code.Contains("Object ID"))
                    {
                        TempData["ERROR"] = TempData["ERROR"] + " & Invalid Admin Object ID!";
                    }
                }
                else if (code.Contains("WARN"))
                {
                    if(code.Contains("admin object ID"))
                    {
                        TempData["WARN"] = "No admin object ID specified, admin functionality unavailable!";
                    } 

                }
                else
                {
                    TempData["Message"] = "Configuration saved successfully for selected regions.";
                }
            }

            return RedirectToPage();
        }
    }
}
