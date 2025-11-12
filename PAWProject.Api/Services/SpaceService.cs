using PAW3CP1.Architecture;
using PAW3CP1.Architecture.Providers;
using PAWProject.Api.Controllers;
using PAWProject.Api.Services.Contracts;
using PAWProject.Models.DTO.SpaceFlightDTOs;
using static System.Net.WebRequestMethods;

namespace PAWProject.Data
{
    public class SpaceService(IRestProvider restProvider, IConfiguration configuration) : ISpaceService
    {
        private string baseUrl = "https://api.spaceflightnewsapi.net/v4/articles/";
        //Documentation: https://api.spaceflightnewsapi.net/v4/docs/

        public async Task<SpaceApiDTO> GetDataAsync()
        {
            var response = await restProvider.GetAsync(baseUrl, null);
            var data = await JsonProvider.DeserializeAsync<SpaceApiDTO>(response);
            return data;
        }
    }
}
