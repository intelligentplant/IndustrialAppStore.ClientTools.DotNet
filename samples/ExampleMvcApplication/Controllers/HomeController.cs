using System.Diagnostics;

using ExampleMvcApplication.Models;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleMvcApplication.Controllers {
    public class HomeController : Controller {

        [Authorize]
        public async Task<IActionResult> Index(
            [FromServices] IndustrialAppStoreHttpClient iasClient,
            CancellationToken cancellationToken = default
        ) {
            var dataSources = await iasClient.DataSources.GetDataSourcesAsync(
                features: DataSourceDriverFeatures.ReadTagValues, 
                roles: DataCoreRoles.Read, 
                cancellationToken: cancellationToken);

            var selectItems = new List<SelectListItem>() { 
                new SelectListItem() {
                    Text = "Select a data source",
                    Disabled = true,
                    Selected = true
                }
            };

            selectItems.AddRange(dataSources.Select(x => new SelectListItem() { Text = x.Name.DisplayName, Value = x.Name.QualifiedName }));

            var viewModel = new IndexViewModel() {
                DataSources = selectItems
            };
            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<TagValue>), 200)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> FindTags(
            [FromBody] FindTagsRequest request,
            [FromServices] IndustrialAppStoreHttpClient iasClient,
            CancellationToken cancellationToken = default
        ) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (request.Page < 1) {
                request.Page = 1;
            }

            const int pageSize = 20;

            var tags = await iasClient.DataSources.FindTagsAsync(
                request.DataSourceName,
                string.IsNullOrWhiteSpace(request.TagNameFilter) ? "*" : request.TagNameFilter,
                page: request.Page,
                pageSize: pageSize,
                cancellationToken: cancellationToken
            );

            var viewModel = new TagListViewModel() {
                Request = request,
                Tags = tags,
                CanPageNext = tags.Count() == pageSize
            };

            return PartialView("_FindTagsPartial", viewModel);
        }


        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<TagValue>), 200)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChartData(
            [FromBody] GetChartDataRequest request,
            [FromServices] IndustrialAppStoreHttpClient iasClient,
            CancellationToken cancellationToken = default
        ) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var tagValues = await iasClient.DataSources.ReadPlotTagValuesAsync(
                request.DataSourceName,
                [request.TagName],
                "*-1D",
                "*",
                500,
                null,
                cancellationToken
            );

            if (tagValues == null || !tagValues.TryGetValue(request.TagName, out var values)) {
                return NotFound();
            }

            return Ok(values.Values);
        }

    }
}
