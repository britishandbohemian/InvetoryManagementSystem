using Bogus;
using InventoryAPI.Model.CategoriesModel;
using InventoryAPI.Model.CustomizationModel;
using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.LinkModels;
using InventoryAPI.Model.NotificationsModel;
using InventoryAPI.Model.ProductsModel;
using InventoryAPI.Model.SalesModel;
using InventoryAPI.Model.SupplierModel;
using InventoryAPI.Model.UserModels;
using Microsoft.EntityFrameworkCore;
using static InventoryAPI.Model.SalesModel.Sale;

namespace InventoryAPI.Data
{
    // Database context class that inherits from DbContext
    public class DatabaseContext : DbContext
    {



        // Constructor for the database context, takes DbContextOptions as parameter
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }


        public DbSet<Expense> Expenses { get; set; }

        //------------CORE MODELS --------------------
        // Users
        public DbSet<User> Users { get; set; }
        public DbSet<Notifications> Notification { get; set; }

        // Components
        public DbSet<Item> Items { get; set; }

        // Products
        public DbSet<Product> Products { get; set; }

        public DbSet<ItemExpirationData> ItemExpirationData { get; set; }


        //Join table
        public DbSet<ProductToItem> ProductItems { get; set; }

        //------------CORE MODELS END --------------------


        //--------------CATEGORIZATION---------------

        //Categories
        public DbSet<Category> Categories { get; set; }


        //--------------CATEGORIZATION END---------------

        // System customization
        public DbSet<Customization> SystemCustomizations { get; set; }


        //---------------MY INVENTORY ------------------------
        // Orders


        // Sales
        public DbSet<Sale> Sales { get; set; }

        public DbSet<ProductExpirationEvent> ProductExpirationEvents { get; set; }

        //---------------MY INVENTORY END ------------------------


        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ComponentInSale> ComponentsInSale { get; set; }
        public DbSet<ProductInSale> ProductInSales { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        //Supplier and Supplier Orders
        //----------------------------

        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public class ProductItemMap
        {
            public int ProductId { get; set; }
            public List<int> ItemIds { get; set; }
        }

        public class ItemAmount
        {
            public int ItemId { get; set; }
            public decimal AmountUsed { get; set; }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ProductToItem>()
                   .HasOne(pi => pi.Product)
                   .WithMany(p => p.LinkedItems)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict); // or use DeleteBehavior.NoAction

            modelBuilder.Entity<ProductToItem>()
                .HasOne(pi => pi.Item)
                .WithMany()
                .HasForeignKey(pi => pi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductToItem>()
                .HasKey(pi => pi.LinkId);


            // Define ComponentInSale -> Product relationship (if applicable)
            modelBuilder.Entity<ComponentInSale>()
                .HasOne(p => p.ProductInSale)
                .WithMany(b => b.ComponentsInSale)
                .OnDelete(DeleteBehavior.Restrict);


            // Define Product -> OrderProduct relationship
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderedProducts)
                .HasForeignKey(op => op.ProductId);

            // Define Item -> OrderItem relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany(i => i.OrderedItems)
                .HasForeignKey(oi => oi.ItemId);


            // Pre-defined categories
/*            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Samosas" },
                new Category { CategoryId = 2, CategoryName = "Beverages" },
                new Category { CategoryId = 3, CategoryName = "Sauces" },
                new Category { CategoryId = 4, CategoryName = "Sides" }
            };
*/

            // New section: Add categories to modelBuilder
/*            modelBuilder.Entity<Category>().HasData(categories);
*/            // Items for different categories
            // Define Items
/*            var foodItemNames = new List<Item>
{
    // Items for Samosas
    new Item { ItemId = 1, ItemName = "Potato" },
    new Item { ItemId = 2, ItemName = "Onion" },
    new Item { ItemId = 3, ItemName = "Ground Beef" },
    new Item { ItemId = 4, ItemName = "Chicken" },
    new Item { ItemId = 5, ItemName = "Spices" },

    
    // Items for Sauces
    new Item { ItemId =6, ItemName = "Tamarind" },
    new Item { ItemId = 7, ItemName = "Chili Peppers" },
    new Item { ItemId = 8, ItemName = "Garlic" },
    new Item { ItemId = 9, ItemName = "Tomato" },
    new Item { ItemId = 10, ItemName = "Sugar" },

    // Items for Sides
    new Item { ItemId = 11, ItemName = "Yogurt" },
    new Item { ItemId = 12, ItemName = "Cucumber" },
    new Item { ItemId = 13, ItemName = "Cheese" },
    new Item { ItemId = 14, ItemName = "Bread" },
    new Item { ItemId = 15, ItemName = "Rice" },

      new Item { ItemId = 16, ItemName = "Cooking Oil" },
    new Item { ItemId = 17, ItemName = "All-Purpose Flour" },
    new Item { ItemId = 18, ItemName = "Water" },
    new Item { ItemId = 19, ItemName = "Salt" },
    new Item { ItemId = 20, ItemName = "Garam Masala" },
    new Item { ItemId = 21, ItemName = "Cumin Seeds" },
    new Item { ItemId = 22, ItemName = "Green Chili" },
    new Item { ItemId = 23, ItemName = "Coriander Leaves" },
    new Item { ItemId = 24, ItemName = "Peas" },
    new Item { ItemId = 25, ItemName = "Lemon Juice" }
};
*/
            // Define Products
/*            var foodProducts = new List<Product>
{
    // Products - Samosas
    new Product { ProductId = 1, ProductName = "Potato Samosa" },
    new Product { ProductId = 2, ProductName = "Beef Samosa" },
    new Product { ProductId = 3, ProductName = "Chicken Samosa" },
    new Product { ProductId = 4, ProductName = "Mince Samosa" },
    new Product { ProductId = 5, ProductName = "Chicken And Mince Samosa" },
new Product { ProductId = 6, ProductName = "Beef Samosa Extra Beef" },
new Product { ProductId = 7, ProductName = "Veggie Samosa" },
new Product { ProductId = 8, ProductName = "Spicy Chicken Samosa" },
new Product { ProductId = 9, ProductName = "Lamb Samosa" },
new Product { ProductId = 10, ProductName = "Cheese and Spinach Samosa" },


    // Products - Sauces
    new Product { ProductId = 11, ProductName = "Tamarind Sauce" },
    new Product { ProductId = 12, ProductName = "Spicy Chili Sauce" },

    // Products - Sides
    new Product { ProductId = 13, ProductName = "Cucumber Raita" },
    new Product { ProductId = 14, ProductName = "Garlic Bread" },
    new Product { ProductId = 15, ProductName = "Cheesy Fries" },
    new Product { ProductId = 16, ProductName = "Steamed Rice" },
    new Product { ProductId = 17, ProductName = "Garlic Naan" }
};
*/
            // Define Product-Item mapping
            // Complete mapping
            // Define Product-Item mapping
   /*         var productItemMapping = new List<ProductItemMap>
{
    // Samosas
    new ProductItemMap { ProductId = 1, ItemIds = new List<int> {1, 5, 16, 17, 18, 19} },  // Potato Samosa
    new ProductItemMap { ProductId = 2, ItemIds = new List<int> {3, 5, 16, 17, 18, 19} },  // Beef Samosa
    new ProductItemMap { ProductId = 3, ItemIds = new List<int> {4, 5, 16, 17, 18, 19} },  // Chicken Samosa
    new ProductItemMap { ProductId = 4, ItemIds = new List<int> {3, 5, 16, 17, 18, 19} },  // Mince Samosa
    new ProductItemMap { ProductId = 5, ItemIds = new List<int> {3, 4, 5, 16, 17, 18, 19} },  // Chicken And Mince Samosa
    new ProductItemMap { ProductId = 6, ItemIds = new List<int> {3, 5, 16, 17, 18, 19} },  // Beef Samosa (Duplicate)
    new ProductItemMap { ProductId = 7, ItemIds = new List<int> {1, 2, 5, 16, 17, 18, 19} },  // Veggie Samosa
    new ProductItemMap { ProductId = 8, ItemIds = new List<int> {4, 5, 22, 16, 17, 18, 19} },  // Spicy Chicken Samosa
    new ProductItemMap { ProductId = 9, ItemIds = new List<int> {5, 16, 17, 18, 19} },  // Lamb Samosa (needs more details)
    new ProductItemMap { ProductId = 10, ItemIds = new List<int> {13, 5, 16, 17, 18, 19} },  // Cheese and Spinach Samosa

    // Sauces
    new ProductItemMap { ProductId = 11, ItemIds = new List<int> {6, 10} },  // Tamarind Sauce
    new ProductItemMap { ProductId = 12, ItemIds = new List<int> {7, 8, 9} },  // Spicy Chili Sauce

    // Sides
    new ProductItemMap { ProductId = 13, ItemIds = new List<int> {11, 12} },  // Cucumber Raita
    new ProductItemMap { ProductId = 14, ItemIds = new List<int> {8, 14} },  // Garlic Bread
    new ProductItemMap { ProductId = 15, ItemIds = new List<int> {13, 15} },  // Cheesy Fries
    new ProductItemMap { ProductId = 16, ItemIds = new List<int> {15} },  // Steamed Rice

};

*/


/*            SeedData(modelBuilder, foodItemNames, foodProducts, categories, productItemMapping);
*/


        }






        public enum AmountUsedUnitOfMeasurement
        {
            Count,
            Liter,
            Millileter,
            Kilogram,
            Grams,
        }


/*        public void SeedData(
            ModelBuilder modelBuilder,
            List<Item> foodItemNames,
            List<Product> foodProducts,
            List<Category> categories,
         List<ProductItemMap> productItemMapping)// Added this line
        {



            // Users Corect
            var userFaker = new Faker<User>()
                    .RuleFor(u => u.UserId, f => f.UniqueIndex + 1)  // Use unique positive values for UserId
                    .RuleFor(u => u.Username, f => f.Name.FirstName())
                    .RuleFor(u => u.UserPassword, "12345678")  // Set a fixed password for all users
                    .RuleFor(u => u.UserEmail, (f, u) => f.Internet.Email(u.Username))
                    .RuleFor(u => u.UserContact, f => f.Phone.PhoneNumberFormat(0))
                    .RuleFor(u => u.Role, f => f.PickRandom<UserRole>());

            var users = userFaker.Generate(5);
            modelBuilder.Entity<User>().HasData(users);



            // Define a list of real-world supplier names suitable for a samoosa shop Correct
            var realWorldSupplierNames = new List<string>
{
    "Spice Distributors", "Vegetable Wholesalers", "Meat Suppliers", "Oil & Fats Co.",
    "Spice Farm Inc", "Baker Wholesaler", "Organic Growers", "Poultry Ltd", "Bakery Essentials", "Goods Whole Saler"
};

            // Suppliers
            var suppliersFaker = new Faker<Supplier>()
                .RuleFor(s => s.SupplierId, f => -(f.UniqueIndex + 1))  // Providing unique negative values for SupplierId
                .RuleFor(s => s.SupplierName, f => f.PickRandom(realWorldSupplierNames))
                .RuleFor(s => s.EmailAddress, f => f.Internet.Email())
                .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber().Substring(0, 9)) // Limit phone number length to 15 characters
                .RuleFor(s => s.ContactPerson, f => f.Name.FullName())
                .RuleFor(s => s.Street, f => f.Address.StreetAddress())
                .RuleFor(s => s.City, f => f.Address.City())
                .RuleFor(s => s.State, f => f.Address.State())
                .RuleFor(s => s.PostalCode, f => f.Address.ZipCode())
                .RuleFor(s => s.Rating, f => f.Random.Int(1, 5))
                .RuleFor(s => s.CreatedAt, f => f.Date.Past(2))
                .RuleFor(s => s.UpdatedAt, f => f.Date.Recent());

            var suppliers = suppliersFaker.Generate(10);
            modelBuilder.Entity<Supplier>().HasData(suppliers);



            // Generate SupplierIds
            var supplierIds = suppliers.Select(s => s.SupplierId).ToList();

            int counter = 1;

            // Create a Faker object for the Item class
            var nonExtras = new HashSet<string> { "Cooking Oil", "All-Purpose Flour", "Water", "Salt" };

            var itemFaker = new Faker<Item>()
                .RuleFor(i => i.ItemId, f => counter++)
                .RuleFor(i => i.ItemName, f => foodItemNames[counter - 1].ItemName)
                .RuleFor(i => i.UnitsInInventory, f => f.Random.Int(0, 100))
                .RuleFor(i => i.SupplierId, f => f.PickRandom(suppliers).SupplierId)
                .RuleFor(i => i.PricePerUnit, f => Math.Round(f.Random.Decimal(1.0M, 20.0M), 2))
                .RuleFor(i => i.PiecesPerUnit, f => f.Random.Int(1, 20))
                .RuleFor(i => i.MinimumThreshold, f => f.Random.Int(10, 50))
                .RuleFor(i => i.MaximumThreshold, f => f.Random.Int(50, 150))
                .RuleFor(i => i.Status, f => Item.ItemStatus.Active)
                .RuleFor(i => i.AvalibleOnExtra, f => !nonExtras.Contains(foodItemNames[counter - 1].ItemName))
                .FinishWith((f, i) =>
                {

                    if (i.ItemName.Contains("Potato") || i.ItemName.Contains("Onion") ||
                        i.ItemName.Contains("Ground Beef") || i.ItemName.Contains("Chicken") ||
                        i.ItemName.Contains("Spices"))
                    {
                        i.UnitOfMeasurement = Item.ItemUnitsOfMeasurement.Kilogram;
                    }
                    else if (i.ItemName.Contains("Tamarind") || i.ItemName.Contains("Sugar") ||
                             i.ItemName.Contains("Yogurt"))
                    {
                        i.UnitOfMeasurement = Item.ItemUnitsOfMeasurement.Liter;
                    }
                    else if (i.ItemName.Contains("Chili Peppers") || i.ItemName.Contains("Garlic") ||
                             i.ItemName.Contains("Tomato") || i.ItemName.Contains("Cucumber") ||
                             i.ItemName.Contains("Cheese") || i.ItemName.Contains("Bread") ||
                             i.ItemName.Contains("Rice"))
                    {
                        i.UnitOfMeasurement = Item.ItemUnitsOfMeasurement.Grams;
                    }
                    if (i.PiecesPerUnit.HasValue && i.PiecesPerUnit > 0)
                    {
                        i.PricePerPiece = i.PricePerUnit / i.PiecesPerUnit;
                    }
                });

            // Generate fake items
            var fakeItems = itemFaker.Generate(24);
            modelBuilder.Entity<Item>().HasData(fakeItems);



            int productIdCounter = 1;  // Counter for ProductId

            // Create a Faker object for the Product class
            var productFaker = new Faker<Product>()
     .RuleFor(p => p.ProductId, f => productIdCounter++)
     .RuleFor(p => p.ProductName, f => foodProducts[productIdCounter - 1].ProductName)
     .RuleFor(p => p.CategoryId, (f, p) => {
         if (p.ProductName.Contains("Samosa"))
             return 1; // Samosas
         else if (p.ProductName.Contains("Sauce"))
             return 3; // Sauces
         else if (p.ProductName.Contains("Naan") || p.ProductName.Contains("Bread") || p.ProductName.Contains("Fries") || p.ProductName.Contains("Rice"))
             return 4; // Sides
         else
             return 2; // Beverages (or other default)
     })
     .RuleFor(p => p.UnitsInInventory, f => f.Random.Number(0, 100))
     .RuleFor(p => p.MarkupPercentage, f => f.Random.Decimal(0, 100))
     .RuleFor(p => p.MinimumThreshold, f => f.Random.Number(0, 10))
     .RuleFor(p => p.MaximumThreshold, f => f.Random.Number(10, 100))
     .RuleFor(p => p.Status, Product.ProductStatus.Active)
     .RuleFor(p => p.UnitOfMeasurement, f => f.PickRandom<Product.ProductUnitsOfMeasurement>())
     .RuleFor(p => p.LastSoldDate, f => f.Date.Past())
     .RuleFor(p => p.DateCreated, f => f.Date.Past())
                .FinishWith((f, p) =>
                {
                    var relatedItems = productItemMapping
                                        .FirstOrDefault(map => map.ProductId == p.ProductId)?
                                        .ItemIds
                                        .Select(id => fakeItems.FirstOrDefault(item => item.ItemId == id))
                                        .ToList();

                    if (relatedItems != null)
                    {
                        // Calculate total cost for product
                        decimal totalCostPrice = (decimal)relatedItems.Sum(item => item.PricePerPiece);

                        // Calculate selling price based on markup
                        decimal sellingPrice = totalCostPrice * (1 + (p.MarkupPercentage / 100));

                        p.ProductCostPrice = totalCostPrice;
                        p.ProductSellingPrice = sellingPrice;
                    }
                });



            // Generate fake products
            var fakeProducts = productFaker.Generate(16);
            modelBuilder.Entity<Product>().HasData(fakeProducts);


            // List of beverage names
            var beverageNames = new List<string>
{
    "Coca-Cola",
    "Pepsi",
    "Sprite",
    "Mountain Dew",
    "Dr. Pepper",
    "Fanta",
    "Gatorade",
    "Red Bull",
    "Monster",
    "7-Up"
};


            var maxProductIdInProducts = 10;  // Replace this with actual maximum ProductId in 'products' list if it's dynamic

            var beverageProductsFaker = new Faker<Product>()
                .RuleFor(p => p.ProductId, f => maxProductIdInProducts + (f.UniqueIndex + 1))
                .RuleFor(p => p.ProductName, f => f.PickRandom(beverageNames))  // Use beverage names
                .RuleFor(p => p.CategoryId, 2)  // Beverages category
                .RuleFor(p => p.UnitsInInventory, f => f.Random.Int(1, 1000))
                .RuleFor(p => p.MarkupPercentage, f => f.Random.Decimal(1, 100))
                .RuleFor(p => p.MinimumThreshold, f => f.Random.Int(1, 100))
                .RuleFor(p => p.MaximumThreshold, f => f.Random.Int(101, 150))
                .RuleFor(p => p.Status, f => f.PickRandom<Product.ProductStatus>())
                .RuleFor(p => p.UnitOfMeasurement, f => Product.ProductUnitsOfMeasurement.Liter)
                .RuleFor(p => p.SupplierId, f => f.PickRandom(suppliers).SupplierId)
                .RuleFor(p => p.LastSoldDate, f => f.Date.Past(1))
                .RuleFor(p => p.DateCreated, f => f.Date.Past(2))
                .RuleFor(p => p.ProductCostPrice, f => f.Random.Decimal(5, 20))  // Cost price between 5 to 20
                .RuleFor(p => p.ProductSellingPrice, (f, u) => u.ProductCostPrice + (u.ProductCostPrice * (u.MarkupPercentage / 100)))  // Selling price based on cost price and markup percentage
                .RuleFor(p => p.LinkedItems, f => new List<ProductToItem>());  // Empty list of linked items

            // Generate 10 beverage products
            var beverageProducts = beverageProductsFaker.Generate(10);

            // Add these products to the modelBuilder
            modelBuilder.Entity<Product>().HasData(beverageProducts);



            var productToItemData = new List<ProductToItem>
{
    // Samosas
    // Potato Samosa
    new ProductToItem { LinkId = 1,ProductId = 1, ItemId = 1, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 2,ProductId = 1, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 3,ProductId = 1, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 4,ProductId = 1, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 5,ProductId = 1, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 6,ProductId = 1, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Beef Samosa
    new ProductToItem { LinkId = 7,ProductId = 2, ItemId = 3, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 8,ProductId = 2, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 9,ProductId = 2, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 10,ProductId = 2, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 11,ProductId = 2, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 12,ProductId = 2, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

   // Chicken Samosa
    new ProductToItem { LinkId = 13,ProductId = 3, ItemId = 4, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 14, ProductId = 3, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem {LinkId = 15, ProductId = 3, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 16,ProductId = 3, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 17, ProductId = 3, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 18, ProductId = 3, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Mince Samosa
    new ProductToItem {LinkId = 19, ProductId = 4, ItemId = 3, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 20, ProductId = 4, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 21,ProductId = 4, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 22,ProductId = 4, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 23, ProductId = 4, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 24, ProductId = 4, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    // Chicken And Mince Samosa
    new ProductToItem { LinkId = 25,ProductId = 5, ItemId = 3, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 26, ProductId = 5, ItemId = 4, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 27, ProductId = 5, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 28,ProductId = 5, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 29, ProductId = 5, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 30,ProductId = 5, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 31,ProductId = 5, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Beef Samosa (Duplicate)
    new ProductToItem {LinkId = 32, ProductId = 6, ItemId = 3, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 33,ProductId = 6, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 34,ProductId = 6, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 35,ProductId = 6, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 36, ProductId = 6, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 37, ProductId = 6, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Veggie Samosa
    new ProductToItem { LinkId = 38,ProductId = 7, ItemId = 1, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 39,ProductId = 7, ItemId = 2, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 40,ProductId = 7, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem { LinkId = 41,ProductId = 7, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 42, ProductId = 7, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 43, ProductId = 7, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 44,ProductId = 7, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
 // Spicy Chicken Samosa
    new ProductToItem {LinkId = 45, ProductId = 8, ItemId = 4, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 46,ProductId = 8, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem {LinkId = 47, ProductId = 8, ItemId = 22, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 48,ProductId = 8, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 49,ProductId = 8, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 50, ProductId = 8, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 51, ProductId = 8, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Lamb Samosa (needs more details)
    new ProductToItem { LinkId = 52,ProductId = 9, ItemId = 5, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 53, ProductId = 9, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 54, ProductId = 9, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 55,ProductId = 9, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 56,ProductId = 9, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Cheese and Spinach Samosa
    new ProductToItem {LinkId = 57, ProductId = 10, ItemId = 13, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 58, ProductId = 10, ItemId = 5, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },
    new ProductToItem {LinkId = 59, ProductId = 10, ItemId = 16, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 60, ProductId = 10, ItemId = 17, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 61, ProductId = 10, ItemId = 18, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 62,ProductId = 10, ItemId = 19, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

        // Tamarind Sauce
    new ProductToItem { LinkId = 63,ProductId = 11, ItemId = 6, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 64,ProductId = 11, ItemId = 10, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Spicy Chili Sauce
    new ProductToItem { LinkId = 65,ProductId = 12, ItemId = 7, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem { LinkId = 66,ProductId = 12, ItemId = 8, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 67, ProductId = 12, ItemId = 9, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Cucumber Raita
    new ProductToItem {LinkId = 68, ProductId = 13, ItemId = 11, AmountUsed = 0.1M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },
    new ProductToItem {LinkId = 69, ProductId = 13, ItemId = 12, AmountUsed = 0.05M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Liter },

    // Garlic Bread
    new ProductToItem {  LinkId = 70,   ProductId = 14, ItemId = 8, AmountUsed = 1, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem { LinkId = 71,ProductId = 14, ItemId = 14, AmountUsed = 0.2M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },

    // Cheesy Fries
    new ProductToItem {LinkId = 72,  ProductId = 15, ItemId = 13, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Count },
    new ProductToItem {LinkId = 73,  ProductId = 15, ItemId = 15, AmountUsed = 0.3M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },

    // Steamed Rice
    new ProductToItem {LinkId = 74, ProductId = 16, ItemId = 15, AmountUsed = 0.5M, AmountUsedMeasurement = ProductToItem.AmountUsedUnitOfMeasurement.Kilogram },




};



            modelBuilder.Entity<ProductToItem>().HasData(productToItemData);



            ////---------------------orders------------------------------

            // ItemOrders
            var itemOrdersFaker = new Faker<OrderItem>()
                .RuleFor(io => io.OrderedItemId, f => (f.UniqueIndex + 1))  // Providing unique negative values for OrderedItemId
                      .RuleFor(io => io.SupplierOrderId, f => 0)
                .RuleFor(io => io.ItemId, f => f.PickRandom(fakeItems).ItemId)   // Use valid ItemId values from the generated items
                .RuleFor(io => io.PiecesPerUnit, f => f.Random.Int(1, 100))   // Generating PiecesPerUnit value
                .RuleFor(io => io.UnitsOrdered, f => f.Random.Int(1, 20))
                .RuleFor(io => io.TotalCostOfOrder, (f, io) => f.Finance.Amount(1, 5) * io.UnitsOrdered * io.PiecesPerUnit)  // The total cost is derived from UnitsOrdered multiplied by a random price per piece.
                .RuleFor(io => io.OrderDate, f => f.Date.Past(1))
                .RuleFor(io => io.Status, f => OrderItem.OrderItemStatus.Shipped);

            var itemOrders = itemOrdersFaker.Generate(10);





            // SupplierOrders
            var supplierOrdersFaker = new Faker<SupplierOrder>()
                .RuleFor(so => so.SupplierOrderId, f => (f.UniqueIndex + 1))  // Providing unique negative values for SupplierOrderId
          .RuleFor(so => so.SupplierId, f => f.PickRandom(suppliers).SupplierId) // Existing line
    .RuleFor(so => so.SupplierName, (f, so) => suppliers.First(s => s.SupplierId == so.SupplierId).SupplierName)  // New line  // Use valid SupplierIds from the generated suppliers
        .RuleFor(so => so.TotalCostOfOrder, (f, so) =>
        {
            // Sum the total costs of item orders and product orders
            return itemOrders.Sum(io => io.TotalCostOfOrder);
        })
                .RuleFor(so => so.LeadTimeFromSupplier, f => f.Random.Int(1, 30))

                .RuleFor(so => so.OrderDate, f => f.Date.Past(1))
                .RuleFor(so => so.ExpectedDeliveryDate, f => f.Date.Soon(30))
                .RuleFor(so => so.Status, f => SupplierOrder.StatusOfOrder.Confirmed)
                .RuleFor(so => so.Rating, f => f.Random.Int(1, 5))
                .RuleFor(so => so.PlacedByUserId, f => f.Random.Int(1, 5))  // Adjusted to pick from 5 generated users
                .RuleFor(so => so.TrackingNumber, f => f.Random.Replace("???-???-???"))
                .RuleFor(so => so.ActualDeliveryDate, (f, so) => so.Status == SupplierOrder.StatusOfOrder.Arrived ? f.Date.Between(so.OrderDate, DateTime.Now) : (DateTime?)null);  // Only set if the order has arrived

            var supplierOrders = supplierOrdersFaker.Generate(10);
            modelBuilder.Entity<SupplierOrder>().HasData(supplierOrders);

            // Now update SupplierOrderId in ItemOrders and OrderProducts
            for (int i = 0; i < itemOrders.Count; i++)
            {
                itemOrders[i].SupplierOrderId = supplierOrders[i % supplierOrders.Count].SupplierOrderId;
            }


            // Re-add the updated ItemOrders and OrderProducts to the modelBuilder
            modelBuilder.Entity<OrderItem>().HasData(itemOrders);



            ////---------------sales-------------------------------------------
            //Sales
            var feedbackPhrases = new List<string>
{
    "It was good",
    "Add more spice",
    "Loved it",
    "Could be better",
    "Too salty",
    "Just perfect",
    "Not what I expected",
    "Excellent service",
    "Quick delivery",
    "Would buy again"
};

            var userIds = users.Select(u => u.UserId).ToList();

            //redo The Enitire Sales logic, A sale Must Contain Products The total Of that Sale Should be the total of the products in the Sale
            // Generate sales for the last 30 days
            // Generate sales for the last 30 days
            var startDate = DateTime.Now.AddDays(-30);
            List<Sale> sales = new List<Sale>();
            int uniqueSaleId = 1; // Start SaleId from 1 and maintain its uniqueness across iterations

            for (DateTime date = startDate; date <= DateTime.Now; date = date.AddDays(1))
            {
                // Generating a random number of sales for this day
                int numSalesToday = new Random().Next(1, 10);  // 1 to 9 sales

                for (int i = 0; i < numSalesToday; i++)
                {
                    var sale = new Faker<Sale>()
                        .RuleFor(s => s.SaleId, f => uniqueSaleId++)  // Increment the uniqueSaleId each time
                        .RuleFor(s => s.UserId, f => f.PickRandom(userIds))
                        .RuleFor(s => s.SaleDate, date)
                        .RuleFor(s => s.PaymentType, f => f.PickRandom<Sale.PaymentMethod>())
                        .RuleFor(s => s.AdditionalInfo, f => f.PickRandom(feedbackPhrases))
                        .Generate();

                    sales.Add(sale);
                }
            }




            // Generate at least 3 products per sale
            var productsInSale = new List<ProductInSale>();
            foreach (var sale in sales)
            {
                decimal subtotal = 0;

                var productInSalesFaker = new Faker<ProductInSale>()
                    .RuleFor(p => p.ProductInSaleId, f => f.UniqueIndex + 1)
                    .RuleFor(p => p.ProductId, f => f.PickRandom(fakeProducts).ProductId)
                    .RuleFor(p => p.SaleId, sale.SaleId)
                    .RuleFor(p => p.Quantity, f => f.Random.Number(1, 10))
            .RuleFor(p => p.ProductSellingPrice, (f, p) => fakeProducts.First(prod => prod.ProductId == p.ProductId).ProductSellingPrice);
                // Use random selling price or retrieve from Product model

                var productInSalesForThisSale = productInSalesFaker.Generate(3); // Generating 3 ProductInSale objects for each Sale

                foreach (var pis in productInSalesForThisSale)
                {
                    subtotal += pis.ProductSellingPrice * pis.Quantity;
                }

                sale.Subtotal = subtotal;
                sale.Tax = subtotal * 0.16m;  // Assuming a 10% tax rate
                sale.Total = subtotal + sale.Tax;

                productsInSale.AddRange(productInSalesForThisSale);
            }

            modelBuilder.Entity<ProductInSale>().HasData(productsInSale);


            // Create a lookup from ProductId to a list of ItemIds
            var productToComponents = productItemMapping.ToDictionary(p => p.ProductId, p => p.ItemIds);

            // The product has actual components
            // Initialize a random number generator
            Random random = new Random();

            // Initialize a ComponentInSaleId counter
            int componentInSaleIdCounter = 1;  // Assuming you start from 1
                                               // Create a dictionary for quick lookup of ProductToItem by ProductId
            var productToItemDataDict = productToItemData.GroupBy(pti => pti.ProductId)
                                                          .ToDictionary(g => g.Key, g => g.ToList());

            var componentsInSale = new List<ComponentInSale>();
            foreach (var productInSale in productsInSale)
            {
                List<int> actualComponentItemIds;
                if (productToComponents.TryGetValue(productInSale.ProductId, out actualComponentItemIds))
                {
                    foreach (var itemId in actualComponentItemIds)
                    {
                        // Lookup the corresponding ProductToItem object
                        var productToItem = productToItemDataDict[productInSale.ProductId].FirstOrDefault(pti => pti.ItemId == itemId);

                        if (productToItem != null)
                        {
                            var componentInSale = new ComponentInSale()
                            {
                                ComponentInSaleId = componentInSaleIdCounter++,  // Assign and then increment the ComponentInSaleId counter
                                ProductInSaleId = productInSale.ProductInSaleId,
                                ItemId = itemId,
                                AmountUsed = productToItem.AmountUsed // Use the AmountUsed from ProductToItem
                            };

                            componentsInSale.Add(componentInSale);
                        }
                        else
                        {
                            // The itemId does not exist in the ProductToItem data for this ProductId. Handle accordingly.
                        }
                    }
                }
                else
                {
                    // The product has no actual components, so either skip or handle as you wish
                }
            }


            modelBuilder.Entity<ComponentInSale>().HasData(componentsInSale);

            modelBuilder.Entity<Sale>().HasData(sales);



        }
*/
    }
}
