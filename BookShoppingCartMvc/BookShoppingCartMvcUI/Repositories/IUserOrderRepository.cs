﻿using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMvcUI.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}