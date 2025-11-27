using System.Collections.Generic;
using Singular;
using UnityEngine.Purchasing;

namespace ActionFit_Plugin.IAP
{
    public class IAPFunction
    {
#if ENABLE_IN_APP_PURCHASE
        #region Logger

        private string Revenue(PurchaseEventArgs purchaseEventArgs) => purchaseEventArgs.purchasedProduct.metadata.localizedPriceString;
        private (Product, Dictionary<string, object>) SingularInApp(PurchaseEventArgs purchaseEventArgs)
        {
            Product product = purchaseEventArgs.purchasedProduct;
            Dictionary<string, object> attribute = new Dictionary<string, object>
            {
                {"Revenue",Revenue(purchaseEventArgs)}
            };
            return (product, attribute);
        }
        
        public void SDKLogger(string store, PurchaseEventArgs purchaseEvent)
        {
            Product product;
            Dictionary<string, object> attribute;
            (product, attribute) = SingularInApp(purchaseEvent);
#if ENABLE_SINGULAR_SDK
            SingularSDK.InAppPurchase(product, attribute);
#endif
        }
        
        #endregion
#endif
    }
}