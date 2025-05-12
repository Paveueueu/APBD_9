using Tutorial9.Model;
using Tutorial9.Repositories;

namespace Tutorial9.Services;


public class DbService : IDbService
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public DbService(IConfiguration configuration, IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    public async Task<int> FulfillOrder(OrderFulfillmentDto dto)
    {
        await _warehouseRepository.BeginTransactionAsync();
        
        // Check if product exists
        var productExists = await _warehouseRepository.DoesProductExistAsync(dto.IdProduct);
        if (!productExists)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new KeyNotFoundException("Product does not exist.");
        }
        
        // Check if warehouse exists
        var warehouseExists = await _warehouseRepository.DoesWarehouseExistAsync(dto.IdWarehouse);
        if (!warehouseExists)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new KeyNotFoundException("Warehouse does not exist.");
        }
            
        
        // Check if amount is greater than 0
        var amountCorrect = (dto.Amount > 0);
        if (!amountCorrect)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new ArgumentException("Amount is invalid.");
        }
        
        // Check if order exists and get its orderId
        var orderId = await _warehouseRepository.GetOrderIdAsync(dto.IdProduct, dto.Amount);
        if (orderId == null)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new KeyNotFoundException("Order does not exist.");
        }
        
        // Check if order creation date is before createdAt date
        var orderCreatedAt = await _warehouseRepository.GetOrderCreationDateAsync((int) orderId);
        if (orderCreatedAt > dto.CreatedAt)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new InvalidOperationException("Order creation date is after the provided date.");
        }
        
        // Check if order has already been completed
        var hasOrderBeenCompleted = await _warehouseRepository.HasOrderBeenCompletedAsync((int) orderId);
        if (hasOrderBeenCompleted)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new InvalidOperationException("Order has already been completed.");
        }
        
        // Try to fulfill order and check if it succeeded
        await _warehouseRepository.UpdateOrderFulfillmentDateAsync((int) orderId, dto.CreatedAt);
        var recordId = await _warehouseRepository.InsertRecordIntoProduct_WarehouseAsync((int) orderId, dto.IdProduct, dto.IdWarehouse, dto.Amount, dto.CreatedAt);
        if (recordId == null)
        {
            await _warehouseRepository.RollbackTransactionAsync();
            throw new Exception("Failed to fulfill order.");
        }
        
        await _warehouseRepository.CommitTransactionAsync();
        return (int) recordId;
    }
}


