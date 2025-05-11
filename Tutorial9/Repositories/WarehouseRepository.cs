using Microsoft.Data.SqlClient;

namespace Tutorial9.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> DoesProductExist(int productId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT COUNT(1) FROM Product WHERE IdProduct = @productId", connection);
        command.Parameters.AddWithValue("@productId", productId);
        return (int)await command.ExecuteScalarAsync() > 0;
    }

    public async Task<bool> DoesWarehouseExist(int warehouseId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @warehouseId", connection);
        command.Parameters.AddWithValue("@warehouseId", warehouseId);
        return (int)await command.ExecuteScalarAsync() > 0;
    }

    public async Task<bool> DoesOrderExist(int productId, int amount)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(
            "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @productId AND Amount = @amount",
            connection);
        command.Parameters.AddWithValue("@productId", productId);
        command.Parameters.AddWithValue("@amount", amount);
        return await command.ExecuteScalarAsync() != null;
    }

    public async Task<bool> DoesOrderExistInProduct_Warehouse(int idOrder)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @idOrder", connection);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        return (int)await command.ExecuteScalarAsync() > 0;
    }

    public async Task UpdateOrderFulfillmentDate(int idOrder)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("UPDATE [Order] SET FulfilledAt = @now WHERE IdOrder = @idOrder", connection);
        command.Parameters.AddWithValue("@now", DateTime.UtcNow);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertRecordIntoProduct_Warehouse(int idOrder)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Pobierz IdProduct i Amount z zamówienia
        var (productId, amount) = await GetOrderDetails(connection, idOrder);
        var price = await GetProductPrice(connection, productId);

        // Wstaw rekord do Product_Warehouse (IdWarehouse ustawione na 0 tymczasowo)
        var command = new SqlCommand(@"
                INSERT INTO Product_Warehouse 
                    (IdOrder, IdProduct, IdWarehouse, Amount, Price, CreatedAt) 
                OUTPUT INSERTED.IdProduct_Warehouse
                VALUES (@idOrder, @productId, @idWarehouse, @amount, @price, @createdAt)", connection);

        command.Parameters.AddWithValue("@idOrder", idOrder);
        command.Parameters.AddWithValue("@productId", productId);
        command.Parameters.AddWithValue("@idWarehouse", 0); // Tymczasowe rozwiązanie
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@price", price * amount);
        command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

        return (int) await command.ExecuteScalarAsync();
    }

    private async Task<(int productId, int amount)> GetOrderDetails(SqlConnection connection, int idOrder)
    {
        var command = new SqlCommand("SELECT IdProduct, Amount FROM [Order] WHERE IdOrder = @idOrder", connection);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync()
            ? (reader.GetInt32(0), reader.GetInt32(1))
            : throw new Exception("Order not found");
    }

    private async Task<decimal> GetProductPrice(SqlConnection connection, int productId)
    {
        var command = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @productId", connection);
        command.Parameters.AddWithValue("@productId", productId);
        return (decimal) await command.ExecuteScalarAsync();
    }
}