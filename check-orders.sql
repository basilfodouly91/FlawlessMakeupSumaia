-- Check if any orders exist in the database
SELECT 
    Id,
    OrderNumber,
    UserId,
    GuestEmail,
    GuestName,
    OrderDate,
    Status,
    TotalAmount,
    ShippingFirstName,
    ShippingLastName,
    ShippingPhone,
    ShippingCity
FROM Orders
ORDER BY OrderDate DESC;

-- Show count
SELECT COUNT(*) as TotalOrders FROM Orders;

-- Check order items
SELECT 
    oi.Id,
    oi.OrderId,
    o.OrderNumber,
    oi.ProductName,
    oi.Quantity,
    oi.UnitPrice,
    oi.TotalPrice
FROM OrderItems oi
INNER JOIN Orders o ON oi.OrderId = o.Id
ORDER BY o.OrderDate DESC;

