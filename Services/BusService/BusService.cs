using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
using DB;
using Services.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Azure;


namespace Services.BusService
{
    public class BusService : IBusService
    {
        private readonly DBContext _context;
        public BusService(DBContext context) 
        {
           _context = context;
        }
        public async Task<int> AddBusRecordAsync(UpdateBusDto busdto)
        {
            var newBus = new Bus
            {
                DriverName = busdto.DriverName,
                DriverPhoneNumber = busdto.DriverPhoneNumber,
                BusStopStation = busdto.BusStopStation,
                CarNumber = busdto.CarNumber,
                BusCapacity = busdto.BusCapacity,
                CarModel = busdto.CarModel,
                BusLineStops = busdto.BusLineStops,
                BusType = busdto.BusType,
                CreatedById = 1,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.Buses.AddAsync(newBus);
            await _context.SaveChangesAsync();
            return newBus.Id;

        }
        public async Task<ApiReponse<List<Bus>>> GetBusesAsync(BusFilterDto filterDto)
        {
            var query = _context.Buses.Where(x=>x.IsDeleted==false).AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.DriverName))
            {
                query = query.Where(b => b.DriverName.Contains(filterDto.DriverName) && !b.IsDeleted);
            }

            if (!string.IsNullOrEmpty(filterDto.DriverPhoneNumber))
            {
                query = query.Where(b => b.DriverPhoneNumber.Contains(filterDto.DriverPhoneNumber) && !b.IsDeleted);
            }

            if (!string.IsNullOrEmpty(filterDto.BusStopStation))
            {
                query = query.Where(b => (b.BusStopStation.Contains(filterDto.BusStopStation) ||b.BusLineStops.Contains(filterDto.BusStopStation)) && !b.IsDeleted);
            }

            if (filterDto.CarNumber.HasValue)
            {
                query = query.Where(b => b.CarNumber == filterDto.CarNumber.Value && !b.IsDeleted);
            }

            if (filterDto.BusCapacity.HasValue)
            {
                query = query.Where(b => b.BusCapacity == filterDto.BusCapacity.Value && !b.IsDeleted);
            }

            if (!string.IsNullOrEmpty(filterDto.CarModel))
            {
                query = query.Where(b => b.CarModel.Contains(filterDto.CarModel) && !b.IsDeleted);
            }
            var totalCount = await query.CountAsync();
            var pageNumber = filterDto.PageNumber ?? 1;
            var pageSize = filterDto.PageSize ?? totalCount;
            var buses = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            //return (buses, totalCount);
            return new ApiReponse<List<Bus>>
            {
                Data = buses,
                //Data = totalCount == 1 ? buses : new { items = buses },
                Pagination = new Pagination { CurrentPage = pageNumber, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize), TotalItems = totalCount },
                Message = "Buses retrieved successfully",
                ErrorList = new List<string>()
            };
        }

        public async Task<ApiReponse<BusDto>> GetBusByIdAsync(int id)
        {
            var bus = await _context.Buses
                .Where(b => b.Id == id && !b.IsDeleted)
                .Select(b => new BusDto
                {
                    Id = b.Id,
                    DriverName = b.DriverName,
                    DriverPhoneNumber = b.DriverPhoneNumber,
                    BusStopStation = b.BusStopStation,
                    CarNumber = b.CarNumber,
                    BusCapacity = b.BusCapacity,
                    CarModel = b.CarModel,
                    BusLineStops = b.BusLineStops,
                    BusType = b.BusType,
                    IsDeleted = b.IsDeleted,
                    CreatedOn = b.CreatedOn,
                    CreatedById = b.CreatedById,
                    UpdatedOn = b.UpdatedOn,
                    UpdatedById = b.UpdatedById
                })
                .FirstOrDefaultAsync();

            return new ApiReponse<BusDto>
            {
                Data = bus,
                Message = bus != null ? "Bus found" : "Bus not found",
                ErrorList = bus == null ? new List<string> { "No bus found with the given ID." } : new List<string>()
            };
        }
        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null || bus.IsDeleted)
            {
                return false;
            }

            bus.IsDeleted = true;
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateBusAsync(int id, UpdateBusDto busDto)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return false;
            }

            bus.DriverName = busDto.DriverName;
            bus.DriverPhoneNumber = busDto.DriverPhoneNumber;
            bus.BusStopStation = busDto.BusStopStation;
            bus.CarNumber = busDto.CarNumber;
            bus.BusCapacity = busDto.BusCapacity;
            bus.CarModel = busDto.CarModel;
            bus.UpdatedOn = DateTime.UtcNow;
            bus.UpdatedById = 1;
            bus.BusLineStops = busDto.BusLineStops;
            bus.BusType= busDto.BusType;
            bus.IsDeleted = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ProcessExcelFile(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Assuming row 1 is the header
                    {
                        var entity = new UpdateBusDto
                        {
                            DriverName = worksheet.Cells[row, 1].Value?.ToString(),
                            DriverPhoneNumber = worksheet.Cells[row, 2].Value?.ToString(),
                            BusStopStation = worksheet.Cells[row, 3].Value?.ToString(),
                            CarNumber = Int32.Parse(worksheet.Cells[row, 4].Value.ToString()),
                            BusCapacity = Int32.Parse(worksheet.Cells[row, 5].Value.ToString()),
                            CarModel = worksheet.Cells[row, 6].Value?.ToString(),
                            BusLineStops = worksheet.Cells[row, 7].Value?.ToString(),
                            BusType = worksheet.Cells[row, 8].Value?.ToString()
                        };
                        if (_context.Buses.Where(b => b.CarNumber == entity.CarNumber).Any() == true)
                        {
                            var id = _context.Buses.Where(b => b.CarNumber == entity.CarNumber).First().Id;
                            await UpdateBusAsync(id, entity);
                        }
                        else
                        {
                            await AddBusRecordAsync(entity);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}
