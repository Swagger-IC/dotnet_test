using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Components;

namespace Rise.Client.Orders;
public class Winkelmand
{
        private readonly List<WinkelmandItem> items = new();
        public IReadOnlyList<WinkelmandItem> Items => items.AsReadOnly();
        public event Action? OnCartChanged;
        private readonly ISyncLocalStorageService localStorage;
        private readonly IModalService modalService;
        public Winkelmand(ISyncLocalStorageService localStorage, IModalService modalService)
        {
                this.localStorage = localStorage;
                this.modalService = modalService;
                OnCartChanged += UpdateLocalStorage;
        }

        public void Load()
        {
                if (items.Count == 0)
                {
                        var stored = this.localStorage.GetItem<List<WinkelmandItem>>("Winkelmand");
                        if (stored != null)
                        {
                                items.AddRange(stored);
                                OnCartChanged?.Invoke();
                        }
                }
        }

        public void AddItem(int productId, string name, int amount, int inStock)
        {
                if (amount > inStock)
                {
                        ToonModal(inStock, amount);
                }
                else
                {
                        var existingItem = items.SingleOrDefault(x => x.ProductId == productId);
                        if (existingItem == null)
                        {
                                WinkelmandItem item = new WinkelmandItem(productId, name, amount, inStock);
                                items.Add(item);
                        }
                        else
                        {
                                if (existingItem.Amount + amount > inStock)
                                {
                                        ToonModal(inStock, existingItem.Amount + amount);
                                }
                                else
                                {
                                        existingItem.Amount += amount;
                                }
                        }
                        OnCartChanged?.Invoke();
                }

                // aantal controleren bij het definitief uitscannen
        }
        public void AmountChanged(int productId, int amount){
                var existingItem = items.SingleOrDefault(x => x.ProductId == productId);
                if(existingItem != null){
                        existingItem.Amount = amount;
                }
                OnCartChanged?.Invoke();
        }
        public void RemoveItem(WinkelmandItem item)
        {
                items.Remove(item);
                OnCartChanged?.Invoke();
        }
        private void UpdateLocalStorage()
        {
                this.localStorage.SetItem("Winkelmand", items);
        }
        private void ToonModal(int inStock, int amount)
        {
                var parameters = new ModalParameters
                        {
                                { "InStock", inStock },
                                { "Amount", amount }
                        };
                this.modalService.Show<FoutmeldingProductDetail>("Waarschuwing!", parameters, new ModalOptions
                {
                        DisableBackgroundCancel = true
                });
        }
}
