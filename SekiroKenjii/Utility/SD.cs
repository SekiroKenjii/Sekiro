using SekiroKenjii.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Utility
{
    public class SD
    {
        public const string DefaultSupplierImage = "default_supplier_image.jpg";
        public const string ImageFolder = @"images\SupplierImage";

        public const string DefaultProductImage = "default_product_image.jpg";
        public const string ProductImageFolder = @"images\ProductImage";
        public const string DefaultProductHQImage = "default_product_hqimage.jpg";
        public const string ProductHQImageFolder = @"images\ProductHQImage";

        public const string DefaultCategoryImage = "default_category_image.jpg";
        public const string CategoryImageFolder = @"images\CategoryImage";

        public const string ManagerUser = "Manager";
        public const string CustomerOfficer = "Customer Officer";
        public const string PackingUser = "Packing Staff";
        public const string Shipper = "Shipper";
        public const string CustomerUser = "Customer";

        public const string ssShoppingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusReadyPack = "Ready to Pack";
        public const string StatusInPackingProcess = "Being Packing";
        public const string StatusReadyShip = "Ready to Ship";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
            if (couponFromDb == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimumAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
                        //$20 off $100
                        return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
                    }
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
                    {
                        //20% off $500
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.Discount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
