﻿using DB.Models;
using Microsoft.AspNetCore.Http;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BusService
{
    public interface IBusService
    {
        Task<int> AddBusRecordAsync(UpdateBusDto busdto);
        Task<ApiReponse<List<Bus>>> GetBusesAsync(BusFilterDto filterDto);
        Task<ApiReponse<BusDto>> GetBusByIdAsync(int id);
        Task<bool> DeleteBusAsync(int id);
        Task<bool> UpdateBusAsync(int id, UpdateBusDto busDto);
        Task<bool> ProcessExcelFile(IFormFile file);
    }
}
