using Rise.Shared.Leveranciers;
using System.Net.Http.Json;

namespace Rise.Client.Products.Services
{
    public class LeverancierService : ILeverancierService
    {
        private readonly HttpClient httpClient;

        public LeverancierService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<LeverancierDto>> GetLeveranciersAsync()
        {
            var leveranciers = await httpClient.GetFromJsonAsync<IEnumerable<LeverancierDto>>("leverancier");
            return leveranciers!;
        }
    }
}
