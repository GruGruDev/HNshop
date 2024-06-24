using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unidecode.NET;

namespace Suitshop.Utility
{
	public static class SD
	{
		//PageSize
		public const int PageSize = 4;

		//Category
		public const string Readytowear = "Trang phục";
		public const string Shoes = "Giày dép";
		public const string Accessories = "Phụ kiện";

		//Session key
		public const string Session_Cart = "Cart";

		//Role
		public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

		//Order Status
		public const string Order_WaitForConfirmation = "Chờ xác nhận";
		public const string Order_WaitForShip = "Chờ giao hàng";
		public const string Order_Shipped = "Đã giao hàng";
		public const string Order_Canceled = "Đã hủy";

		//Payment Status
		public const string Payment_Paid = "Đã thanh toán";
		public const string Payment_PaymentOnDelivery = "Thanh toán khi nhận hàng";
		public const string Payment_Refund = "Đã hoàn tiền";


		public static string MapVietnameseToEnglishColorName(string vietnameseColorName)
		{
			Dictionary<string, string> colorMap = new Dictionary<string, string>
			{
				{ "Đen", "black" },
				{ "Trắng", "white" },
				{ "Đỏ", "red" },
				{ "Lục", "green" },
				{ "Vàng", "yellow" },
				{ "Xám", "gray" },
				{ "Nâu", "brown" },
				{ "Cam", "orange" },
				{ "Hồng", "pink" },
				{ "Tím", "purple" },
				{ "Màu da", "beige" },
				{ "Xanh", "blue" },
			};

			if (colorMap.ContainsKey(vietnameseColorName))
			{
				return colorMap[vietnameseColorName];
			}

			return vietnameseColorName;
		}

		public static string CreateSlug(string name, int id)
		{
			string slug = name;

			// Loại bỏ dấu tiếng Việt
			slug = NonUnicode(slug);

			// Loại bỏ dấu và ký tự đặc biệt
			slug = Regex.Replace(slug, @"[^a-zA-Z0-9\s-]", "");

			// Loại bỏ dấu cách và thay thế dấu cách bằng gạch nối
			slug = slug.Replace(" ", "-");

			// Loại bỏ các ký tự liên tiếp của gạch nối
			slug = Regex.Replace(slug, @"-+", "-");

			// Đảm bảo slug không bắt đầu hoặc kết thúc bằng gạch nối
			slug = slug.Trim('-');

			// Thêm ID vào slug (hoặc bất kỳ thông tin khác nếu cần)
			slug = $"{slug}-{id}";

			// Chuyển thành chữ thường
			slug = slug.ToLower();

			return slug;
		}

		public static string NonUnicode(string text)
		{
			string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
											"đ",
											"é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
											"í","ì","ỉ","ĩ","ị",
											"ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
											"ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
											"ý","ỳ","ỷ","ỹ","ỵ",};
			string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
											"d",
											"e","e","e","e","e","e","e","e","e","e","e",
											"i","i","i","i","i",
											"o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
											"u","u","u","u","u","u","u","u","u","u","u",
											"y","y","y","y","y",};
			for (int i = 0; i < arr1.Length; i++)
			{
				text = text.Replace(arr1[i], arr2[i]);
				text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
			}
			return text;
		}

		public static string ReplaceSpace(string text)
		{
			string text2=text.Trim();
			char[] result = new char[text2.Length];

			for (int i = 0; i < text2.Length; i++)
			{
				if (text2[i] == ' ')
				{
					result[i] = '-';
				}
				else
				{
					result[i] = text2[i];
				}
			}

			return new string(result);
		}

	}
}
