# PayDotNet - Payments engine for ASP.NET Core

> 👷‍♀️👷‍♂️ Work in Progress

A Future NuGet package inspired by the Ruby on Rails Gem: [Pay](https://github.com/pay-rails/pay)

## Architecture

This package is inspired by the design of [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-7.0) and gives us the most extendability for features.

> Managers are high-level classes which an app developer uses to perform operations, such as creating an Identity user. Stores are lower-level classes that specify how entities, such as users and roles, are persisted. Stores follow the repository pattern and are closely coupled with the persistence mechanism. Managers are decoupled from stores, which means you can replace the persistence mechanism without changing your application code (except for configuration).

Thus you could have the following top-down architecture.

- ASP.NET Core App
- Billable managers. Example: BillableManager, CustomerManager, SubscriptionManager
- Billable **Stores**. Example: CustomerStore, SubscriptionStore
- Data Access Layer
- Data Source. Example: SQL Server Database, Azure Table Storage

## Roadmap

| Feature | Description | Milestone | Status |
|-----------|-----------|-----------|--------|
| **[Stripe](https://stripe.com/docs)** Payment Processor  | We start the initial concept by implementing Stripe as our first PaymentProcessor.  | MVP | 🟢 (Testing) |
| EntityFramework | For the inital storage, we learn from ASP.NET Core identity and implement a basic store using Entity Framework. This gives us the benefit of having a lot of Database Providers already available out of the box. Initial tutorials will cover SqlServer, SqlLite, InMemory and Cosmos. | MVP | 🟡 |
| **Fake** payment processor | A fake payment processor to make testing easier. All the Manager API's are usable and you can thus use it to charge, and subscribe users without accounts in either of the payment processors. | MVP | 🔴 |
| **[Braintree](https://developer.paypal.com/braintree/docs)** | The second payment processor that is implemented by PayPal | vNext | 🔴 |
| API controllers | The following routes should be available out of the box or if the integrator opts-in through service registration. Additionally it should cover authorization scenarios while allow integrators to customize the controllers.<ul><li>`/api/pay/{processorName}/customers`</li><li>`/api/pay/{processorName}/customers/subscriptions`</li><li>`/api/pay/{processorName}/customers/charges`</li><li>`/api/pay/{processorName}/customers/paymentmethods`</li><li>`/api/pay/{processorName}/merchants`</li></ul> | vNext | 🔴 |
| TBA | - | - | - |

