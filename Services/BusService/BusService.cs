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


namespace Services.BusService
{
    public class BusService : IBusService
    {
        private readonly DBContext _context;
        public BusService(DBContext context) 
        {
           _context = context;
        }
        public async Task<int> AddBusRecordAsync(CreateBusDto busdto)
        {
            var newBus = new Bus
            {
                // Id = _context.Buses.Count() > 0 ? _context.Buses.Max(b => b.Id) + 1 : 1,
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
        public async Task<IEnumerable<Bus>> GetBusesAsync(BusFilterDto filterDto)
        {
            var query = _context.Buses.AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.DriverName))
            {
                query = query.Where(b => b.DriverName.Contains(filterDto.DriverName));
            }

            if (!string.IsNullOrEmpty(filterDto.DriverPhoneNumber))
            {
                query = query.Where(b => b.DriverPhoneNumber.Contains(filterDto.DriverPhoneNumber));
            }

            if (!string.IsNullOrEmpty(filterDto.BusStopStation))
            {
                query = query.Where(b => b.BusStopStation.Contains(filterDto.BusStopStation));
            }

            if (filterDto.CarNumber.HasValue)
            {
                query = query.Where(b => b.CarNumber == filterDto.CarNumber.Value);
            }

            if (filterDto.BusCapacity.HasValue)
            {
                query = query.Where(b => b.BusCapacity == filterDto.BusCapacity.Value);
            }

            if (!string.IsNullOrEmpty(filterDto.CarModel))
            {
                query = query.Where(b => b.CarModel.Contains(filterDto.CarModel));
            }

            return await query.ToListAsync();
        }
        public async Task<BusDto> GetBusByIdAsync(int id)
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

            return bus;
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

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> FetchAndUpdateAsync(int id, UpdateBusDto busdto)
        {
            var bus = await GetBusByIdAsync(id);
            if(bus == null)
            {
                return false;
            }
            var update = await UpdateBusAsync(id, busdto);
            if(!update)
                return false;
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
                        var entity = new CreateBusDto
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
                        await AddBusRecordAsync(entity);
                        //var entity = new Bus
                        //{
                        //    DriverName = worksheet.Cells[row, 1].Value?.ToString(),
                        //    DriverPhoneNumber = worksheet.Cells[row, 2].Value?.ToString(),
                        //    BusStopStation = worksheet.Cells[row, 3].Value?.ToString(),
                        //    CarNumber = Int32.Parse(worksheet.Cells[row, 4].Value.ToString()),
                        //    BusCapacity = Int32.Parse(worksheet.Cells[row, 5].Value.ToString()),
                        //    CarModel = worksheet.Cells[row, 6].Value?.ToString(),
                        //    BusLineStops = worksheet.Cells[row, 7].Value?.ToString(),
                        //    BusType = worksheet.Cells[row, 8].Value?.ToString(),
                        //    IsDeleted = Boolean.Parse(worksheet.Cells[row, 9].Value?.ToString()),
                        //    CreatedOn = DateTime.Parse(worksheet.Cells[row, 10].Value?.ToString()),
                        //    CreatedById = Int32.Parse(worksheet.Cells[row, 11].Value?.ToString()),
                        //    UpdatedOn = DateTime.Parse(worksheet.Cells[row, 12].Value?.ToString()),
                        //    UpdatedById = Int32.Parse(worksheet.Cells[row, 13].Value?.ToString())
                        //    //UpdatedOn = worksheet.Cells[row, 12].Value?.ToString() == "NULL" ? null : DateTime.Parse(worksheet.Cells[row, 12].Value?.ToString()),
                        //    //UpdatedById = worksheet.Cells[row, 13].Value?.ToString() == "NULL" ? null : Int32.Parse(worksheet.Cells[row, 13].Value?.ToString())

                        //};
                        //_context.Buses.Add(entity);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}
