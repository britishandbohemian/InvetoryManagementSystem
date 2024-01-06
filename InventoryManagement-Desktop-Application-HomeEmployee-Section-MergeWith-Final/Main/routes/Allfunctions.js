
//--------------ROUTES ------------------------------

//----- FIRSTLY ADD PRODUCTS
//PAGE TO ADD PRODUCT

//Generic get Route For pages

//------PRODUCT ROUTES---------

app.get("/viewComponents", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Categories = await getAllCategories();
  const Supplier = await getSuppliers();
  const Components = await getAllComponents();

  // If the user session exists, render the add product page
  res.render("User/Admin/Items/ViewItems", {
    Components,
    Categories,
    Supplier,
    user: req.session.user
    // You can pass additional data to the template here
  });
});



app.get("/ViewProducts", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const products = await getAllProducts();

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/ViewProducts", {
    products,
    user: req.session.user
    // You can pass additional data to the template here
  });
});

app.get("/EditProduct/:productId", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  const productId = req.params.productId;

  const Categories = await getAllCategories();
  const product = await getProductById(productId);
  const Components = product.components; // Extract components from the product

  console.log(Components);

  res.render("User/Admin/Products/EditProduct", {
    product,
    Categories,
    Components, // Pass components to the template
    user: req.session.user
  });
});

app.get("/EditComponent/:componentId", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  const componentId = req.params.componentId; // extract the supplier ID from the route parameter

  const Component = await getComponent(componentId);

  // If the user session exists, render the add product page
  res.render("User/Admin/Items/EditItem", {
    Component,
    user: req.session.user
    // You can pass additional data to the template here
  });
});





app.post("/OrderProduct", async (req, res) => {
  // Extract the product data from the request body
  const productData = req.body; // Ensure body-parser middleware is used

  try {
    // Add the product to the database (replace with your actual logic)
    const response = await orderProduct(productData);

    // Redirect to a success page
    res.send(response);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});

app.post("/OrderComponent", async (req, res) => {
  // Extract the product data from the request body
  const ComponentData = req.body; // Ensure body-parser middleware is used

  try {
    // Add the product to the database (replace with your actual logic)
    const response = await orderComponent(ComponentData);

    // Redirect to a success page
    res.send(response);
  } catch (error) {
    // Handle error, such as rendering an error message or redirecting to an error page
    res.status(500).send(error.message);
  }
});

app.get("/ShowAddProductsMadeInStore", (req, res) => {
  const message = "";
  res.render("User/Admin/Products/AddProductsMadeInStore", {
    message: message,
    user: req.session.user
  });
});

app.post("/api/categories", async (req, res) => {
  try {
    // Here, you can extract the data from the request body
    const category = req.body;

    const CategoryDto = {
      categoryName: category.categoryName
    };

    console.log(CategoryDto);

    // Then, you can use your existing createCategory function or database logic to add the category
    const response = await createCategory(CategoryDto);

    // Respond with the newly created category, including its ID
    res.redirect("/GetCategories");
  } catch (error) {
    console.error("Error creating category:", error);
    res.status(500).send(error.message);
  }
});



//------PRODUCT ROUTES END---------

//SHOW  LOGIN

//--ROUTES TO SHOW PAGES
//HOME ADMIN SHOW
// Route to render the login page

//GET CUSTOMER ORDERS
app.get("/CustomerOrders", (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("CustomerOrders", { user: req.session.user });
});

app.get("/Orders", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Components = await getAllComponents();
  const Products = await getAllProducts();
  const Supplier = await getSuppliers();
  const Orders = await fetchAllOrders(); // Assuming fetchAllOrders is imported or defined in the same file

  res.render("User/Admin/Orders/ViewOrders", {
    Supplier,
    Components,
    Products,
    Orders, // Send all orders to the EJS template
    user: req.session.user
  });
});

app.post("/confirmOrder", async (req, res) => {
  const { orderId } = req.body; // Assuming orderId is sent in the request body

  try {
    const confirmationMessage = await confirmOrder(orderId); // Assuming confirmOrder is imported or defined in the same file
    res.status(200).json({ message: confirmationMessage });
  } catch (error) {
    res
      .status(500)
      .json({ message: `Failed to confirm order: ${error.message}` });
  }
});

//--------------ROUTES TO HANDLE USERS---------------

//-------------ROUTES USER END----------

//-------------ROUTES SUPLLIER----------
// ADD A SUPPLIER TO THE TABLE
// ADD A SUPPLIER TO THE TABLE

app.post("/Addsupplier", async (req, res) => {
  try {
    // req.body now contains the request payload, which should be the supplier information
    const supplierData = req.body;

    // create a supplier DTO
    const supplierDTO = {
      SupplierName: supplierData.supplierName,
      EmailAddress: supplierData.emailAddress,
      PhoneNumber: supplierData.phoneNumber,
      ContactPerson: supplierData.contactPerson,
      Address: supplierData.address,
      Rating: 1
    };

    // Insert the supplier data into the database using your database middleware

    const response = await addSupplier(supplierDTO);
    if (!req.session.user) {
      return res.redirect("/");
    }
    // Send the response back to the client
    res.redirect("back");
  } catch (error) {
    console.error("Error:", error);
    res
      .status(500)
      .json({ message: "There was an error processing your request." });
  }
});



//UPDATE A SUPPLIER DETAILS
app.post("/UpdateSupplier/:supplierId", async (req, res) => {
  try {
    const supplierId = req.params.supplierId; // extract the supplier ID from the route parameter

    // req.body now contains the request payload, which should be the updated supplier information
    const updatedSupplierData = req.body;

    // create a supplier DTO
    const UpdateSupplierDTO = {
      supplierName: updatedSupplierData.supplierName,
      emailAddress: updatedSupplierData.emailAddress,
      phoneNumber: updatedSupplierData.phoneNumber,
      contactPerson: updatedSupplierData.contactPerson,
      address: updatedSupplierData.address,
      rating: updatedSupplierData.rating,
      supplierId: supplierId
    };

    console.log(JSON.stringify(UpdateSupplierDTO) + "Worked index");
    // Update the supplier data in the database using your database middleware
    const response = await updateSupplier(supplierId, UpdateSupplierDTO); // assuming you have a function named 'updateSupplier' to handle the database update

    console.log(JSON.stringify(response));
    if (!req.session.user) {
      return res.redirect("/");
    }
    res.send(response);
  } catch (error) {
    console.error("Error:", error);
    res
      .status(500)
      .json({ message: "There was an error processing your request." });
  }
});

//SHOW INDIVIUAL SUPPLIER
app.get("/ViewSupplier/:supplierId", async (req, res) => {
  let id = req.params.supplierId; // Use 'supplierId' instead of 'id'
  const Supplier = await getSupplierById(id);
  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("User/Admin/Supplier/ViewSupplier.ejs", {
    Supplier,
    user: req.session.user
  });
});

//DELETE A SUPPLIER FROM THE TABLE
app.delete("/api/Supplier/:supplierId", async (req, res) => {
  const { supplierId } = req.params;
  try {
    await deleteSupplier(supplierId);
    res //THIS NOW SENDS A STATUS CODE TO THE PAGE TO SAY WHAT HAPPENDED IN THIS FUNCTION CRUCIAL PART OF THE CODE
      .status(200)
      .send({ success: true, message: "Supplier deleted successfully" });
  } catch (error) {
    console.error(error);
    res.status(500).send("Internal Server Error"); // or a more specific error status/code depending on the error
  }
});

//VIEW SPECIFIC SUPPLIER
//CAN VIEW ORDERS MADE TO SUPPLIER ORDER FROM THE SUPPLIER
app.get("/ViewSupplier/:id", async (req, res) => {
  const supplierId = req.params.id;

  try {
    const supplier = await getSupplier(supplierId);
    if (!req.session.user) {
      return res.redirect("/");
    }
    res.render("ViewSupplier", { supplier, user: req.session.user });
  } catch (error) {
    console.error(error);
    res.status(500).send({ message: "Error fetching supplier" });
  }
});

//----------ROUTES SUPPLIER END ----------------------

//-----USER ROUTES-------------------------------------
//------------------------------------------------------
app.post("/getUserByUsernameAndPassword", async (req, res) => {
  const { username, password } = req.body;
  const data = await getUserByUsernameAndPassword(username, password);
  res.json(data);
});

//Function Okay



//--------------- Import Functions From functions js
const {
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
  fetchComponentsThatNeedReordering,
  addSupplier,
  getAllCategories,
  deleteSupplier,
  updateSupplier,
  getAllUsers,
  deleteUser,
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
>>>>>>>> aa46c0dc31a7bd48ce718f13921846cc7f270dcc:InventoryManagement-Desktop-Application/Desktop_new/index.js

//FUNCTION TO LOGIN A USER
app.post("/api/auth/Login", async (req, res) => {
  const { username, userPassword } = req.body;
  try {
    const user = await loginUser(username, userPassword);

<<<<<<<< HEAD:InventoryManagement-Desktop-Application/Main/routes/Allfunctions.js
=======

  const success = req.query.success === "true";
  const error = req.query.error === "true";

  if (success) {
    // Render the page with a success message
    res.render("Admin/Users/ViewUsers", {
      data,
      user: req.session.user,
      success
    });
  } else if (error) {
    // Render the page with an error message
  } else {
    // Render the page normally
    res.render("User/Admin/Users/ViewUsers", { data, user: req.session.user });
  }
});

//GET ALL THE USERS BY ID
app.get("/getUserById/:id", async (req, res) => {
  const data = await getUserById(req.params.id);
  if (!req.session.user) {
    return res.redirect("/");
  }
  res.render("Admin/editUser", { data, user: req.session.user });
});

//UPDATE THE USER DATA
app.post("/updateUser/:id", async (req, res) => {
  const data = await updateUser(req.params.id, req.body);
  // res.status(200).send({ success: true });
  res.redirect("/users");
});

//DELETE THE USERS BY ID
app.delete("/deleteUser/:id", async (req, res) => {
  try {
    const data = await deleteUser(req.params.id, req.body);
    // Redirect with a success query parameter
    res.redirect("/users?success=true");
  } catch (error) {
    console.error(error);
    // Redirect with an error query parameter
    res.redirect("/users?error=true");
  }
});
//---------END USER ROUTE------------
//----
//----

>>>>>>> aa46c0dc31a7bd48ce718f13921846cc7f270dcc:InventoryManagement-Desktop-Application/Desktop_new/routes/Allfunctions.js
// ------------------ ORDERS ROUTE
//VIEW ORDERS
app.get("/ViewOrders", async (req, res) => {
  if (!req.session.user) {
    res.redirect("/");
    // Render the page with a success message
  }
  const success = req.query.success === "true";
  const error = req.query.error === "true";
  try {
    const orders = await getAllOrders();
    if (success) {
      res.render("User/Orders/ViewOrders", {
        user: req.session.user,
        orders,
        success
      });
      console.log("workeds");
    } else if (error) {
      // Render the page with an error message
      console.error(error);
      res.render("User/Orders/ViewOrders", {
        error: "An error occurred while fetching orders.",
        user: req.session.user
      });
    } else {
      // Render the page normally
      return res.render("ViewOrders", {
        orders,
        user: req.session.user
      });
<<<<<<< HEAD:InventoryManagement-Desktop-Application/Main/routes/Allfunctions.js
========
    if (!user) {
      // Check if user is not found
      const message = "Invalid credentials";
      return redirect("/Login", message); // Rendering login page with error message
>>>>>>>> aa46c0dc31a7bd48ce718f13921846cc7f270dcc:InventoryManagement-Desktop-Application/Desktop_new/index.js
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

<<<<<<<< HEAD:InventoryManagement-Desktop-Application/Main/routes/Allfunctions.js
//------END OF ORDERS ROUTE
========
//ADMIN PAGE

app.get("/homeAdmin", async (req, res) => {
  //Redirect to Login Page
  if (!req.session.user) {
    return res.redirect("/");
  }

  //Variables Passed to admin Page
  const Profit = await fetchProfitStats();
  const ProductsNeedReordering = await fetchProductsThatNeedReordering();
  const ComponentsNeedReordering = await fetchComponentsThatNeedReordering();
  //render Page
  res.render("User/Admin/homeAdmin", {
    Profit,
    ComponentsNeedReordering,
    ProductsNeedReordering,
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

app.get("/viewComponents", async (req, res) => {
  if (!req.session.user) {
    return res.redirect("/");
  }

  const Categories = await getAllCategories();
  const Supplier = await getSuppliers();
  const Components = await getAllComponents();

  // If the user session exists, render the add product page
  res.render("User/Admin/Items/ViewItems", {
    Components,
    Categories,
    Supplier,
    user: req.session.user
    // You can pass additional data to the template here
  });
});

//----WEB APPLICATION TO SELL PRODUCTS
//GET POINT OF SALE SYSTEM
app.get("/PointOfSale", async (req, res) => {
  //const products = await getAllProducts();
  const products = [
    {
      "productId": 1,
      "productName": "Laptop Pro",
      "categoryId": 1,
      "unitsInInventory": 50,
      "productSellingPrice": 1200,
      "productCostPrice": 1000,
      "prodcutMarkup": 10,
      "reorderLevel": 20,
      "status": 1,
      "sellByDate": "2023-12-31T00:00:00",
      "lastSoldDate": null,
      "supplierId": 1,
      "orderProducts": [],
      "productComponents": []
    },
    {
      "productId": 2,
      "productName": "Smartphone Ultra",
      "categoryId": 2,
      "unitsInInventory": 100,
      "productSellingPrice": 800,
      "productCostPrice": 600,
      "prodcutMarkup": 15,
      "reorderLevel": 30,
      "status": 1,
      "sellByDate": "2023-11-30T00:00:00",
      "lastSoldDate": null,
      "supplierId": 2,
      "orderProducts": [],
      "productComponents": []
    },
    {
      "productId": 3,
      "productName": "Wireless Headphones",
      "categoryId": 3,
      "unitsInInventory": 150,
      "productSellingPrice": 150,
      "productCostPrice": 100,
      "prodcutMarkup": 20,
      "reorderLevel": 40,
      "status": 1,
      "sellByDate": "2023-10-31T00:00:00",
      "lastSoldDate": null,
      "supplierId": 3,
      "orderProducts": [],
      "productComponents": []
    }
  ];

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
  const products = [];

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/ViewProducts", {
    products,
    user: req.session.user
    // You can pass additional data to the template here
  });
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
  const Categories = [];
  const Supplier = [];

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

  const Categories = [];
  const Supplier = [];
  const Components = [];

  // If the user session exists, render the add product page
  res.render("User/Admin/Products/AddProductsMadeInStore", {
    Supplier,
    Categories,
    Components,
    user: req.session.user
  });
});

//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------
//----------------------------------------------------------

// Start the server
const port = process.env.PORT || 4000;
app.listen(port, () => {
  console.log(`Server started on port ${port}`);
});
>>>>>>>> aa46c0dc31a7bd48ce718f13921846cc7f270dcc:InventoryManagement-Desktop-Application/Desktop_new/index.js
=======
    }
  } catch {}
});

//------END OF ORDERS ROUTE
>>>>>>> aa46c0dc31a7bd48ce718f13921846cc7f270dcc:InventoryManagement-Desktop-Application/Desktop_new/routes/Allfunctions.js
