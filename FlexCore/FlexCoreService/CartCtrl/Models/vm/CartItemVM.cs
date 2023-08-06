﻿namespace FlexCoreService.CartCtrl.Models.vm
{
	public class CartItemVM
	{
		public int Id { get; set; }
		public int CartId { get; set; }
		public int ProductId { get; set; }
		public int Qty { get; set; }
		public CartItemProductVM Product { get; set; }
		public int SubTotal => Product.SalesPrice * Qty;
	}
}
