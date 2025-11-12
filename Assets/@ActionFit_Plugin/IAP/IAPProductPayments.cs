using System;

namespace ActionFit_Plugin.IAP
{
    public class IAPProductPayments
    {
        public Action PayAction(IAPProductID id)
        {
            switch (id)
            {
                case IAPProductID.adRemove:
                    return () =>
                    {
                        PlayerData.AdsRemove = true;
                    };
                default:
                    throw new Exception($"No Pay Action {id.ToString()}");
            }
        }
        
    }
}