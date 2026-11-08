# Class Diagrams & Architecture

---

## 1. Clean Architecture Dependencies

Dependencies point strictly inward toward Domain:

```mermaid
classDiagram
    direction BT

    class Domain {
        <<Core>>
        Entities
        Value Objects
        Domain Exceptions
        Guard Clauses
    }

    class Application {
        <<Use Cases>>
        Service Interfaces
        Service Implementations
        DTOs and Contracts
        Validators
        Repository Abstractions
    }

    class Infrastructure {
        <<Data & External>>
        DbContext
        EF Core Configurations
        Repository Implementations
        UnitOfWork
        Migrations & Seeding
    }

    class Api {
        <<Composition Root>>
        Minimal API Endpoints
        Exception Handling
        Filters
        Configuration & Scalar Docs
    }

    Application ..> Domain : References
    Infrastructure ..> Application : References
    Api ..> Application : References
    Api ..> Infrastructure : References
```

---

## 2. Domain Model Class Diagram

Aggregates, entities, and value objects within `BullMarket.Domain`:

```mermaid
classDiagram
    direction TB

    class Customer {
        +Guid Id
        +string FirstName
        +string LastName
        +string Email
        +string Phone
        +Address ShippingAddress
        +Address BillingAddress
        +DateTimeOffset CreatedAt
        +SetShippingAddress(Address)
        +SetBillingAddress(Address)
    }

    class Address {
        <<Value Object>>
        +string RecipientName
        +string Line1
        +string Line2
        +string City
        +string StateOrProvince
        +string PostalCode
        +string CountryCode
        +string Phone
    }

    class Cart {
        +Guid Id
        +Guid CustomerId
        +DateTimeOffset CreatedAt
        +DateTimeOffset UpdatedAt
        +IReadOnlyList~CartItem~ Items
        +decimal Subtotal
        +AddItem(...)
        +UpdateItem(...)
        +RemoveItem(...)
        +Clear(...)
    }

    class CartItem {
        +Guid Id
        +Guid CartId
        +Guid ProductId
        +string Sku
        +string ProductName
        +decimal UnitPrice
        +string Currency
        +int Quantity
        +decimal LineTotal
    }

    class Order {
        +Guid Id
        +string OrderNumber
        +Guid CustomerId
        +DateTimeOffset OrderDate
        +Address ShippingAddress
        +Address BillingAddress
        +decimal Subtotal
        +decimal ShippingAmount
        +decimal TaxAmount
        +decimal TotalAmount
        +string Currency
        +OrderStatus OrderStatus
        +PaymentStatus PaymentStatus
        +DateTimeOffset CancelledAt
        +uint Version
        +IReadOnlyCollection~OrderItem~ Items
        +Confirm()
        +StartProcessing()
        +MarkShipped()
        +MarkDelivered()
        +MarkReturned()
        +Cancel(DateTimeOffset)
    }

    class OrderItem {
        +Guid Id
        +Guid OrderId
        +Guid ProductId
        +string Sku
        +string ProductName
        +decimal UnitPrice
        +int Quantity
        +decimal LineTotal
    }

    class Product {
        +Guid Id
        +string Sku
        +string Name
        +string Slug
        +string Brand
        +Guid CategoryId
        +string ShortDescription
        +string Description
        +decimal Price
        +string Currency
        +bool IsActive
        +bool IsFeatured
        +DateTimeOffset CreatedAt
        +DateTimeOffset UpdatedAt
        +IReadOnlyCollection~ProductImage~ Images
        +IReadOnlyCollection~ProductSpecification~ Specifications
        +IReadOnlyCollection~ProductTags~ Tags
        +SetTechnicalSpecifications(...)
        +SetUsageClassification(...)
        +AddImage(...)
        +AddSpecification(...)
        +AddTag(...)
    }

    class Category {
        +Guid Id
        +string Name
        +string Slug
        +string Description
    }

    class InventoryItem {
        +Guid Id
        +Guid ProductId
        +int QuantityOnHand
        +int QuantityReserved
        +int QuantityAvailable
        +DateTimeOffset UpdatedAt
        +uint Version
        +Reserve(int, DateTimeOffset)
        +Release(int, DateTimeOffset)
        +CommitReservation(int, DateTimeOffset)
        +Reduce(int, DateTimeOffset)
        +Restock(int, DateTimeOffset)
    }

    Customer *-- Address : owns
    Cart *-- CartItem : contains
    Order *-- OrderItem : contains
    Order *-- Address : owns
    Product --> Category : belongs to
    Product ..> InventoryItem : tracked by
```

---

## 3. Application Services & Persistence Abstractions

```mermaid
classDiagram
    direction LR

    class IProductService {
        <<Interface>>
        +GetProductsAsync(...)
        +GetProductByIdAsync(...)
        +GetCategoriesAsync(...)
    }

    class ICartService {
        <<Interface>>
        +GetCartAsync(...)
        +AddItemAsync(...)
        +UpdateItemAsync(...)
        +RemoveItemAsync(...)
        +ClearAsync(...)
    }

    class IOrderService {
        <<Interface>>
        +CreateOrderAsync(...)
        +GetOrderAsync(...)
        +GetOrdersByCustomerAsync(...)
        +CancelOrderAsync(...)
    }

    class IUnitOfWork {
        <<Interface>>
        +SaveChangesAsync(...)
        +ExecuteInTransactionAsync(...)
    }

    class ICartRepository {
        <<Interface>>
    }

    class IProductRepository {
        <<Interface>>
    }

    class IOrderRepository {
        <<Interface>>
    }

    class IInventoryRepository {
        <<Interface>>
    }

    CartService ..|> ICartService : implements
    OrderService ..|> IOrderService : implements
    ProductService ..|> IProductService : implements

    CartService --> ICartRepository : uses
    CartService --> IProductRepository : uses
    CartService --> IInventoryRepository : uses
    CartService --> IUnitOfWork : uses

    OrderService --> IOrderRepository : uses
    OrderService --> ICartRepository : uses
    OrderService --> IInventoryRepository : uses
    OrderService --> IUnitOfWork : uses
```
