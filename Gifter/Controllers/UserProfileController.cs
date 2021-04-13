using Gifter.Models;
using Gifter.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepo;

        public UserProfileController(IUserProfileRepository userProfileRepo)
        {
            _userProfileRepo = userProfileRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userProfileRepo.GetAll());
        }

        [HttpGet("GetUserIdWithComments/{id}")]
        public IActionResult GetUserIdWithComments(int id)
        {
            var user = _userProfileRepo.GetUserIdWithComments(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("GetUserIdWithPostsAndComments/{id}")]
        public IActionResult GetUserIdWithPostsAndComments(int id)
        {
            var user = _userProfileRepo.GetUserWithPostsAndComments(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult UserProfile(UserProfile user)
        {
            _userProfileRepo.Add(user);
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, UserProfile user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _userProfileRepo.Update(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userProfileRepo.Delete(id);
            return NoContent();
        }

    }
}
