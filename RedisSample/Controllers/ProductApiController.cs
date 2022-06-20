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

            var dataAsByteArray = await _cache.GetAsync("search");

            if ((dataAsByteArray?.Count() ?? 0) > 0)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray);
                var products = JsonSerializer.Deserialize
                    <List<Product>>(serializedData);

                return Ok(new ProductResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsDataFromCache = true,
                    Data = products,
                    Message = "Data retrieved from Redis Cache",
                    Timestamp = DateTime.UtcNow
                });
            }

            var data = await _repo.GetSearchAsync();

            serializedData = JsonSerializer.Serialize(data);
            dataAsByteArray = Encoding.UTF8.GetBytes(serializedData);
            var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
            await _cache.SetAsync("search", dataAsByteArray, options);

            ProductResponse productResponse = new ProductResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsDataFromCache = false,
                Data = data,
                Message = "Data not available in Redis Cache", Timestamp = DateTime.UtcNow
            };

            return Ok(productResponse);
        }

    }
}
