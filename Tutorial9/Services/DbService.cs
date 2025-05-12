using Tutorial9.Model;
using Tutorial9.Repositories;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;
    private readonly IWarehouseRepository _warehouseRepository;
    
    public DbService(IConfiguration configuration, IWarehouseRepository warehouseRepository)
    {
        _configuration = configuration;
        _warehouseRepository = warehouseRepository;
    }
    
    public async Task<int> FulfillOrder(OrderFulfillmentDto dto)
    {
        await _warehouseRepository.BeginTransactionAsync();
        
        // Check if product exists
        var productExists = await _warehouseRepository.DoesProductExistAsync(dto.IdProduct);
        if (!productExists)
            throw new InvalidOperationException("Product does not exist.");
        
        // Check if warehouse exists
        var warehouseExists = await _warehouseRepository.DoesWarehouseExistAsync(dto.IdWarehouse);
        if (!warehouseExists)
            throw new InvalidOperationException("Warehouse does not exist.");
        
        // Check if amount is greater than 0
        var amountCorrect = (dto.Amount > 0);
        if (!amountCorrect)
            throw new ArgumentOutOfRangeException(nameof(dto.Amount));
        
        // Check if order exists and get its orderId
        var orderId = await _warehouseRepository.GetOrderIdAsync(dto.IdProduct, dto.Amount);
        if (orderId == null)
            throw new InvalidOperationException("Order does not exist.");
        
        // Check if order creation date is before createdAt date
        var orderCreatedAt = await _warehouseRepository.GetOrderCreationDateAsync((int) orderId);
        if (orderCreatedAt < dto.CreatedAt)
            throw new InvalidOperationException("Order creation date is before the provided date.");
        
        // Check if order has already been completed
        var hasOrderBeenCompleted = await _warehouseRepository.HasOrderBeenCompletedAsync((int) orderId);
        if (hasOrderBeenCompleted)
            throw new InvalidOperationException("Order has already been completed.");
        
        // Try to fulfill order and check if it succeeded
        var recordId = await _warehouseRepository.FulfillOrderAsync((int) orderId, DateTime.Now);
        if (recordId == null)
            throw new Exception("Failed to fulfill order.");
        
        await _warehouseRepository.CommitTransactionAsync();
        return (int) recordId;
    }
}