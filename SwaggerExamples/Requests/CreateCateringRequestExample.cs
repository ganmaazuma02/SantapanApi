using SantapanApi.Contracts.V1.Requests;
using SantapanApi.Domain.Constants;
using Swashbuckle.AspNetCore.Filters;

namespace SantapanApi.SwaggerExamples.Requests
{
    public class CreateCateringRequestExample : IExamplesProvider<CreateCateringRequest>
    {
        public CreateCateringRequest GetExamples()
        {
            return new CreateCateringRequest
            {
                Email = "caterer@santapan.my",
                Name = "Kambing Golek Mak Minah",
                CateringCategories = new string[] { Categories.Station, Categories.Wedding},
                Details = "Stesen kambing golek bakar"
            };
        }
    }
}
