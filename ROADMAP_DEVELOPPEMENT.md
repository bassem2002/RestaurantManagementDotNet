# ?? Roadmap de Développement - ProjetResto

## ? État Actuel du Projet

### Structure Existante
```
ProjetResto/
??? Models/                    ? Modèles de données
??? Backend/                   ? API REST (ASP.NET Core)
?   ??? Controllers/           ? (Account, Category, Item, Cart, Order, Payment)
?   ??? Repositories/          ? Pattern Repository
?   ??? DTOs/                  ? Data Transfer Objects
?   ??? Program.cs             ? Configuration DI + JWT + CORS
??? Front/                     ?? Blazor WebAssembly
    ??? Services/              ? (Auth, Categorie, Item, Cart, Order, Client)
    ??? Pages/                 ?? Admin + Client (partiel)
    ??? Layout/                ?? MainLayout + ClientLayout
    ??? Program.cs             ? Configuration DI + Auth
```

---

## ?? PRIORITÉ 1: Fonctionnalités Essentielles (À FAIRE EN PREMIER)

### 1?? **Page de Commande Complète (Checkout)**
**Fichiers à créer:**
- `Front/Pages/Client/Checkout.razor` - Formulaire de commande
- `Front/Models/CheckoutRequest.cs` - Modèle de requête
- `Front/Services/OrderServices.cs` - Service de commande (amélioration)

**Fonctionnalités:**
```
Panier ? Formulaire de livraison ? Récapitulatif ? Paiement ? Confirmation
```

**À implémenter:**
```csharp
// Front/Services/OrderServices.cs
public async Task<OrderResponseDTO> CreateOrderAsync(CheckoutRequest request)
{
    var order = new Order 
    { 
        UserId = userId,
        Items = cartService.Items,
        TotalAmount = cartService.Total,
        ShippingAddress = request.Address,
        PhoneNumber = request.Phone
    };
    return await _httpClient.PostAsJsonAsync("/api/order", order);
}
```

### 2?? **Page de Paiement (Stripe/PayPal Integration)**
**Fichiers à créer:**
- `Front/Pages/Client/Payment.razor` - Interface de paiement
- `Front/Services/PaymentService.cs` - Service Stripe/PayPal
- `Backend/Controllers/PaymentController.cs` - Webhook de paiement

**À ajouter dans `Program.cs`:**
```csharp
builder.Services.AddScoped<PaymentService>();
```

### 3?? **Notifications en Temps Réel (SignalR)**
**Fichiers à créer:**
- `Backend/Hubs/OrderHub.cs` - Hub SignalR
- `Front/Services/NotificationService.cs` - Service notification

**À ajouter dans `Backend/Program.cs`:**
```csharp
builder.Services.AddSignalR();
app.MapHub<OrderHub>("/orderhub");
```

---

## ?? PRIORITÉ 2: Fonctionnalités de Gestion (Important)

### 4?? **Page de Profil Utilisateur**
**Fichiers à créer:**
- `Front/Pages/Client/Profile.razor` - Édition du profil
- `Front/Pages/Client/Addresses.razor` - Gestion des adresses
- `Front/Pages/Client/Orders.razor` - Historique des commandes

### 5?? **Page d'Historique des Commandes**
**Fichiers à créer:**
- `Front/Pages/Client/OrderHistory.razor` - Liste des commandes
- `Front/Pages/Client/OrderTracking.razor` - Suivi en temps réel

### 6?? **Dashboard Admin Avancé**
**Améliorer:**
- `Front/Pages/Admin/Dashboard.razor` - Graphiques de ventes
- `Front/Pages/Admin/Reports.razor` - Rapports
- `Front/Pages/Admin/Analytics.razor` - Statistiques

---

## ?? PRIORITÉ 3: Optimisations & Sécurité

### 7?? **Sécurité & Authentification**
- ? JWT existant
- ? **À AJOUTER:** Refresh token
- ? **À AJOUTER:** 2FA (Two-Factor Authentication)
- ? **À AJOUTER:** Role-based authorization

**À créer:**
```csharp
// Backend/Services/TokenService.cs
public string GenerateRefreshToken()
{
    var randomNumber = new byte[64];
    using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
```

### 8?? **Validation & Error Handling**
- ? Validation côté frontend (FluentValidation)
- ? Gestion globale des erreurs
- ? Messages d'erreur localisés (i18n)

**À créer:**
```csharp
// Front/Services/ValidationService.cs
public class ValidationService 
{
    public ValidationResult ValidateCheckout(CheckoutRequest request)
    {
        // Validation logique
    }
}
```

### 9?? **Optimisation Performance**
- ? Pagination des listes (Category, Items, Orders)
- ? Cache côté client (IndexedDB)
- ? Lazy loading des images

---

## ?? PRIORITÉ 4: UX/UI & Polish

### ?? **Design & Styling**
- ? Bootstrap 5 existant
- ? **À AJOUTER:** Thème personnalisé (couleurs, logo)
- ? **À AJOUTER:** Animations (loading, transitions)
- ? **À AJOUTER:** Responsive design (mobile-first)

**À améliorer:**
```css
/* Front/wwwroot/css/app.css */
:root {
    --primary-color: #ff6b35;
    --secondary-color: #f7931e;
    --success-color: #06a77d;
}

.loading-animation {
    animation: spin 1s linear infinite;
}
```

### 1??1?? **Notifications Toast**
- ? Service de toasts (succès, erreur, avertissement)

**À créer:**
```csharp
// Front/Services/ToastService.cs
public class ToastService
{
    public void ShowSuccess(string message) { }
    public void ShowError(string message) { }
    public void ShowInfo(string message) { }
}
```

### 1??2?? **Progressive Web App (PWA)**
- ?? Service worker existant
- ? **À AMÉLIORER:** Manifest complet
- ? **À AJOUTER:** Offline support

---

## ?? Code à Ajouter Maintenant

### **1. Ajouter OrderServices complet**
```csharp
// Front/Services/OrderServices.cs
public class OrderServices
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public OrderServices(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<OrderDTO>> GetUserOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<OrderDTO>>("api/order");
    }

    public async Task<OrderDTO> GetOrderByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<OrderDTO>($"api/order/{id}");
    }

    public async Task<OrderDTO> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/order", request);
        return await response.Content.ReadAsAsync<OrderDTO>();
    }
}
```

### **2. Créer Checkout.razor**
```razor
@page "/client/checkout"
@layout ClientLayout
@inject Front.Services.OrderServices orderService
@inject Front.Services.CartService cartService
@inject NavigationManager navigationManager

<h2>Passer la commande</h2>

<div class="row">
    <div class="col-md-6">
        <div class="card">
            <div class="card-body">
                <h5>Adresse de Livraison</h5>
                <form @onsubmit="HandleCheckout">
                    <div class="mb-3">
                        <label>Adresse</label>
                        <input type="text" class="form-control" @bind="request.ShippingAddress" required />
                    </div>
                    <div class="mb-3">
                        <label>Téléphone</label>
                        <input type="tel" class="form-control" @bind="request.Phone" required />
                    </div>
                    <button type="submit" class="btn btn-primary w-100">Valider</button>
                </form>
            </div>
        </div>
    </div>

    <div class="col-md-6">
        <div class="card">
            <div class="card-body">
                <h5>Résumé</h5>
                <div class="d-flex justify-content-between">
                    <strong>Total:</strong>
                    <strong>@cartService.Total DT</strong>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private CreateOrderRequest request = new();

    private async Task HandleCheckout()
    {
        var order = await orderService.CreateOrderAsync(request);
        navigationManager.NavigateTo($"/client/order-confirmation/{order.OrderId}");
    }
}
```

### **3. Ajouter Models manquants**
```csharp
// Front/Models/CheckoutRequest.cs
public class CheckoutRequest
{
    public string ShippingAddress { get; set; }
    public string Phone { get; set; }
    public string Notes { get; set; }
}

// Front/Models/OrderDTO.cs
public class OrderDTO
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public List<OrderItemDTO> Items { get; set; }
}
```

### **4. Améliorer Program.cs**
```csharp
// Ajouter les services manquants
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<NotificationService>();

// Ajouter Blazored.LocalStorage pour persister les données
builder.Services.AddBlazoredLocalStorage();
```

---

## ?? Ordre de Développement Recommandé

### **Semaine 1:**
1. ? Checkout (formulaire)
2. ? Order confirmation
3. ? Payment gateway (Stripe ou Paypal)

### **Semaine 2:**
4. ? User profile page
5. ? Order history
6. ? Order tracking

### **Semaine 3:**
7. ? Admin dashboard (graphiques)
8. ? Notifications (SignalR)
9. ? Refresh tokens

### **Semaine 4:**
10. ? Validation (FluentValidation)
11. ? Toast notifications
12. ? PWA offline support

---

## ??? Dépendances à Ajouter

```bash
# Frontend
dotnet add package Blazored.LocalStorage
dotnet add package Radzen.Blazor (pour les graphiques admin)
dotnet add package CurrieTechnologies.Razor.SweetAlert2 (notifications)

# Backend
dotnet add package Stripe.net (paiement)
dotnet add package FluentValidation (validation)
```

---

## ? Quick Start Checklist

- [ ] Créer `Checkout.razor`
- [ ] Implémenter `OrderServices.CreateOrderAsync()`
- [ ] Créer page de confirmation
- [ ] Intégrer Stripe/PayPal
- [ ] Ajouter notifications
- [ ] Créer page Profile
- [ ] Créer page Order History
- [ ] Ajouter graphiques admin
- [ ] Implémenter SignalR
- [ ] Ajouter validation

---

**Total estimé: 4 semaines de développement pour une app complète et production-ready! ??**
