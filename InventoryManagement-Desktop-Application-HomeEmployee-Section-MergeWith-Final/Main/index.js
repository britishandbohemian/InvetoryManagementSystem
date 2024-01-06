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
  fetchOrderDetails,
  orderItem,
  createSale,
  fetchProfitStats,
  fetchAllSales,
  loginUser,
  orderComponent,
  registerUser,
  getComponent,
  orderProduct,
  getSuppliers,
  getSupplierById,
  addProducts,
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
  orderFromSupplier,
  createCategory,
  getUserById,
  fetchProductsThatNeedReordering,
  updateUser,
  getAllProducts,
  getProductById,
  fetchAllOrders,
  confirmOrder,
  addProductsWithComponent
} = require("./routes/ApplicationFunctions");
const { Console } = require("console");

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

  console.log(Inrole);
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

//ADMIN PAGE

app.get("/homeAdmin", async (req, res) => {
  //Redirect to Login Page
  if (!req.session.user) {
    return res.redirect("/");
  }

  //Variables Passed to admin Page
  const Statistics = await fetchProfitStats();
  const ProductsRunningLow = await fetchProductsThatNeedReordering();
  const ItemsRunningLow = await fetchItemsRunningLow();

  //render Page
  res.render("User/Admin/homeAdmin", {
    Statistics,
    ItemsRunningLow,
    ProductsRunningLow,
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

//----WEB APPLICATION TO SELL PRODUCTS
//GET POINT OF SALE SYSTEM
app.get("/PointOfSale", async (req, res) => {
  //const products = await getAllProducts();
  const products = await getAllProducts();

  if (!req.session.user) {
    return res.redirect("/");
  }

  res.render("User/SellItems", {
    products,
    user: req.session.user
  });
});

app.post("/MakeSale", async (req, res) => {
  try {
    const { userId, productsInSale, totalPrice, saleDate } = req.body;

    const saleData = {
      saleId: 0, // Assuming this is auto-generated or irrelevant here
      userId: userId || 1, // Use the userId from request or a default value
      productsInSale: productsInSale || [], // Use the productsInSale array from request or an empty array
      totalPrice: totalPrice || 0, // Use totalPrice from request or a default value
      saleDate: saleDate || new Date().toISOString() // Use saleDate from request or the current date/time
    };

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

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/ViewProducts", {
    products,
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

    console.log(Product);
    // Redirect to a success page
    res.send(Product);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});

app.post("/AddProductsMadeInStore", async (req, res) => {
  // Extract the product data from the request body
  const productData = req.body; // Ensure body-parser middleware is used

  try {
    console.log(productData);
    // Add the product to the database (replace with your actual logic)
    const response = await addProductsWithComponent(productData);

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

  // const Categories = await getAllCategories();
  //  const Supplier = await getSuppliers();
  const Categories = [
    {
      "categoryName": "Electronics",
      "categoryId": 1
    },
    {
      "categoryName": "Hardware",
      "categoryId": 2
    },
    {
      "categoryName": "Software",
      "categoryId": 3
    }
  ];

  const Supplier = [
    {
      "supplierId": 1,
      "supplierName": "TechCorp",
      "emailAddress": "techcorp@example.com",
      "phoneNumber": "1234567890",
      "contactPerson": "John Doe",
      "address": "123 Tech Street, TechCity",
      "supplier_Active": 1,
      "supplierOrders": null,
      "listOfProducts": [],
      "listOfComponents": [
        {
          "itemId": 1,
          "itemName": "Motherboard",
          "unitOfMeasurement": 1,
          "sellByDate": "2023-09-12T10:37:34.533",
          "lastUsedDate": null,
          "minimumThreshold": 10,
          "maximumThreshold": 50,
          "purchasePrice": 150,
          "status": 1,
          "productComponents": [],
          "supplierId": 1,
          "categoryId": 1,
          "packetsInInventory": 10,
          "piecesPerPacket": 5,
          "totalPiecesInInventory": 50,
          "pricePerPiece": 30
        },
        {
          "itemId": 2,
          "itemName": "Processor",
          "unitOfMeasurement": 1,
          "sellByDate": "2023-09-15T10:40:53.82",
          "lastUsedDate": null,
          "minimumThreshold": 5,
          "maximumThreshold": 30,
          "purchasePrice": 250,
          "status": 1,
          "productComponents": [],
          "supplierId": 1,
          "categoryId": 1,
          "packetsInInventory": 6,
          "piecesPerPacket": 4,
          "totalPiecesInInventory": 24,
          "pricePerPiece": 62.5
        }
      ],
      "rating": 9
    },
    {
      "supplierId": 2,
      "supplierName": "SoftTech",
      "emailAddress": "softtech@example.com",
      "phoneNumber": "0987654321",
      "contactPerson": "Jane Smith",
      "address": "456 Soft Avenue, SoftCity",
      "supplier_Active": 1,
      "supplierOrders": null,
      "listOfProducts": [],
      "listOfComponents": [
        {
          "itemId": 3,
          "itemName": "Software License",
          "unitOfMeasurement": 1,
          "sellByDate": "2023-10-01T10:37:34.533",
          "lastUsedDate": null,
          "minimumThreshold": 50,
          "maximumThreshold": 200,
          "purchasePrice": 100,
          "status": 1,
          "productComponents": [],
          "supplierId": 2,
          "categoryId": 3,
          "packetsInInventory": 50,
          "piecesPerPacket": 1,
          "totalPiecesInInventory": 50,
          "pricePerPiece": 100
        }
      ],
      "rating": 8
    }
  ];

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/AddProductsFromSupplier", {
    Supplier,
    Categories,
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

app.get("/ConfirmOrder/:orderId", async (req, res) => {
  if (!req.session.user) {
      return res.redirect("/");
  }

  const orderId = req.params.orderId; // Extract order ID from the route parameter
  const Suppliers = await getSuppliers();

  // If the user session exists, render the add product page
  res.render("User/Admin/Supplier/ConfirmOrder", {
      Suppliers,
      user: req.session.user,
      orderId  // Pass the orderId to the view
  });
});


app.get("/viewSales", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Sales = [];

  // If the user session exists, render the add product page
  res.render("User/Admin/Sales/Sales", {
    Sales,
    user: req.session.user
    // You can pass additional data to the template here
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
  console.log(response);

  if (response && response.orderedProductId) {
      // Return the result as JSON
      res.json(response);
  } else {
      res.status(400).send('There was an error placing the order.');
  }
});


//EMPLOYEE HOME PAGE

app.get("/homeEmployee", async (req, res) => {
  //Redirect to Login Page
  if (!req.session.user) {
    return res.redirect("/");
  }


  const ProductsRunningLow = await fetchProductsThatNeedReordering();
  const ItemsRunningLow = await fetchItemsRunningLow();


  //render Page
  res.render("User/Employee/HomeEmployee", {
    //Statistics,
    ItemsRunningLow,
    ProductsRunningLow,
    user: req.session.user
  });
});



app.post('/submitItemOrder', async (req, res) => {
  const formData = new ItemOrder(req.body);

  console.log(formData);
  const response = await orderItem(formData);
  console.log(response);

  if (response && response.orderedItemId) {
      // Return the result as JSON
      res.json(response);
  } else {
      res.status(400).send('There was an error placing the order.');
  }
});


app.post('/submitSupplierOrder', async (req, res) => {
  const dto = {
      totalCostOfOrder: req.body.totalCostOfOrder,
      leadTimeFromSupplier: req.body.leadTimeFromSupplier,
      supplierId: req.body.supplierId,
      expectedDeliveryDate: req.body.expectedDeliveryDate,
      placedByUserId: req.body.placedByUserId,
      orderedProductId: req.body.orderedProductId, // Adjusted this line
      orderedItemId: req.body.orderedItemId,       // Adjusted this line
  };

  try {
      const result = await orderFromSupplier(dto);
      res.json(result);
  } catch (error) {
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
