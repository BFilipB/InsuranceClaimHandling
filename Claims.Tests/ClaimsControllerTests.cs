using System.Net.Http.Json;
using Claims.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            var claims = await response.Content.ReadFromJsonAsync<IEnumerable<Claim>>();

            // A brand-new instance has an empty Mongo collection, so the important assertion
            // is that the body actually deserializes into a (possibly empty) list of Claims,
            // not that it's non-empty.
            Assert.NotNull(claims);
        }
    }
}
