using Microsoft.AspNet.Mvc;
using LogFileService.Service;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LogFileService.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var refine = new DataBlockRefinement();
            var reader = new BlockReader();
            var blockList = reader.ReadBlocksFromFile(@"wwwroot\Data\testdata.txt");
            var data = await blockList
                .SelectMany(refine.Refine)
                .ToList()
                .Select(t => Json(t));
            return data;
        }
    }
}
