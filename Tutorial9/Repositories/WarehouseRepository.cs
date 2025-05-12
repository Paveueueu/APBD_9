using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Tutorial9.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;
    private SqlConnection _connection;
    private SqlCommand _command;
    private DbTransaction _transaction;

    public WarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    
    
    public async Task BeginTransactionAsync()
    {
        _connection = new SqlConnection(_connectionString);
        _command = new SqlCommand();
        
        _command.Connection = _connection;
        await _connection.OpenAsync();

        _transaction = await _connection.BeginTransactionAsync();
        _command.Transaction = _transaction as SqlTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        await _connection.CloseAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        await _connection.CloseAsync();
    }
    
    

    public async Task<bool> DoesProductExistAsync(int productId)
    {
        _command.CommandText = "SELECT COUNT(1) FROM Product WHERE IdProduct = @IdProduct";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@IdProduct", productId);

        var result = (int) (await _command.ExecuteScalarAsync() ?? throw new Exception());
        return result > 0;
    }

    public async Task<bool> DoesWarehouseExistAsync(int warehouseId)
    {
        _command.CommandText = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@IdWarehouse", warehouseId);

        var result = (int) (await _command.ExecuteScalarAsync() ?? throw new Exception());
        return result > 0;
    }

    public async Task<bool> HasOrderBeenCompletedAsync(int idOrder)
    {
        _command.CommandText = "SELECT COUNT(1) FROM [Order] WHERE IdOrder = @IdOrder AND FulfilledAt IS NOT NULL";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@IdOrder", idOrder);

        var result = (int) (await _command.ExecuteScalarAsync() ?? throw new Exception());
        return result > 0;
    }
    
    

    public async Task<int?> GetOrderIdAsync(int productId, int amount)
    {
        _command.CommandText = """
                               SELECT IdOrder FROM [Order] 
                               WHERE IdProduct = @ProductId AND Amount = @Amount
                               """;
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@ProductId", productId);
        _command.Parameters.AddWithValue("@Amount", amount);
    
        var result = await _command.ExecuteScalarAsync();
        return result != DBNull.Value ? (int?) result : null;
    }

    public async Task<DateTime> GetOrderCreationDateAsync(int idOrder)
    {
        _command.CommandText = "SELECT CreatedAt FROM [Order] WHERE IdOrder = @IdOrder";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@IdOrder", idOrder);

        var result = await _command.ExecuteScalarAsync();
        return (DateTime) (result ?? throw new Exception());
    }
    
    

    public async Task UpdateOrderFulfillmentDateAsync(int idOrder, DateTime fulfillmentDate)
    {
        _command.CommandText = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@FulfilledAt", fulfillmentDate);
        _command.Parameters.AddWithValue("@IdOrder", idOrder);

        await _command.ExecuteNonQueryAsync();
    }

    public async Task<int?> InsertRecordIntoProduct_WarehouseAsync(int idOrder, int productId, int warehouseId, int amount, DateTime createdAt)
    {
        _command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @ProductId";
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@ProductId", productId);
        var price = (decimal?) await _command.ExecuteScalarAsync();
        if (price == null)
            return null;
        
        _command.CommandText = """
                               INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                               OUTPUT INSERTED.IdProductWarehouse
                               VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)
                               """;
        _command.Parameters.Clear();
        _command.Parameters.AddWithValue("@IdWarehouse", warehouseId);
        _command.Parameters.AddWithValue("@IdProduct", productId);
        _command.Parameters.AddWithValue("@IdOrder", idOrder);
        _command.Parameters.AddWithValue("@Amount", amount);
        _command.Parameters.AddWithValue("@Price", price * amount);
        _command.Parameters.AddWithValue("@CreatedAt", createdAt);
    
        return (int?) await _command.ExecuteScalarAsync();
    }
}