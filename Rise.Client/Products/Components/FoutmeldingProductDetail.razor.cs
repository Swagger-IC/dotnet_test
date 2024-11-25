using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;

namespace Rise.Client.Products.Components;

public partial class FoutmeldingProductDetail
{
    [Parameter] public required int InStock { get; set; }
    [Parameter] public required int Amount { get; set; }
}