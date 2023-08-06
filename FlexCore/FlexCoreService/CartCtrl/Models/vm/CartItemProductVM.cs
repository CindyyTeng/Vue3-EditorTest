﻿namespace FlexCoreService.CartCtrl.Models.vm
{
	public class CartItemProductVM
	{
		public string? ProductSaleId { get; set; }
		public string? ProductName { get; set; }
		public int? UnitPrice { get; set; }
		public int SalesPrice { get; set; }
		public string? Size { get; set; }
		public string? Color { get; set; }
		public IEnumerable<ProductDiscountVM>? MatchDiscounts { get; set; }
		public string? ImgPath { get; set; }
	}
}
