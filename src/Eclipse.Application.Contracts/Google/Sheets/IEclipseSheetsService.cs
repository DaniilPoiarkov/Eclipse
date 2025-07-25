﻿namespace Eclipse.Application.Contracts.Google.Sheets;

public interface IEclipseSheetsService<TObject>
{
    Task<IEnumerable<TObject>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TObject value, CancellationToken cancellationToken = default);
}
