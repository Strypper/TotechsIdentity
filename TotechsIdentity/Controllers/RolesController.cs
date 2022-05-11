using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TotechsIdentity.DataObjects;

namespace TotechsIdentity.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<Role> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.NormalizedName).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<RoleDTO>>(roles));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDTO dTO)
        {
            var role = _mapper.Map<Role>(dTO);
            await _roleManager.CreateAsync(role);

            return Ok(_mapper.Map<RoleDTO>(role));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] RoleDTO dTO)
        {
            var role = await _roleManager.FindByIdAsync(dTO.Id);
            if (role is null)
                return NotFound();

            _mapper.Map(dTO, role);
            await _roleManager.UpdateAsync(role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound();

            await _roleManager.DeleteAsync(role);
            return NoContent();
        }
    }
}
