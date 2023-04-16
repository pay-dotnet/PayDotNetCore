# PayDotNet - Payments engine for ASP.NET Core

> 👷‍♀️👷‍♂️ Work in Progress

A Future NuGet package inspired by the Ruby on Rails Gem: [Pay](https://github.com/pay-rails/pay)

## Roadmap

1. Stripe
2. Fake processor
3. Braintree
4. Paddle

## Architecture

The architecture might look like the following:

- ASP.NET Core App
- Billable / Pay **Managers**
- Billable **Store**.  Example: CustomerManager, SubscriptionManager
- Data Access Layer.
- Data Source. Example: SQL Server Database, Azure Table Storage

This is a similiar approach to [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-7.0) and gives us the most extendability for features.

> Managers are high-level classes which an app developer uses to perform operations, such as creating an Identity user. Stores are lower-level classes that specify how entities, such as users and roles, are persisted. Stores follow the repository pattern and are closely coupled with the persistence mechanism. Managers are decoupled from stores, which means you can replace the persistence mechanism without changing your application code (except for configuration).