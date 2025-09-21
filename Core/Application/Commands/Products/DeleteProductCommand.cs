using MediatR;

namespace Case_1_2.Core.Application.Commands.Products
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
