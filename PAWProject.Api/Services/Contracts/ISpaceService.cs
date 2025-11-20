using PAWProject.Models.DTO.SpaceFlightDTOs;

namespace PAWProject.Api.Services.Contracts
{
    public interface ISpaceService
    {
        Task<SpaceApiDTO> GetDataAsync(int limit, int offset);
    }
}