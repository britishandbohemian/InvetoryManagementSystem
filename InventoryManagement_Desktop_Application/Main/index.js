// Import required modules and functions and constants
const express = require("express");
const app = express();
const path = require("path");
const methodOverride = require("method-override");
app.use(methodOverride("_method"));
const bodyParser = require("body-parser");
app.use(bodyParser.json());
app.use(express.urlencoded({ extended: true }));
const session = require("express-session");




//--------------- Import Functions From functions js
const {
recommendSales,
totalProfit,
buildCategoryNarrative,
  getMostUsedItems,
  generateItemPerformanceNarrative,
  getAggregateItemMetrics,
  getItemsExpiringSoon,
  getItemsByStockLevels,
  fetchBestSellingProducts,
  getOrderDetails,
StoreNarrative,
  ConfirmOrder,
  getProductInformation,
  orderItem,
  createSale,
  fetchProfitStats,
  fetchAllSales,
  loginUser,
  orderComponent,
  registerUser,
  getComponent,
  fetchSalesInDateRange,
  orderProduct,
  getSuppliers,
  SalesRevenue,
  getSupplierById,
  addProducts,
  fetchPredictedStockForToday,
  createComponent,
  getAllComponents,
  getAllOrders,
  fetchItemsRunningLow,
  addSupplier,
  getAllCategories,
  deleteSupplier,
  updateSupplier,
  getAllUsers,
  deleteUser,
Potentialprofit,
  orderFromSupplier,
  createCategory,
  getUserById,
  fetchProductsThatNeedReordering,
  updateUser,
  getAllProducts,
    fetchAllOrderProducts,
  fetchAllOrderItems,
    fetchTotalProducts,
  fetchTotalWorthProducts,
  getProductById,
  fetchAllOrders,
  confirmOrder,
  addProductsWithComponent,
  updateCategoryName,
    getProductsByStockLevels,
  getLeastSellingProducts,
  getTopSellingProducts,
  getProductsNearExpiration,
  getOverstockedProducts,
  getLowStockProducts,
  getTotalProducts,
filterCategoriesByWorthLevel
} = require("./routes/ApplicationFunctions");


//------------------------- Needed to Run App -------------------------------
// Serve static files from the 'public' directory
app.use(express.static("public"));

app.use(
  session({
    secret: "mySecretKey", // Use a more complex key for production.
    resave: false,
    saveUninitialized: true
  })
);

// Enable json and urlencoded for post requests
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

// Set the views directory and use EJS as the template engine
app.set("views", path.join(__dirname, "views"));
app.set("view engine", "ejs");

//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//-----------------------------ROUTES-----------------------------

//SHOW PAGES------------------------------

app.get("/", (req, res) => {
  const message = "";
  res.render("User/login", { message: message });
});

//REGISTER PAGE SHOW
app.get("/Register", (req, res) => {
  const message = "";
  res.render("User/register", { message: message });
});

//FUNCTION TO REGISTER A USER
app.post("/api/auth/register", async (req, res) => {
  const {
    username,
    userPassword,
    userEmail,
    userContact,
    role: Inrole
  } = req.body;

  let role = 0;
  switch (Inrole) {
    case "user":
      role = 1;
      break;
    case "admin":
      role = 2;
      break;
    case "supplier":
      role = 4;
      break;
    default:
      break;
  }

  try {
    const user = await registerUser(
      username,
      userPassword,
      userEmail,
      userContact,
      role // Convert the role to an integer
    );

    if (!user) {
      // If the registration is unsuccessful for any reason
      const message = "Failed to register user";
      return res.status(400).render("User/register", { message }); // 400 Bad Request
    }

    const message = ""; // Message can be used to display a success message if needed
    res.redirect("/");
  } catch (error) {
    const message = "Failed To register User";
    res.status(500).render("User/register", { message }); // 500 Internal Server Error
  }
});

//FUNCTION TO LOGIN A USER
app.post("/api/auth/Login", async (req, res) => {
  const { username, userPassword } = req.body;
  try {
    const user = await loginUser(username, userPassword);

    if (!user) {
      // Check if user is not found
      const message = "Invalid credentials";
      return redirect("/Login", message); // Rendering login page with error message
    }

    req.session.user = user;

    // Switch based on user role
    switch (user.role) {
      case 1:
        return res.redirect("/SellItems");
      case 2:
        return res.redirect("/homeAdmin");
      case 3:
        return res.redirect("/homeAdmin");
      default:
        return res.redirect("/SellItems"); // Default view if no role matches
    }
  } catch (error) {
    const message = "Failed to Login, Invalid Credentials";
    return res.status(500).render("User/Login", { message }); // Rendering login page with error message
  }
});

app.get("/ViewSales", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }


  const Items = await getAllComponents();
  const Products = await getAllProducts();
  const Supplier = await getSuppliers();
  const Orders = await fetchAllOrders();


const Narrative = await StoreNarrative();


  res.render("User/Admin/Sales/Sales", {
    Supplier,
    Items,
    Products,
    Orders,
Narrative,// Send all sales data for the date range to the EJS template
    user: req.session.user,
  });
});

app.post("/delete/:userId", async (req, res) => {
  const userId = req.params.userId;
  try {
    // Assuming deleteUser is a function that deletes a user by ID and returns the deleted user
    const deletedUser = await deleteUser(userId);

    if (!deletedUser) {
      res.status(404).send('User not found');
      return;
    }

    res.status(200).send('User deleted successfully');
  } catch (error) {
    res.status(500).send('An error occurred: ' + error.message);
  }
});


//ADMIN PAGE

app.get("/homeAdmin", async (req, res) => {
  //Redirect to Login Page
  if (!req.session.user) {
    return res.redirect("/");
  }

  //Variables Passed to admin Page
  const Profit = await fetchProfitStats();
  const ProductsRunningLow = await fetchProductsThatNeedReordering();
  const ItemsRunningLow = await fetchItemsRunningLow();
let parsedData = JSON.parse(await recommendSales());
const Recommendations = parsedData;

const SalesRev = await SalesRevenue();

  //render Page
  res.render("User/Admin/homeAdmin", {
Recommendations,
    Profit,
    ItemsRunningLow,
    ProductsRunningLow,
    SalesRev,
    user: req.session.user
  });
});

app.get("/OrderProductsAndItems", async (req, res) => {
  //Redirect to Login Page
  if (!req.session.user) {
    return res.redirect("/");
  }

  //Variables Passed to admin Page
  const Supplier = await getSuppliers();
  const Products = await getAllProducts();
  const Items = await getAllComponents();

  const OrderProducts = await fetchAllOrderProducts();
  const OrderItems = await fetchAllOrderItems();

  //render Page
  res.render("User/Admin/Orders/ListOfProductsAndItems", {
    Items,
    Products,
    Supplier,
    Orders: {
      orderProducts: OrderProducts,
      orderItems: OrderItems
    },
    user: req.session.user
  });
});


//OPTIONS PAGE
//GET OPTIONS
app.get("/Options", (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("Options", { user: req.session.user });
});

//GET DATA ROUTES---------------------------

//GET CATEGORIES IN THE SYSTEM
//Color Code Categories
app.get("/GetCategories", async (req, res) => {
  const Categories = await getAllCategories();


  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("User/Admin/Categories/ViewCategories", {
    Categories,
    user: req.session.user
  });
});


app.get("/EditProduct/:productId", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  const productId = req.params.productId;

  const Categories = await getAllCategories();
  const product = await getProductById(productId);
  const Components = product.Items; // Extract components from the product



  res.render("User/Admin/Products/EditProduct", {
    product,
    Categories,
    Components, // Pass components to the template
    user: req.session.user
  });
});

app.get("/getItems", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Components = await getAllComponents();
  // If the user session exists, render the add product page
res.send(Components);
});

app.get("/ViewItems", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Categories = await getAllCategories();
  const Supplier = await getSuppliers();
  const Components = await getAllComponents();
  const ItemsRunningLow = await fetchItemsRunningLow();


  // If the user session exists, render the add product page
  res.render("User/Admin/Items/ViewItems", {
    Components,
    Categories,
    Supplier,ItemsRunningLow,
    user: req.session.user
    // You can pass additional data to the template here
  });
});

app.post("/CreateComponent", async (req, res) => {
  try {
    const {
      itemName,
      unitOfMeasurement,
      minimumThreshold,
      maximumThreshold,
      supplierId,
      categoryId
    } = req.body;


    // Create the componentDto object with default values and incoming request data
    const componentDto = {
      itemName: itemName || "Default Name",
      categoryId: categoryId || 0,
      unitOfMeasurement: unitOfMeasurement || 0,
      pricePerUnit: 0,
      piecesPerUnit: 0,
      expiryDate: "2023-09-18T13:38:48.188Z", // You can also use a value from the req.body, if provided
      minimumThreshold: minimumThreshold || 0,
      maximumThreshold: maximumThreshold || 0,
      supplierId: supplierId || 0,
      status: 1,
      unitsInInventory: 0
    };



    // Then, you can use your existing createComponent function or database logic to add the component
    const response = await createComponent(componentDto);

    // Respond with the newly created component, including its ID
    res.send(response);
  } catch (error) {
    console.error("Error creating component:", error);
    res.status(500).send(error.message);
  }
});

app.post("/api/categories", async (req, res) => {
  try {
    // Here, you can extract the data from the request body
    const category = req.body;

    const CategoryDto = {
      categoryName: category.categoryName
    };



    // Then, you can use your existing createCategory function or database logic to add the category
    const response = await createCategory(CategoryDto);

    // Respond with the newly created category, including its ID
    res.redirect("/GetCategories");
  } catch (error) {
    console.error("Error creating category:", error);
    res.status(500).send(error.message);
  }
});

app.post("/Addsupplier", async (req, res) => {
  try {
    // Validate req.body to ensure it has all required fields
    const { supplierName, emailAddress, phoneNumber, contactPerson, street, city, state, postalCode, rating } = req.body;

    if (!supplierName || !emailAddress || !phoneNumber || !contactPerson || !street || !city || !state || !postalCode || rating === undefined) {
      return res.status(400).json({ message: "Missing required fields" });
    }

    // Create a DTO (Data Transfer Object)
    const supplierDto = {
      supplierName,
      emailAddress,
      phoneNumber,
      contactPerson,
      street,
      city,
      state,
      postalCode,
      rating
    };

console.log(supplierDto)

    // Insert the supplier data into the database using your database middleware
    const response = await addSupplier(supplierDto);

    res.send(response);
  } catch (error) {
    console.error("Error:", error);
    res.status(500).json({ message: "There was an error processing your request." });
  }
});



//----WEB APPLICATION TO SELL PRODUCTS
//GET POINT OF SALE SYSTEM
app.get("/SellItems", async (req, res) => {
  //const products = await getAllProducts();
  const products = await getAllProducts();
  const categories = await getAllCategories();

  if (!req.session.user) {
    return res.redirect("/");
  }

  res.render("User/SellItems", {
    products,
categories,
    user: req.session.user
  });
});

app.get("/SellItemsAdmin", async (req, res) => {
  //const products = await getAllProducts();
  const products = await getAllProducts();
  const categories = await getAllCategories();

  if (!req.session.user) {
    return res.redirect("/");
  }

  res.render("User/Admin/SellItems", {
    products,
categories,
    user: req.session.user
  });
});

app.post("/MakeSale", async (req, res) => {
  try {
    const saleData = req.body;
    const result = await createSale(saleData); // createSale should be defined somewhere in your code
    res.send(result);
  } catch (error) {
    res.status(500).send(`Error: ${error.message}`);
  }
});

//GET ALL PRODUCTS TO RENDER OUT THE CART
app.get("/Cart", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  const products = await getAllProducts();
  res.render("User/CustomerCart", { products, user: req.session.user });
});


// Define the route for getting products by stock levels
app.get('/ProductsByStockLevels/:level', async (req, res) => {
  try {
    const level = req.params.level;
    const products = await getProductsByStockLevels(level);
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

// Define the route for getting least selling products
app.get('/LeastSellingProducts/:bottom', async (req, res) => {
  try {
    const bottom = req.params.bottom;
    const products = await getLeastSellingProducts(bottom);
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

// Define the route for getting top selling products
app.get('/TopSellingProducts/:top', async (req, res) => {
  try {
    const top = req.params.top;
    const products = await getTopSellingProducts(top);
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

// ... Repeat similar blocks for each of the other functions ...

// Define the route for getting products near expiration
app.get('/ProductsNearExpiration/:daysThreshold', async (req, res) => {
  try {
    const daysThreshold = req.params.daysThreshold;
    const products = await getProductsNearExpiration(daysThreshold);
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

// Define the route for getting overstocked products
app.get('/OverstockedProducts', async (req, res) => {
  try {
    const products = await getOverstockedProducts();
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

// Define the route for getting low stock products
app.get('/LowStockProducts', async (req, res) => {
  try {
    const products = await getLowStockProducts();
    res.json(products);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});



app.get('/MostUsedItems', async (req, res) => {
  try {
    const items = await getMostUsedItems();
    res.send(items);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});


app.get('/ItemsExpiringSoon/:days', async (req, res) => {
  try {
    const days = req.params.days;
    const items = await getItemsExpiringSoon(days);
    res.send(items);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});


app.get('/ItemsByStockLevels/:level', async (req, res) => {
  try {
    const level = req.params.level;
    const items = await getItemsByStockLevels(level);
    res.send(items);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});



app.get('/ItemPerformanceNarrative/:id', async (req, res) => {
  try {
    const itemId = req.params.id;
    const narrative = await generateItemPerformanceNarrative(itemId);
    res.send(narrative);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});





app.get('/CategoryNarrative/:id', async (req, res) => {
  try {
    const categoryId = req.params.id;
    const narrative = await buildCategoryNarrative(categoryId);
    res.send(narrative);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

app.get('/FilterCategories/:level', async (req, res) => {
  try {
    const level = req.params.level;
    const categories = await filterCategoriesByWorthLevel(level);
    res.send(categories);
  } catch (error) {
    console.error(error);
    res.status(500).send('Server Error');
  }
});

































//----- ADDING A PRODUCT TO THE SYSTEM I WILL MAKE IT THE BEST 3 PAGES IVE EVER DONE IN HONOR OF GOD
//SELECT TYPE OF PRODUCT
//1
//View Products Page
app.get("/ViewProducts", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  //const products = await getAllProducts();
  const products = await getAllProducts();
  const Categories = await getAllCategories();
  const Components = await getAllComponents();
  const ProductsRunningLow = await fetchProductsThatNeedReordering();
    const totalProducts = await getTotalProducts();


  // If the user session exists, render the add product page
  res.render("User/Admin/Products/ViewProducts", {
    products,
    totalProducts,
    Categories,
    Components,
    ProductsRunningLow,
    user: req.session.user
    // You can pass additional data to the template here
  });
});

app.post("/AddProductsFromSupplier", async (req, res) => {
  // Extract the product data from the request body
  const productData = req.body; // Ensure body-parser middleware is used

  try {
    // Add the product to the database (replace with your actual logic)
    const Product = await addProducts(productData);
    res.send(Product);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send({ status: 'error', message: error.message });
  }
});

// Define a GET route to handle fetching product information
app.get('/ProductInformation', async (req, res) => {
  const productId = req.query.productId;
console.log(productId)
  try {
    const productInfo = await getProductInformation(productId);
console.log(productInfo)
    if (productInfo) {
      res.json(productInfo);
      console.log(productInfo)
    } else {
      res.status(404).send('Product not found');
    }
  } catch (error) {
    res.status(500).send(error.message);
  }
});

// Start the server on port 3000
app.listen(3000, () => {
  console.log('Server is running on port 3000');
});


app.post("/AddProductsMadeInStore", async (req, res) => {
  // Extract the product data from the request body
  const productData = req.body; // Ensure body-parser middleware is used

  try {

    // Add the product to the database (replace with your actual logic)
    const response = await addProductsWithComponent(productData);

    // Redirect to a success page
    res.send(response);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});


app.post("/UpdateCategoryName/:id", async (req, res) => {
  // Extract the product data from the request body
  const categoryData = req.body; // Ensure body-parser middleware is used

  try {


    // Add the product to the database (replace with your actual logic)
    const response = await updateCategoryName(categoryData);

    // Redirect to a success page
    res.send(response);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});

app.post("/UpdateSupplier/:id", async (req, res) => {
  // Extract the product data from the request body
  const supplierData = req.body; // Ensure body-parser middleware is used

  try {


    // Add the product to the database (replace with your actual logic)
    const response = await updateSupplier(supplierData);

    // Redirect to a success page
    res.send(response);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});

app.get("/SelectProductType", (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("User/Admin/Products/SelectProductType", {
    user: req.session.user
  });
});

app.get("/Add-Product-From-Supplier", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Categories = await getAllCategories();
  const Supplier = await getSuppliers();

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/AddProductsFromSupplier", {
    Supplier,
    Categories,
    user: req.session.user
  });
});

app.get("/ConfirmOrder/:supplierOrderId/:supplierId", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const supplierId = req.params.supplierId;
  const orderId = req.params.supplierOrderId; // Extracting the orderId from the route parameter

  const orderDetails = await getOrderDetails(orderId);


  // If the user session exists, render the confirm order page with the fetched data
  res.render("User/Admin/Orders/ConfirmOrder", {
    orderDetails, // Passing the fetched order details to the view
    supplierId,
    user: req.session.user
  });
});


app.get("/Add-Product-Made-In-Store", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Categories = await getAllCategories();
  const Supplier = await getSuppliers();
  const Components = await getAllComponents();
  // If the user session exists, render the add product page
  res.render("User/Admin/Products/AddProductsMadeInStore", {
    Supplier,
    Categories,
    Components,
    user: req.session.user
  });
});







class ProductOrder {
  constructor(data) {
      this.productId = data.productId;
      this.unitsOrdered = data.unitsOrdered;
      this.totalCostOfOrder = data.totalCostOfOrder;
      this.piecesPerUnit = data.piecesPerUnit;
      this.orderDate = new Date(data.orderDate);
      this.status = "Pending";  // default status
  }
}


class ItemOrder {
  constructor(data) {
      this.itemId = data.itemId;
      this.unitsOrdered = data.unitsOrdered;  // Note: Changed `unitsOrdered` to `unitsNeeded` based on your form, adjust if needed.
      this.totalCostOfOrder = data.totalCostOfOrder;
      this.piecesPerUnit = data.piecesPerUnit;
      this.orderDate = new Date(data.orderDate);
      this.status = "Pending";  // default status
  }
}


app.post('/submitProductOrder', async (req, res) => {
  const formData = new ProductOrder(req.body);

  const response = await orderProduct(formData);

console.log(response)
  if (response && response.orderedProductId) {
      // Return the result as JSON
      res.json(response);
  } else {
      res.status(400).send('There was an error placing the order.');
  }
});




app.post('/submitItemOrder', async (req, res) => {
  const formData = new ItemOrder(req.body);


  const response = await orderItem(formData);


  if (response && response.orderedItemId) {
      // Return the result as JSON
      res.json(response);
  } else {
      res.status(400).send('There was an error placing the order.');
  }
});


app.post('/submitSupplierOrder', async (req, res) => {


  const dto = req.body; // Taking req.body as is.

  try {
      const result = await orderFromSupplier(dto);
      res.json(result);
  } catch (error) {
      console.error("Error while processing:", error); // Logging the error for debugging.
      res.status(500).send(error.message);
  }
});

 

//--------SUPPLIERS
//GET THE SUPPLIERS
app.get("/Suppliers", async (req, res) => {
  try {
    const Suppliers = await getSuppliers();
    if (!req.session.user) {
      return res.redirect("/");
    }

    res.render("User/Admin/Supplier/Suppliers", {
      Suppliers,
      user: req.session.user
    });
  } catch (error) {
    console.error(error);
    res.status(500).send({ message: "Error fetching suppliers" });
  }
}); 


///--USERS

//GET ALL THE USERS FROM THE DATABASE
app.get("/users", async (req, res) => {
  const data = await getAllUsers();
  if (!req.session.user == null) {
    return res.redirect("/");
  }

  res.render("User/Admin/Users/ViewUsers",{data ,      user: req.session.user})

});

app.post('/ConfirmOrder', async (req, res) => {
    const dto = req.body;  // Get the data from the request body.
  if (!req.session.user == null) {
    return res.redirect("/");
  }
    try {
        const data = await ConfirmOrder(dto);
        res.json(data);  // Send back the response from the API.
    } catch (error) {
        res.status(500).json({ message: error.message });
    }
});



//Orders

app.get("/Orders", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }



  const Items = await getAllComponents();
  const Products = await getAllProducts();
  const Supplier = await getSuppliers();
  const Orders = await fetchAllOrders(); // Assuming fetchAllOrders is imported or defined in the same file

  res.render("User/Admin/Orders/ViewOrders", {
    Supplier,
    Items,
    Products,
    Orders, // Send all orders to the EJS template
    user: req.session.user,
  });
});
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------

// Start the server
const port = process.env.PORT || 3000;
app.listen(port, '0.0.0.0',() => {
  console.log(`Server started on port ${port}`);
});
