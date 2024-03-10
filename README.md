# Exercise
We're going to make our own implementation of the webshop described in the book, that'll consist of the following services:
- A product catalog service
- A shopping cart service
- A recommendation service

We've create a scaffold for you to get started, everything you need to get going is located @ https://gitlab.au.dk/swwao/nozama.

The solution holds the following projects, which represents a microservice in the system:
- `Nozama.ProductCatalog` - a .NET MVC project that exposes two endpoints: `/stats` and `/catalog`.
- `Nozama.Recommendations`– a .NET Minimal API project that contains a background service that polls data from `Nozama.ProductCatalog`  

## Get everything up and running
This first thing to do is to check the log: Why are `Nozama.Recommendations` throwing exceptions, and how can we fix this? (spoiler alert: We'll dive deeper in the next lecture, so just hack it for now)

## Search endpoint in product catalog
Implement a search endpoint in `Nozama.ProductCatalog`
- You should use query strings for search values[^1]
- You decide for what to search for: `Name`, `Description`, `ProductId`, etc. 
- Add a couple of products and test it out!

## Add a stats endpoint
Implement the following in `Nozama.Recommendations`
- Setup the background worker to poll data from `/stats` from `Nozama.ProductCatalog` and save it to it's database in 1600 milliseconds intervals
- Add a service, that calculate the total number lookups for products (think about which microservice should implement this service)
- Setup an endpoint that fetches the 100 latest searches ordered from most searched to least searched (you decide where this should go)

## References
- https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types

[^1]: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding
