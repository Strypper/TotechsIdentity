using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Threading.Tasks;
using TotechsIdentity.DataObjects;

namespace TotechsIdentity;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : BaseController
{
    #region [Fields]
    private readonly IMapper _mapper;
    private readonly UserManager _userManager;
    #endregion

    #region [CTor]
    public UserController(IMapper mapper,
                          UserManager userManager)
    {
        this._mapper = mapper;
        this._userManager = userManager;
    }
    #endregion

    #region [Controllers]
    [HttpGet]
    public async Task<IActionResult> Get(string guid)
    {
        var user = await _userManager.FindByGuidAsync(guid);
        if (user is null)
            return BadRequest(new { message = $"Can't find this user based on given guid: {guid}" });

        return Ok(_mapper.Map<UserDTO>(user));
    }
    #endregion
}