using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.OrderDTOs;

public class OrderMessageProductDTO
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
}
