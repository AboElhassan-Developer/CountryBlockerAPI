using CountryBlockerAPI.Services;

namespace CountryBlockerAPI.Tests
{
    public class BlockServiceAdditionalTests
    {
        private readonly BlockService _blockService;

        public BlockServiceAdditionalTests()
        {
            _blockService = new BlockService();
        }

        // Test removing a country that doesn't exist in the list
        [Fact]
        public void UnblockCountry_ShouldReturnFalse_WhenCountryDoesNotExist()
        {
            // Arrange
            string countryCode = "XX"; // Non-existent country

            // Act
            bool result = _blockService.UnblockCountry(countryCode);

            // Assert
            Assert.False(result); //  Should be `false` because the country wasn't blocked in the first place
        }

        //  Test blocking multiple countries and verifying the list
        [Fact]
        public void BlockMultipleCountries_ShouldAddAllCountries()
        {
            // Arrange
            string[] countryCodes = { "US", "FR", "EG" };

            // Act
            foreach (var country in countryCodes)
            {
                _blockService.BlockCountry(country);
            }

            // Assert
            foreach (var country in countryCodes)
            {
                Assert.True(_blockService.IsCountryBlocked(country)); //  Ensure each country is blocked
            }
        }

        //  Test temporary blocking and ensuring it expires after the specified time
        [Fact]
        public void BlockCountryTemporarily_ShouldExpireAfterTime()
        {
            // Arrange
            string countryCode = "DE";
            int durationMinutes = 1; // Only one minute

            // Act
            _blockService.BlockCountryTemporarily(countryCode, durationMinutes);

            // Assert
            Assert.True(_blockService.IsCountryBlocked(countryCode)); // Ensure the country is blocked

            //  Wait for a minute and then check again
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(1.1));

            // The block should have expired now
            Assert.False(_blockService.IsCountryBlocked(countryCode));
        }
    }
}
