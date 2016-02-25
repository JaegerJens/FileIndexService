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
            var d1 = blockList
                .SelectMany(f => f.Data);
            IObservable<string> dl = d1.SelectMany(sep.Separate);
            var d2 = dl.ToList();
            var d3 = d2.Select(t => Json(t));
            var data = await d3;
            return data;
        }
    }
}
