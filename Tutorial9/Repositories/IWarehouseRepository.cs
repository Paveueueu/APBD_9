using Tutorial9.Model;

namespace Tutorial9.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExistAsync(int productId);
    Task<bool> DoesWarehouseExistAsync(int warehouseId);
    Task<bool> HasOrderBeenCompletedAsync(int idOrder);
    
    Task<int?> GetOrderIdAsync(int productId, int amount);
    Task<DateTime> GetOrderCreationDateAsync(int idOrder);
    
    Task UpdateOrderFulfillmentDateAsync(int idOrder, DateTime fulfillmentDate);
    Task<int> InsertRecordIntoProduct_WarehouseAsync(int idOrder, DateTime createdAt);
    Task<int?> FulfillOrderAsync(int orderIdResult, DateTime now);
    
    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
}