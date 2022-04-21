using com.amazon.device.iap.cpt;

namespace Rilisoft
{
	internal static class MarketProductFactory
	{
		internal static GoogleMarketProduct CreateGoogleMarketProduct(GoogleSkuInfo googleSkuInfo)
		{
			return new GoogleMarketProduct(googleSkuInfo);
		}

		internal static AmazonMarketProduct CreateAmazonMarketProduct(ProductData amazonItem)
		{
			return new AmazonMarketProduct(amazonItem);
		}
	}
}
