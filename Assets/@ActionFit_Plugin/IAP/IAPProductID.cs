using System.Collections.Generic;

namespace ActionFit_Plugin.IAP
{
    public enum IAPProductID
    {
        // IAP 아이디
        adRemove
    }

    public static class IAPProductCategory
    {
        public static readonly IAPProductID[] NonConsumable = {
            IAPProductID.adRemove,
            // 다른 NonConsumable 제품들 추가 가능
        };
    }
}