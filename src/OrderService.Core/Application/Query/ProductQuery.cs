using MediatR;
using OrderService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Application.Query
{
    public sealed record GetProductByNameQuery(string ProductName) : IRequest<Product>;
}
