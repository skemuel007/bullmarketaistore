# Application Layer Design Patterns Implementation & Guide

This document details the software design patterns implemented within `BullMarket.Application` and `BullMarket.Domain`.

---

## 1. Strategy Pattern for Shipping & Pricing Calculations [IMPLEMENTED]

- **Interfaces**: `IShippingStrategy`, `ITaxStrategy` in [PricingStrategies.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Orders/Strategies/PricingStrategies.cs)
- **Implementations**:
  - `TieredShippingStrategy`: Free shipping when cart subtotal meets/exceeds `CommerceOptions.FreeShippingThreshold`; standard shipping fee otherwise.
  - `StandardTaxStrategy`: Rounded tax calculation using `CommerceOptions.TaxRate`.
- **Usage**: Injected directly into [OrderService.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Orders/OrderService.cs), allowing pluggable algorithms without modifying order creation logic.
- **Unit Tests**: [PricingStrategyTests.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/tests/BullMarket.Application.Tests/Orders/PricingStrategyTests.cs).

---

## 2. Specification Pattern for Catalog Queries [IMPLEMENTED]

- **Core Infrastructure**: [Specification.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Common/Specifications/Specification.cs)
  - Provides composable LINQ expressions via `.And()`, `.Or()`, and `.Not()`, as well as in-memory evaluation (`IsSatisfiedBy`).
- **Domain Specifications**: [ProductSpecifications.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Catalog/Specifications/ProductSpecifications.cs)
  - `ActiveProductSpecification`: Filters active catalog items.
  - `ProductPriceRangeSpecification`: Boundary checks for minimum and maximum price.
  - `ProductHardwareSpecification`: RAM/VRAM constraints.
  - `ProductCapabilitiesSpecification`: Flags for AI-ready, LLM-ready, developer-ready, etc.
- **Unit Tests**: [SpecificationTests.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/tests/BullMarket.Application.Tests/Catalog/SpecificationTests.cs).

---

## 3. Domain Events Pattern [IMPLEMENTED]

- **Core Infrastructure**:
  - [IDomainEvent.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Domain/Common/IDomainEvent.cs) (`IDomainEvent`, `IHasDomainEvents`)
  - [IDomainEventDispatcher.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Abstractions/IDomainEventDispatcher.cs) (`IDomainEventHandler<TEvent>`, `IDomainEventDispatcher`)
  - [DomainEventDispatcher.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Common/DomainEventDispatcher.cs) (Resolves and invokes matching handlers dynamically)
- **Concrete Events & Handlers**:
  - `OrderCreatedDomainEvent`, `OrderCancelledDomainEvent` in [OrderEvents.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Domain/Orders/OrderEvents.cs)
  - [OrderCreatedEventHandler.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Orders/OrderCreatedEventHandler.cs)
- **Integration**:
  - [Order.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Domain/Orders/Order.cs) collects domain events internally.
  - [OrderService.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/backend/src/BullMarket.Application/Orders/OrderService.cs) dispatches events post-commit.
- **Unit Tests**: [DomainEventDispatcherTests.cs](file:///Users/skemuel007/projects/personal/dotnet/bullmarketaistore/tests/BullMarket.Application.Tests/DomainEventDispatcherTests.cs).

---

## 4. CQRS & Mediator Pattern [CANDIDATE FOR EXPANSION]

- Split services into separate command handlers (`CreateOrderCommandHandler`, `AddCartItemCommandHandler`) and query handlers (`GetProductsQueryHandler`, `GetCartQueryHandler`).

---

## 5. Decorator / Pipeline Behaviors [CANDIDATE FOR EXPANSION]

- Automatic execution of validation and OpenTelemetry performance tracking wrapping handlers.

