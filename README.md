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
> to get the app runing we can use the docker-compose file located in the root of the solution.
the docker file create a network and run the 3 services in the solution. To run the services we can use the following command:
```bash
docker-compose -f docker-compose.debug.yml build
docker-compose -f docker-compose.debug.yml up -d
```



## Search endpoint in product catalog
Implement a search endpoint in `Nozama.ProductCatalog`
- You should use query strings for search values[^1]
- You decide for what to search for: `Name`, `Description`, `ProductId`, etc. 
- Add a couple of products and test it out!

    
    <details>
        <summary>Code</summary>
            

        ```csharp 

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Product>>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var products = await _dbContext.Products
                .AsNoTracking()
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

            return Ok(products);
        }

        ```

    </details>


## Add a stats endpoint
Implement the following in `Nozama.Recommendations`
- Setup the background worker to poll data from `/stats` from `Nozama.ProductCatalog` and save it to it's database in 1600 milliseconds intervals

    > Added StatsBackgroundworker to fetch data from the product catalog service and save it to the database. [StatsBackgroundWorker](nozama/Nozama.Recommendations/workers/statsBackgroundWorker.cs)
    
- Add a service, that calculate the total number lookups for products (think about which microservice should implement this service)

    > Added an Service to the ``Nozma.ProductCatalog`` and adding an endpoint to the statsController ``/totallookups`` to fetch the total searches(LookUps) for each product .[ProductLookupService](nozama/Nozama.ProductCatalog/Services/ProductLookupService.cs)

- Setup an endpoint that fetches the 100 latest searches ordered from most searched to least searched (you decide where this should go)

    > Added a controller ``SearchController.cs`` that contains the endpoint ``/latestSearches`` to fetch the latest searches for each product. [SearchesController](nozama/Nozama.ProductCatalog/Controllers/SearchesController.cs)




## References
- https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types

[^1]: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding


## Questions

- What are the six characteristics of microservices?
1. The six characteristics of microservices are:
   - **Componentization via Services**: The application is composed of loosely coupled services.
   - **Organized around Business Capabilities**: Each service corresponds to a specific business function or capability.
   - **Products, not Projects**: Services are long-lived and evolve over time.
   - **Smart Endpoints and Dumb Pipes**: Services implement logic while communication between them is kept simple.
   - **Decentralized Data Management**: Each service manages its own database or data storage.
   - **Infrastructure Automation**: Emphasizes the use of automation for deployment, scaling, and management.

- Should you always go with microservices when starting a new project?

    > Whether to use microservices for a new project depends on various factors such as project complexity, team expertise, scalability requirements, and expected changes over time. While microservices offer benefits like scalability and flexibility, they also introduce complexity and overhead. It's crucial to assess these factors and consider if the benefits outweigh the challenges for your specific project.

- How does the .NET Core framework support the implementation of microservices?
    
3. .NET Core framework supports microservices implementation through features such as:
   - **Modularity**: .NET Core allows you to create modular applications, which align with microservices architecture by enabling the development of independent services.
   - **Cross-Platform Compatibility**: .NET Core supports deployment on various platforms, facilitating microservices deployment in diverse environments.
   - **Containerization Support**: .NET Core seamlessly integrates with containerization technologies like Docker, simplifying the deployment and management of microservices.
   - **RESTful APIs**: .NET Core provides robust support for building RESTful APIs, which are commonly used for communication between microservices.

- How do we identify business and technical capabilities?
    > Business capabilities represent the functions or activities a business performs to achieve its objectives. These can be identified through analysis of business processes, value streams, and stakeholder requirements. Technical capabilities, on the other hand, refer to the technological competencies required to support and enable the business capabilities. They include aspects such as software architecture, infrastructure, and integration mechanisms.

- What are the three main communication styles for collaboration between microservices?
    5. The three main communication styles for collaboration between microservices are:
    - **Synchronous Communication**: Services communicate directly with each other via request-response mechanisms like HTTP. This includes RESTful APIs and RPC (Remote Procedure Call).
    - **Asynchronous Communication**: Services communicate indirectly through message brokers or event buses. Examples include using messaging queues or publish-subscribe patterns.
    - **Event-Driven Communication**: Services communicate by reacting to events or changes in state. This style is characterized by event sourcing, where events represent changes in system state and are propagated to interested parties asynchronously.


## Issues log

- Use ```5200/swagger/index.html``` to access the swagger documentation for the product catalog service. 

- the search list returnig empty list when the search term is not found.

- The microsoft.EntityFrameworkCore.Tools.DotNet package is giving error use ```dotnet list package``` to check the version of the package and restore if it fails with the following command ```dotnet restore```

- Scope issue with the recommendationDbContext in the StatsBackgroundWorker class.fix: requires using a factory scope 
- issues with the migration solved with ``dotnet ef migration add <migration name>``  and ``dotnet ef database update``