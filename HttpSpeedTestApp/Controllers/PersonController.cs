using Bogus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Person = HttpSpeedTestApp.Models.Person;

namespace HttpSpeedTestApp.Controllers
{
    public class PersonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string requestUrl = "http://localhost:5000/people";
        public PersonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTest()
        {
            var startTime = DateTime.UtcNow;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(requestUrl);

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            return Content($"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{response}", "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> PostTest()
        {
            var startTime = DateTime.UtcNow;

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(GenerateJsonPayload(1000), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUrl, content);

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            return Content($"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}", "application/json");
        }

        [HttpPut]
        public async Task<IActionResult> PutTest()
        {
            var startTime = DateTime.UtcNow;

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(GenerateJsonPayload(1000), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUrl + "/ 1", content);

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            return Content($"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}", "application/json");
        }

        [HttpPatch]
        public async Task<IActionResult> PatchTest()
        {
            var startTime = DateTime.UtcNow;

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(GenerateJsonPayload(1000), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync(requestUrl + "/1", content);

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            return Content($"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}", "application/json");
        }

        private string GenerateJsonPayload(int count)
        {
            var faker = new Faker<Person>()
                .RuleFor(p => p.Id, f => f.IndexFaker)
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Age, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Email, f => f.Internet.Email());

            var people = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                people.Add(faker.Generate());
            }

            return System.Text.Json.JsonSerializer.Serialize(people);
        }
    }
}
