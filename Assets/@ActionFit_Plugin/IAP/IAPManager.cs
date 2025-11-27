using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActionFit_Plugin.SDK;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace ActionFit_Plugin.IAP
{
#if ENABLE_IN_APP_PURCHASE
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        #region MyRegion

        private const string Environment = "production";
        public static IStoreController StoreController; // 구매과정을 제어하는 함수 제공 
        private IExtensionProvider _storeExtensionProvider; // 크로스 플랫폼 확장 메서드 제공
        private ConfigurationBuilder _builder;
        private readonly IAPFunction _iapFunction = new();
        private bool _isRestore = false;
        private TaskCompletionSource<bool> _iapInitTcs;
        private readonly Dictionary<IAPProductID, Action> _productPurchaseEvents = new();
        
        #endregion

        public bool IsInitialized() => StoreController != null && _storeExtensionProvider != null;
        public event Action OnCompleteEvent;
        public event Action OnFailedEvent;
        
        public async UniTask Initialized()
        {
            _isRestore = false;
            _iapInitTcs = new TaskCompletionSource<bool>();
            try
            {
                InitializationOptions options = new InitializationOptions().SetEnvironmentName(Environment);
                await UnityServices.InitializeAsync(options);
                if (StoreController != null && _storeExtensionProvider != null)
                {
                    _iapInitTcs.TrySetResult(true);
                    return;
                }
                InitializePurchasing();
                Debug.Log("[IAP] Initialized");
                await _iapInitTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError("[IAP] UnityServices Initialized Fail: " + ex.Message);
                Debug.LogError("Inner Exception: " + ex.InnerException?.Message);
                _iapInitTcs.TrySetException(ex);
            }
        }
        
        public void BuyProduct(IAPProductID productType)
        {
            SDKManager.IsDoingIAP = true;
#if UNITY_EDITOR
            ProcessPurchase(productType);
            SDKManager.IsDoingIAP = false;
            // State Changed
            
            
#else
            string productId = productType.ToString();
            if (!IsInitialized())
            {
                Debug.LogError("[IAP] Initialized Fail");
                return;
            }
            
            // UI Waiting Popup

            //
            
            Product product = StoreController.products.WithID(productId);
            if (product is { availableToPurchase: true })
            {
                try
                {
                    StoreController.InitiatePurchase(product);
                }
                catch (Exception e)
                {
                    Debug.LogError("[IAP] BuyProduct Fail: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("[IAP] Cannot BuyProduct" );
            }
#endif
        }
        
        private void InitializePurchasing()
        {
            if (IsInitialized())  return;
            _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            IAPProductPayments action = new IAPProductPayments();
            foreach (IAPProductID productId in Enum.GetValues(typeof(IAPProductID)))
            {
                try
                {
                    _builder.AddProduct(productId.ToString(),
                        IAPProductCategory.NonConsumable.Contains(productId)
                            ? ProductType.NonConsumable
                            : ProductType.Consumable);
                
                    _productPurchaseEvents.Add(productId, action.PayAction(productId));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[IAP] Failed to add product {productId} - {ex.Message}");
                    continue; // 문제가 발생하면 다음 제품으로 넘어갑니다.
                }
            }
            UnityPurchasing.Initialize(this, _builder);
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _isRestore = true;
            _storeExtensionProvider = extensions;
            StoreController = controller;
            _iapInitTcs?.TrySetResult(true);
            ValidateAllNonConsumable();
        }
        
        public Product GetProduct(IAPProductID id) => IsInitialized() ? StoreController.products.WithID(id.ToString()) : null;
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) => OnFailedIAPEvent(product, failureDescription.reason.ToString());
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) => OnFailedIAPEvent(product, failureReason.ToString());
        public void OnInitializeFailed(InitializationFailureReason error) => _iapInitTcs?.TrySetException(new Exception("[IAP] Init Failed"));
        public void OnInitializeFailed(InitializationFailureReason error, string message) => _iapInitTcs?.TrySetException(new Exception("[IAP] Init Failed"));
        private IAPProductID StringToEnum(string productId)
        {
            if (Enum.TryParse(productId, out IAPProductID parsedEnum)) return parsedEnum;
            throw new ArgumentException("Invalid product ID: " + productId);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
#if UNITY_ANDROID
            _iapFunction.SDKLogger("Google Play Store", purchaseEvent);
#elif UNITY_IOS
            _iapFunction.SDKLogger("Apple App Store", purchaseEvent);
#endif
            
            IAPProductID currentProductType = StringToEnum(purchaseEvent.purchasedProduct.definition.id);
            IAPReceiptValidate iapValidator = new IAPReceiptValidate();
            CrossPlatformValidator validator = iapValidator.GetCrossPlatformValidator();

            try
            {
                IPurchaseReceipt[] result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    if (productReceipt is not GooglePlayReceipt google) continue;
                    if (google.purchaseState != GooglePurchaseState.Deferred) continue;
                    return PurchaseProcessingResult.Pending;
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("IAP - Invalid receipt, not unlocking content");
                throw;
            }
            
            
            if (!_productPurchaseEvents.TryGetValue(currentProductType, out Action iapProduct))
                return PurchaseProcessingResult.Pending;
            // 팝업
            OnCompleteIAPEvent();
            iapProduct.Invoke();
            return PurchaseProcessingResult.Complete;
        }

        
        
#if UNITY_EDITOR
        private PurchaseProcessingResult ProcessPurchase(IAPProductID id)
        {
            // <==== Purchase Complete ===> //
            if (_productPurchaseEvents.TryGetValue(id, out Action iapProduct))
            {
                iapProduct.Invoke();
                return PurchaseProcessingResult.Complete;
            }
            return PurchaseProcessingResult.Pending;
        }
#endif
        
        // 전체 NonConsumable 영수증 검증 후 복구
        private void RestoreNonConsumable()
        {
            if(_isRestore) return;
            
            _storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result, str) => 
            {
                ValidateAllNonConsumable();
            });
        }

        #region Validate

        // 영수증 검증 및 복구
        public bool ValidateProduct(IAPProductID productId, bool restore = false)
        {
            IAPReceiptValidate iapValidator = new IAPReceiptValidate();
            bool result = iapValidator.NonConsumableValidate(productId);
            if (restore && result)
            {
                if (_productPurchaseEvents.TryGetValue(productId, out Action iapProduct))
                {
                    iapProduct.Invoke();
                }
            }

            return result;
        } 

        private void ValidateAllNonConsumable()
        {      
            IAPReceiptValidate iapValidator = new IAPReceiptValidate();
            foreach (IAPProductID productId in IAPProductCategory.NonConsumable)
            {
                iapValidator.NonConsumableValidate(productId);
                if (_productPurchaseEvents.TryGetValue(productId, out Action iapProduct))
                {
                    iapProduct.Invoke();
                }
            }
        }
        
        #endregion

        private void OnCompleteIAPEvent(Product product = null, string message = null)
        {
            // 알아서 넣으시오
            OnCompleteEvent?.Invoke();
        }
        
        private void OnFailedIAPEvent(Product product = null, string message = null)
        {
            // 알아서 넣으시오
            OnFailedEvent?.Invoke();
        }
    }
#endif
}