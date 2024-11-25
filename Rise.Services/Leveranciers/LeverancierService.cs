﻿using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Leveranciers;

namespace Rise.Services.Leveranciers;

public class LeverancierService : ILeverancierService
{
    private readonly ApplicationDbContext dbContext;

    public LeverancierService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<LeverancierDto>> GetLeveranciersAsync()
    {
        IQueryable<LeverancierDto> query = dbContext.Leveranciers.Select(x => new LeverancierDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            Address = x.Address
        });

        var leveranciers = await query.ToListAsync();

        return leveranciers;
    }
}