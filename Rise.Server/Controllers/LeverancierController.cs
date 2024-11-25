using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Leveranciers;

namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeverancierController : ControllerBase
    {
        private readonly ILeverancierService leverancierService;
        
        public LeverancierController (ILeverancierService leverancierService)
        {
            this.leverancierService = leverancierService;
        }

        [HttpGet]
        public async Task<IEnumerable<LeverancierDto>> Get()
        {
            var leveranciers = await leverancierService.GetLeveranciersAsync();
            return leveranciers;
        }
    }
}
