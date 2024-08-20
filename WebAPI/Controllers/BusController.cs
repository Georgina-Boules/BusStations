using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.BusService;
using Services.DTOs;
using DB.Models;
namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BusController : Controller
    {
        private readonly IBusService _busService;
        public BusController(IBusService busService) { 
            _busService = busService;
        }
        [HttpPost]
        public async Task<IActionResult> AddBus(CreateBusDto busDto)
        {
            if(busDto == null)
                return BadRequest("Bus data is Null");
            var id = await _busService.AddBusRecordAsync(busDto);
            return Ok(id);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetFilteredBuses([FromQuery] BusFilterDto filterDto)
        {
            if (filterDto == null)
            {
                return BadRequest("Filter criteria are missing.");
            }
            ApiReponse<List<Bus>> response = await _busService.GetBusesAsync(filterDto);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusById(int id)
        {
            ApiReponse<BusDto> bus = await _busService.GetBusByIdAsync(id);
            if (bus == null)
            {
                return NotFound($"Bus with ID {id} not found");
            }

            return Ok(bus);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            bool isDeleted = await _busService.DeleteBusAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Bus with ID {id} not found");
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] UpdateBusDto updateBusDto)
        {
            if (updateBusDto == null)
            {
                return BadRequest("Bus data is null");
            }

            bool isUpdated = await _busService.UpdateBusAsync(id, updateBusDto);
            if (!isUpdated)
            {
                return NotFound($"Bus with ID {id} not found");
            }

            return NoContent();
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = await _busService.ProcessExcelFile(file);
            return Ok(result);
        }

    }
}
