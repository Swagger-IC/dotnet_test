using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Services;

namespace Rise.Client.Products.Components;

public partial class Paginatie
{
    [Inject] public required ProductenlijstStatus ProductenlijstStatus { get; set; }
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    [Parameter] public EventCallback<int> OnItemsPerPageChanged { get; set; }

    private async Task GoToPreviousPage()
    {
        if (ProductenlijstStatus.Paginanummer > 1)
        {
            ProductenlijstStatus.Paginanummer--;
            await OnPageChanged.InvokeAsync(ProductenlijstStatus.Paginanummer);
        }
    }

    private async Task GoToNextPage()
    {
        if (ProductenlijstStatus.Paginanummer < ProductenlijstStatus.TotalPages)
        {
            ProductenlijstStatus.Paginanummer++;
            await OnPageChanged.InvokeAsync(ProductenlijstStatus.Paginanummer);
        }
    }

    private async Task GoToPage(int page)
    {
        ProductenlijstStatus.Paginanummer = page;
        await OnPageChanged.InvokeAsync(page);
    }

    private async Task OnItemsPerPageChange(ChangeEventArgs e)
    {
        ProductenlijstStatus.Aantal = int.Parse(e.Value!.ToString()!);
        ProductenlijstStatus.Paginanummer = 1;
        await OnItemsPerPageChanged.InvokeAsync(ProductenlijstStatus.Aantal);
    }
}


