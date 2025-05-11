using Tutorial9.Model;

namespace Tutorial9.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExist(int productId);
    Task<bool> DoesWarehouseExist(int warehouseId);
    Task<bool> DoesOrderExist(int productId, int amount);
    Task<bool> DoesOrderExistInProduct_Warehouse(int idOrder);
    
    Task UpdateOrderFulfillmentDate(int idOrder);
    Task<int> InsertRecordIntoProduct_Warehouse(int idOrder);
}