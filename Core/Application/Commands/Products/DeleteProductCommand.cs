using MediatR;

namespace Case_1.Core.Application.Commands.Products
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        
        public DeleteProductCommand(int id)
        {
            Id = id;
        }
    }
}
