using Microsoft.Data.SqlClient;

namespace Tutorial9.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }


    public async Task<bool> DoesProductExistAsync(int productId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DoesWarehouseExistAsync(int warehouseId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasOrderBeenCompletedAsync(int idOrder)
    {
        throw new NotImplementedException();
    }

    public async Task<int?> GetOrderIdAsync(int productId, int amount)
    {
        throw new NotImplementedException();
    }

    public async Task<DateTime> GetOrderCreationDateAsync(int idOrder)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateOrderFulfillmentDateAsync(int idOrder, DateTime fulfillmentDate)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InsertRecordIntoProduct_WarehouseAsync(int idOrder, DateTime createdAt)
    {
        throw new NotImplementedException();
    }

    public async Task<int?> FulfillOrderAsync(int orderIdResult, DateTime now)
    {
        throw new NotImplementedException();
    }

    public async Task BeginTransactionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CommitTransactionAsync()
    {
        throw new NotImplementedException();
    }
}