namespace Rise.Shared.Leveranciers
{
    public interface ILeverancierService
    {
        Task<IEnumerable<LeverancierDto>> GetLeveranciersAsync();
    }
}
