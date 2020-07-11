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

        public const string StatusSubmitted = "Chờ xác nhận";
        public const string StatusApproved = "Đã xác nhận";
        public const string StatusDenied = "Đã từ chối";
        public const string StatusShipping = "Đang giao hàng";
        public const string StatusReadyPack = "Sẵn sàng đóng gói";
        public const string StatusInPackingProcess = "Đang đóng gói";
        public const string StatusReadyShip = "Sẵn sàng vận chuyển";
        public const string StatusCompleted = "Hoàn thành";
        public const string StatusCancelled = "Đã huỷ";
        public const string StatusCancelOnWeb = "Đả huỷ bởi khách hàng";

        public const string PaymentStatusPending = "Đang chờ";
        public const string PaymentStatusApproved = "Đã phê duyệt";
        public const string PaymentStatusRejected = "Đã từ chối (Chờ hoàn tiền)";
        public const string PaymentStatusReturned = "Đã hoàn tiền";

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
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.VND)
                    {
                        return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
                    }
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
                    {
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.Discount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
