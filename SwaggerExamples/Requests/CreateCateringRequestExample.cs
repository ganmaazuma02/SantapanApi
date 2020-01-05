using SantapanApi.Contracts.V1.Requests;
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
                Category = "side",
                Details = "Stesen kambing golek bakar"
            };
        }
    }
}
