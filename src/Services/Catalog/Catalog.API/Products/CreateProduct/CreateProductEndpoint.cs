namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    {

    };

    public record CreateProductResponse(Guid Id)
    {

    };
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products",
                async (CreateProductRequest request, ISender sender) =>
                {
                    // Verificación de entrada en el endpoint
                    if (string.IsNullOrWhiteSpace(request.Name))
                    {
                        return Results.BadRequest("Product name is required.");
                    }
                    if (request.Price <= 0)
                    {
                        return Results.BadRequest("Price must be greater than zero.");
                    }
                    if (request.Category == null || !request.Category.Any())
                    {
                        return Results.BadRequest("At least one category is required.");
                    }

                    var command = request.Adapt<CreateProductCommand>();

                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateProductResponse>();

                    return Results.Created($"/products/{response.Id}", response);

                })
                .WithName("CreateProduct")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .WithSummary("Create Product")
                .WithDescription("Create Product");
        }
    };
}
