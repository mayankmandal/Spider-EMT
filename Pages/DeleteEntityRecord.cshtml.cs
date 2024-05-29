using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using System.Text;

namespace Spider_EMT.Pages
{
    public class DeleteEntityRecordModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public DeleteEntityRecordModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public DeleteEntityViewModel DeleteEntityViewModelData { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                DeleteEntityViewModelData = new DeleteEntityViewModel();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading data.");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Return to the same page if validation fails
            }
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/DeleteEntity?deleteId={DeleteEntityViewModelData.EntityId}&deleteType={DeleteEntityViewModelData.EntityType}";
                HttpResponseMessage response;
                response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = $"{DeleteEntityViewModelData.EntityType} Deleted Successfully";
                    return RedirectToPage("/DeleteEntityRecord");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                return HandleError(ex, "Error occurred during HTTP request.");
            }
            catch (JsonException ex)
            {
                return HandleError(ex, "Error occurred while parsing JSON.");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "An unexpected error occurred.");
            }
        }
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + " Error details: " + ex.Message;
            return RedirectToPage("/Error");
        }
    }
}
