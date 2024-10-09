namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>
{

};

public record CreateProductResult(Guid Id)
{

};

internal class CreateProductCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Verificación adicional en el CommandHandler
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Product name is required.");
        }
        if (command.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }
        if (command.Category == null || !command.Category.Any())
        {
            throw new ArgumentException("At least one category is required.");
        }

        // Crear entidad del producto a partir del comando
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price,
        };

        // Guardar en la base de datos
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        // Retornar CreateProductResult
        return new CreateProductResult(product.Id);
    }
};
