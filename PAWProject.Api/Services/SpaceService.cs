using PAW3CP1.Architecture;
using PAW3CP1.Architecture.Providers;
using PAWProject.Api.Services.Contracts;
using PAWProject.Models.DTO.SpaceFlightDTOs;

namespace PAWProject.Data
{
    public class SpaceService(IRestProvider restProvider, IConfiguration configuration) : ISpaceService
    {
        private string baseUrl = "https://api.spaceflightnewsapi.net/v4/articles/";
        //Documentation: https://api.spaceflightnewsapi.net/v4/docs/

        public async Task<SpaceApiDTO> GetDataAsync(int limit = 10, int offset = 0)
        {
            var url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await restProvider.GetAsync(url, null);
            return await JsonProvider.DeserializeAsync<SpaceApiDTO>(response);
        }
    }
}
