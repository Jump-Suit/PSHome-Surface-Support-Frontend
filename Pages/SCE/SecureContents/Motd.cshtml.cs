using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PSHome_Surface_Support_Frontend.Infastructure.Services;

namespace PSHome_Surface_Support_Frontend.Pages.SCE.SecureContents
{
    public class MessageOfTheDayModel : PageModel
    {
        private readonly XmlConfigService _xmlService;

        [BindProperty]
        public string motdContent { get; set; }

        [BindProperty]
        public List<string> SelectedRegions { get; set; } = new List<string>();
        [BindProperty]
        public List<string> SelectedUserTypes { get; set; } = new List<string>();
        [BindProperty]
        public List<string> SelectedTerritories { get; set; } = new List<string>();
        public MessageOfTheDayModel(XmlConfigService xmlService)
        {
            _xmlService = xmlService;
        }

        public void OnGet()
        {
            motdContent = _xmlService.LoadMotd(_xmlService);

        }

        public IActionResult OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _xmlService.SaveMotdTxt(motdContent, SelectedRegions, SelectedTerritories);


            return RedirectToPage();
        }

    }
}
