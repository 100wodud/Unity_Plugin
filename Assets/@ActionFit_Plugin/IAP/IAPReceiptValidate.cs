using System;
using System.Collections.Generic;
using System.Linq;
using ActionFit_Plugin.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace ActionFit_Plugin.IAP
{
#if ENABLE_IN_APP_PURCHASE
    public class IAPReceiptValidate
    {
        #region Fields

        private byte[] _dataGooglePlay;
        private byte[] _dataApple;

        #endregion
        
        
        
        #region Validate Process

        public CrossPlatformValidator GetCrossPlatformValidator()
        {
            // _dataGooglePlay = GooglePlayTangle.Data();
            // _dataApple = AppleTangle.Data();
            
            return new CrossPlatformValidator(_dataGooglePlay, _dataApple, Application.identifier);
        }
        
        /// <returns>
        /// True : 구매한 내역이 존재한다면
        /// False : 구매한 내역이 없다면
        /// </returns>
        private bool HasValidateNonConsumableReceipt(IAPProductID productTypeId)
        {
            if (Initializer.Instance.IAP is null || !Initializer.Instance.IAP.IsInitialized())
            {
#if DEV
                Debug.LogError("IAP System is null, or Don't Initialized.");
#endif
                return false;
            }
            
            // Receipt Configuration
            var productID = productTypeId.ToString();
            var product = IAPManager.StoreController.products.WithID(productID);
            // Validator
            var validator = GetCrossPlatformValidator();

            if (product.receipt == null)
            {
#if DEV
                Debug.LogError("Product or ProductReceipt is null.");
#endif
                return false;
            }

            try
            {
                var purchaseReceipts = validator.Validate(product.receipt);
                return HasValidateNonConsumableReceiptInternal(product, purchaseReceipts);
            }
            catch (IAPSecurityException exception)
            {
#if UNITY_EDITOR || DEV
                Debug.LogError("Invalid Receipt." + exception.Message);
#endif
                return false;
            }
        }

        #endregion



        #region Has Validate Process (Internal)

        private bool HasValidateNonConsumableReceiptInternal(Product currentProduct, IPurchaseReceipt[] purchaseReceipts)
        {
            var latestPurchaseReceipts = new List<IPurchaseReceipt>();

            foreach (var receipt in purchaseReceipts)
            {
                var purchaseDate = receipt.purchaseDate;
                var isUpdated = false;

                // 리스트에 이미 존재하는 동일한 productID의 영수증을 찾음
                for (int idx = 0; idx < latestPurchaseReceipts.Count; ++idx)
                {
                    if (receipt.productID != latestPurchaseReceipts[idx].productID)
                    {
                        continue;
                    }
                    
                    // 현재 영수증이 더 최신인 경우, 리스트를 업데이트
                    if (purchaseDate > latestPurchaseReceipts[idx].purchaseDate)
                    {
                        latestPurchaseReceipts[idx] = receipt;
                        isUpdated = true;
                        break;
                    }
                }

                // 리스트에 동일한 productID의 영수증이 없으면 추가
                if (!isUpdated) latestPurchaseReceipts.Add(receipt);
            }

            return GetHasNonConsumableReceiptIsNotRefunded(currentProduct, latestPurchaseReceipts);
        }

        private bool GetHasNonConsumableReceiptIsNotRefunded(Product currentProduct, List<IPurchaseReceipt> purchaseReceipts)
        {
            return (from productReceipt in purchaseReceipts 
                where currentProduct.definition.id == productReceipt.productID 
                select RefundCheck(productReceipt)).FirstOrDefault();
        }

        #endregion

        
        public bool NonConsumableValidate(IAPProductID skuID)
        { 
#if UNITY_EDITOR || DEV
            return false;
#else
            return HasValidateNonConsumableReceipt(skuID);
#endif
        }

        private bool RefundCheck(IPurchaseReceipt receipt)
        {
            switch (receipt)
            {
                case GooglePlayReceipt google:
                    GoogleReceipt(google);
                    return google.purchaseState is GooglePurchaseState.Purchased;
                case AppleInAppPurchaseReceipt apple:
                    AppleReceipt(apple);
                    // Debug.Log("Cancellation Date : " + apple.cancellationDate + " And Product ID" + receipt.productID);
                    return apple.cancellationDate == DateTime.MinValue;
                default:
                    return false;
            }
        }

        private void GoogleReceipt(GooglePlayReceipt google)
        {
            // Google 트랜잭션 ID : 구매 거래의 고유 식별자
            Debug.Log($"Google Transaction ID : {google.transactionID}");
            // Google 제품 ID : 구매한 상품의 고유 식별자
            Debug.Log($"Google Product ID : {google.productID}");
            // Google 패키지 이름 : 앱의 패키지 이름
            Debug.Log($"Google Package Name :{google.packageName}");
            // Google 구매 토큰 : 구매를 고유하게 식별하는 토큰
            Debug.Log($"Google Purchase Token : {google.purchaseToken}");
            // Google 구매 날짜 : 상품이 구매된 날짜와 시간
            Debug.Log($"Google Purchase Date : {google.purchaseDate}");
            // Google 구매 상태 : 구매의 상태 (구매됨, 취소됨, 환불됨 등)
            Debug.Log($"Google Purchase State : {google.purchaseState}");
        }
        private void AppleReceipt(AppleInAppPurchaseReceipt apple)
        {
            // Apple 트랜잭션 ID : 구매 거래의 고유 식별자
            Debug.Log($"Apple Transaction ID : {apple.transactionID}");
            // Apple 제품 ID : 구매한 상품의 고유 식별자
            Debug.Log($"Apple Product ID : {apple.productID}");
            // Apple 원래 트랜잭션 식별자 : 원래 구매 거래의 식별자
            Debug.Log($"Apple Original Transaction Identifier : {apple.originalTransactionIdentifier}");
            // Apple 원래 구매 날짜 : 원래 거래가 이루어진 날짜
            Debug.Log($"Apple Original Purchase Date : {apple.originalPurchaseDate}");
            // Apple 구매 날짜 : 상품이 구매된 날짜와 시간
            Debug.Log($"Apple Purchase Date : {apple.purchaseDate}");
            // Apple 제품 유형 : 상품의 유형 (예: 일회성 구매, 구독)
            Debug.Log($"Apple Product Type : {apple.productType}");
            // Apple 구매 수량 : 구매된 상품의 수량
            Debug.Log($"Apple Quantity : {apple.quantity}");
            // Apple 구매 취소 날짜 : 구매가 취소된 경우의 날짜와 시간
            Debug.Log($"Apple Cancellation Date : {apple.cancellationDate}");
            // Apple 무료 체험 기간 여부 : 구독의 무료 체험 기간 여부
            Debug.Log($"Apple Free Trial : {apple.isFreeTrial}");
            // Apple 구독 만료 날짜 : 구독의 만료 날짜
            Debug.Log($"Apple Subscription Expiration Date : {apple.subscriptionExpirationDate}");
            // Apple 소개 가격 기간 : 소개 가격 기간 여부
            Debug.Log($"Apple Introductory Price Period : {apple.isIntroductoryPricePeriod}");
        }
    }
#endif
}