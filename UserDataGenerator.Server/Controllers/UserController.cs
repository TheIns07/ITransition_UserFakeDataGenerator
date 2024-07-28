using Bogus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using UserDataGenerator.Server.Models;

namespace UserDataGenerator.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly Faker _faker;
        
        public UserController()
        {
            _faker = new Faker();
        }

        [HttpGet("{region}/{errors}/{seed}")]
        public IActionResult GetUsers(string region, int errors, int seed)
        {
            _faker.Locale = region.ToLower() switch
            {
                "poland" => "pl",
                "usa" => "en",
                "georgia" => "ka",
                _ => "en"
            };

            _faker.Random = new Randomizer(seed);

            var users = new List<FakeUserDTO>();
            for (int i = 0; i < 20; i++)
            {
                var user = new FakeUserDTO
                {
                    Id = i + 1,
                    Identifier = Guid.NewGuid(),
                    Name = _faker.Name.FullName(),
                    Address = _faker.Address.FullAddress(),
                    Phone = _faker.Phone.PhoneNumber()
                };

                ApplyErrors(user, errors);
                users.Add(user);
            }

            return Ok(users);
        }

        private void ApplyErrors(FakeUserDTO user, int errors)
        {
            var errorCount = (int)Math.Floor(errors * 0.1 * 5);
            for (int i = 0; i < errorCount; i++)
            {
                var randomField = _faker.PickRandom("Name", "Address", "Phone");
                switch (randomField)
                {
                    case "Name":
                        user.Name = _faker.Random.String2(10);
                        break;
                    case "Address":
                        user.Address = _faker.Random.String2(20);
                        break;
                    case "Phone":
                        user.Phone = _faker.Random.String2(10);
                        break;
                }
            }
        }
    }
}
