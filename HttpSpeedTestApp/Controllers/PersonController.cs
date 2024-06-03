using Bogus;
using Bogus.Bson;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Person = HttpSpeedTestApp.Models.Person;

namespace HttpSpeedTestApp.Controllers
{

    [Route("[controller]")]
    public class PersonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string requestUrl = "http://localhost:33786/Person";
        public PersonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GetCurrent")]
        public async Task<IActionResult> GetCurrent()
        {
            var id = 5;
            var client = _httpClientFactory.CreateClient();
            var request = $"{requestUrl}/{id}";

            try
            {
                var startTime = DateTime.UtcNow;

                var response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var endTime = DateTime.UtcNow;
                    var elapsedTime = endTime - startTime;

                    var message = $"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}";
                    return Json(message);
                }
                else
                {
                    var errorMessage = $"Error: {response.StatusCode}\nContent: {await response.Content.ReadAsStringAsync()}";
                    return Json(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return Json(500, $"Internal server error: {ex.Message}");
            }
        }    

        [HttpGet]
        [Route("GetTest")]
        public async Task<IActionResult> GetTest()
        {

            var client = _httpClientFactory.CreateClient();
            var startTime = DateTime.UtcNow;
            var response = await client.GetStringAsync(requestUrl);
            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;
            List<Person> people = JsonConvert.DeserializeObject<List<Person>>(response);
            var currentModel=people.Where(x=>x.Email== "jane.doe@example.com").ToList();
            var message = $"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nİtem count: {people.Count}";
            return Json(message);
        }

        [HttpPost]
        [Route("PostTest")]
        public async Task<IActionResult> PostTest()
        {

            var client = _httpClientFactory.CreateClient();
            var model = GenerateJsonPayload2();
            var jsonPayload = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var startTime = DateTime.UtcNow;
                var response = await client.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var endTime = DateTime.UtcNow;
                    var elapsedTime = endTime - startTime;

                    var message = $"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}";
                    return Json(message);
                }
                else
                {
                    var errorMessage = $"Error: {response.StatusCode}\nContent: {await response.Content.ReadAsStringAsync()}";
                    return Json(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return Json(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("PutTest")]
        public async Task<IActionResult> PutTest()
        {

            var client = _httpClientFactory.CreateClient();
            var model = GenerateJsonPayload2();
            var jsonPayload = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var startTime = DateTime.UtcNow;

                var response = await client.PutAsync($"{requestUrl}/{5}", content);

                if (response.IsSuccessStatusCode)
                {
                    var endTime = DateTime.UtcNow;
                    var elapsedTime = endTime - startTime;

                    var message = $"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}";
                    return Json(message);
                }
                else
                {
                    var errorMessage = $"Error: {response.StatusCode}\nContent: {await response.Content.ReadAsStringAsync()}";
                    return Json(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return Json(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPatch]
        [Route("PatchTest")]
        public async Task<IActionResult> PatchTest()
        {

            var client = _httpClientFactory.CreateClient();
          
            var jsonPayload= GeneratePatchPayload();
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var startTime = DateTime.UtcNow;

                var response = await client.PatchAsync($"{requestUrl}/{5}", content);

                if (response.IsSuccessStatusCode)
                {
                    var endTime = DateTime.UtcNow;
                    var elapsedTime = endTime - startTime;

                    var message = $"Start Time: {startTime:O}\nEnd Time: {endTime:O}\nElapsed Time: {elapsedTime.TotalMilliseconds} ms\n\nResponse:\n{await response.Content.ReadAsStringAsync()}";
                    return Json(message);
                }
                else
                {
                    var errorMessage = $"Error: {response.StatusCode}\nContent: {await response.Content.ReadAsStringAsync()}";
                    return Json(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return Json(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GeneratePatchPayload()
        {
            var model = GenerateJsonPayload2();
            var updates = new Dictionary<string, string>
    {
        { "Email", model.Email }
    };

            return JsonConvert.SerializeObject(updates);
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

            return JsonConvert.SerializeObject(people);
        }
        private Person GenerateJsonPayload2()
        {
            var faker = new Faker<Person>()
                .RuleFor(p => p.Id, f => f.Random.Int(0, 5000000))
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Age, f => f.Random.Int(0, 100))
                .RuleFor(p => p.Email, f => f.Internet.Email()); 

            var person = faker.Generate();

            return person;
        }


    }
}
