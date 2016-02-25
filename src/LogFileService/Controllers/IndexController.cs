using Microsoft.AspNet.Mvc;
using LogFileService.Service;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System;

namespace LogFileService.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var sep = new LineSeparator();
            var reader = new BlockReader();
            var blockList = reader.ReadBlocksFromFile(@"wwwroot\Data\testdata.txt");
            var data = await blockList
                .SelectMany(f => f.Data)
                .SelectMany(sep.Separate)
                .ToList()
                .Select(t => Json(t));
            return data;
        }
    }
}
