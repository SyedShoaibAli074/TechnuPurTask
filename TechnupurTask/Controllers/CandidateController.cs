using TechnupurTask.Core.Context;
using TechnupurTask.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using System.Security.Claims;
using TechnupurTask.Helper;
using Microsoft.AspNetCore.Authorization;

namespace TechnupurTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CandidateController : ControllerBase
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly TokenHelper tokenHelper;
        public CandidateController(ApplicationDBContext dbcontext,TokenHelper tokenHelper)
        {
            this.tokenHelper = tokenHelper; 
            _dbcontext = dbcontext;
        }

        // Create
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] Candidate request)
        {

            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Verify the token's validity using your preferred JWT library
            ClaimsPrincipal principal = tokenHelper.ValidateJwtToken(token);

            // Check if the token is valid
            if (principal == null)
            {
                return new JsonResult(new { Success = false, Message = "Access Denied" });
            }
            Candidate candidate = request;
            candidate.CreatedAt = DateTime.Now;
            candidate.UpdatedAt = DateTime.Now;
            candidate.CreatedBy = 1;
            await _dbcontext.Candidates.AddAsync(candidate);
            await _dbcontext.SaveChangesAsync();
            return Ok("Candidate Created Successfully.");
        }

        // Update
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update([FromForm] Candidate request)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Verify the token's validity using your preferred JWT library
            ClaimsPrincipal principal = tokenHelper.ValidateJwtToken(token);

            // Check if the token is valid
            if (principal == null)
            {
                return new JsonResult(new { Success = false, Message = "Access Denied" });
            }
            var candidate = await _dbcontext.Candidates.FindAsync(request.ID);
            if (candidate == null)
            {
                return NotFound($"No candidate found with Id {request.ID}.");
            }

            candidate.FirstName = request.FirstName;
            candidate.LastName = request.LastName;
            candidate.Email = request.Email;
            candidate.PhoneNumber = request.PhoneNumber;
            candidate.Address = request.Address;

            _dbcontext.Candidates.Update(candidate);
            await _dbcontext.SaveChangesAsync();

            return Ok("Candidate Updated Successfully.");
        }

        // Retrieve
        [HttpPost]
        [Route("Retrieve")]
        public async Task<ActionResult<Candidate>> Retrieve([FromBody] Candidate request)
        {
            var entity = await _dbcontext.Candidates.Where(x => x.ID == request.ID).FirstOrDefaultAsync();

            if (entity == null)
            {
                return Ok($"No entry with Id {request.ID}.");
            }

            return Ok(entity);
        }

        // Delete
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromBody] Candidate request)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Verify the token's validity using your preferred JWT library
            ClaimsPrincipal principal = tokenHelper.ValidateJwtToken(token);

            // Check if the token is valid
            if (principal == null)
            {
                return new JsonResult(new { Success = false, Message = "Access Denied" });
            }
            var candidate = await _dbcontext.Candidates.FindAsync(request.ID);
            if (candidate == null)
            {
                return NotFound($"No candidate found with Id {request.ID}.");
            }

            _dbcontext.Candidates.Remove(candidate);
            await _dbcontext.SaveChangesAsync();

            return Ok("Candidate Deleted Successfully.");
        }

        // List
        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<Candidate>>> List()
        {
            var list = await _dbcontext.Candidates.ToListAsync();
            return Ok(list);
        }
    }
}
