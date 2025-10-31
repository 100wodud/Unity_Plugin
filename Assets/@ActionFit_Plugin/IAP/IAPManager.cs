// using System;
// using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Security;
//
// namespace ActionFit_Plugin.IAP
// {
//     public class IAPManager : IStoreListener
//     {
//         public static IAPManager Instance { get; private set; } = new();
//         private bool IsInitialized() => _storeController != null && _storeExtensionProvider != null;
//         
//         private static IStoreController _storeController;
//         private static IExtensionProvider _storeExtensionProvider;
//
//         [SerializeField] private IAPProductData _productDatabase;
//
//         public void Initialize()
//         {
//             if (IsInitialized()) return;
//
//             var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
//
//             foreach (var product in _productDatabase.products)
//             {
//                 builder.AddProduct(product.productId, product.productType);
//             }
//
//             UnityPurchasing.Initialize(this, builder);
//         }
//
//         public void BuyProduct(string productId)
//         {
//             if (IsInitialized())
//             {
//                 _storeController.InitiatePurchase(productId);
//             }
//         }
//
//         public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//         {
//             _storeController = controller;
//             _storeExtensionProvider = extensions;
//             Debug.Log("IAP Initialized");
//         }
//
//         public void OnInitializeFailed(InitializationFailureReason error)
//         {
//             Debug.LogError($"IAP Init Failed: {error}");
//         }
//
//         public void OnInitializeFailed(InitializationFailureReason error, string message = null)
//         {
//             throw new NotImplementedException();
//         }
//
//         public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//         {
//             Debug.Log($"Purchase Success: {args.purchasedProduct.definition.id}");
//
//             // 구매 완료 처리 로직
//             return PurchaseProcessingResult.Complete;
//         }
//
//         public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//         {
//             Debug.LogWarning($"Purchase Failed: {product.definition.id}, Reason: {failureReason}");
//         }
//
//         private bool ValidateReceipt(Product product)
//         {
// #if UNITY_EDITOR
//             Debug.Log("Receipt validation skipped in Editor.");
//             return true;
// #endif
//
//             try
//             {
//                 var validator = new CrossPlatformValidator(
// #if UNITY_ANDROID
//                     GooglePlayTangle.Data(),
// #elif UNITY_IOS
//             AppleTangle.Data(),
// #else
//             null,
// #endif
//                     null,
//                     Application.identifier);
//
//                 var result = validator.Validate(product.receipt);
//
//                 foreach (IPurchaseReceipt receipt in result)
//                 {
//                     Debug.Log($"✔ Valid receipt: {receipt.productID}, purchase date: {receipt.purchaseDate}");
//                 }
//
//                 return true;
//             }
//             catch (IAPSecurityException ex)
//             {
//                 Debug.LogError($"❌ Receipt validation error: {ex.Message}");
//                 return false;
//             }
//         }
//     }
// }