using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisSample.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisSample.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {

        private readonly IDistributedCache _cache;
        private Repo _repo;

        public ProductApiController(IDistributedCache cache, Repo repo)
        {
            _cache = cache;
            _repo = repo;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string serializedData = null;

            var dataAsByteArray = await _cache.GetAsync("search2");

            if (dataAsByteArray != null)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray);
                var products = JsonSerializer.Deserialize
                    <List<Product>>(serializedData);

                return new JsonResult(products);
            }

            var data = await _repo.GetSearchAsync();

            serializedData = JsonSerializer.Serialize(data);
            dataAsByteArray = Encoding.UTF8.GetBytes(serializedData);
            var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
            await _cache.SetAsync("search2", dataAsByteArray, options);

            ProductResponse productResponse = new ProductResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsDataFromCache = false,
                Data = data,
                Message = "Data not available in Redis Cache", Timestamp = DateTime.UtcNow
            };

            return Ok(productResponse);
        }


        [HttpPost]
        public async Task<JsonResult> Post([FromBody] Seo model)
        {
            var dataAsByteArray = await _cache.GetAsync("sea");
            string serializedData = null;

            if (dataAsByteArray != null)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray);
                var products = JsonSerializer.Deserialize<List<Seo>>(serializedData);

                products.Add(model);

                serializedData = JsonSerializer.Serialize(products);
                dataAsByteArray = Encoding.UTF8.GetBytes(serializedData);
                var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
                await _cache.SetAsync("sea", dataAsByteArray, options);
            }
            else
            {
                List<Seo> products = new List<Seo>();
                products.Add(model);
                serializedData = JsonSerializer.Serialize(products);
                dataAsByteArray = Encoding.UTF8.GetBytes(serializedData);
                var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
                await _cache.SetAsync("sea", dataAsByteArray, options);
            }
            return new JsonResult(Ok());
        }
    }

    public class Seo
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
