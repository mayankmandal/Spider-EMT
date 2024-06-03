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
        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
            }
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/DeleteEntity?deleteId={DeleteEntityViewModelData.EntityId}&deleteType={DeleteEntityViewModelData.EntityType}";
                HttpResponseMessage response;
                response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $"{DeleteEntityViewModelData.EntityType} Deleted Successfully" });
                }
                else
                {
                    return new JsonResult(new { success = true, message = $"{DeleteEntityViewModelData.EntityType} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
        private JsonResult HandleError(Exception ex, string errorMessage)
        {
            return new JsonResult(new { success = false, message = $"{DeleteEntityViewModelData.EntityType} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
