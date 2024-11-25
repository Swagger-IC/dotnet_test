using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Leveranciers;
using Rise.Shared.Products;
using System.Net.Http.Headers;

namespace Rise.Client.Products.Components
{
    public partial class ProductToevoegenKaart
    {
        [Inject] public required IProductService ProductService { get; set; }
        [Inject] public required ILeverancierService LeverancierService { get; set; }
        [Inject] public required NavigationManager NavigationManager { get; set; }

        private readonly CreateProductDto createProductDto = new();

        private IEnumerable<LeverancierDto>? leveranciers;
        private IBrowserFile? selectedFile;

        protected override async Task OnInitializedAsync()
        {
            leveranciers = await LeverancierService.GetLeveranciersAsync();
        }

        private async Task ProductToevoegen()
        {
            try
            {
                if (selectedFile != null)
                {
                    var imageUrl = await UploadImageAsync(selectedFile);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        createProductDto.ImgUrl = imageUrl;
                    }
                    else
                    {                        
                        return; 
                    }
                }

                var success = await ProductService.CreateProductAsync(createProductDto);
                NavigationManager.NavigateTo("/");

                if (success)
                {
                    // Navigeren naar de productlijst
                    
                }
                else
                {
                    return;
                }
               
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
            }
          

        }

        private async Task<string> UploadImageAsync(IBrowserFile file)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(streamContent, "image", file.Name);

                var response = await ProductService.UploadImageAsync(content);

                if (response.IsSuccessStatusCode)
                {
                    var imageUrl = await response.Content.ReadAsStringAsync();
                    return imageUrl;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception during image upload: {ex.Message}");
                return string.Empty;
            }
        }

        private void HandleImageSelection(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
        }
    }


}

