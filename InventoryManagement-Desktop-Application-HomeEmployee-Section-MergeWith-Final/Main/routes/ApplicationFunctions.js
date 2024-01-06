const axios = require("axios");

//-----------USER ROUTES--------------------
const BaseUrlAuth = "http://0.0.0.0:5358/api/Auth/LoginUser";

//LOG USER IN
const loginUser = async (username, userPassword) => {
  try {
    //First Create the function
    //insert the url
    //define the method
    //define how you want the info
    //send the data in body
    const resp = await fetch(BaseUrlAuth, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ username, userPassword })
    });

    if (resp.status === 200) {
      const data = await resp.json();
      console.log("Login successful:", data);
      return data;
    }
  } catch (error) {
    console.error("Login failed:", error.message);
    throw error;
  }
};

const registerUser = async (
  username,
  userPassword,
  userEmail,
  userContact,
  role
) => {
  try {
    const resp = await fetch("http://0.0.0.0:5358/api/Auth/RegisterUser", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        username,
        userPassword,
        userEmail,
        userContact,
        role
      })
    });

    if (resp.ok) {
      const data = await resp.json();
      const token = data.token;
      console.log("User registered successfully. Token:", token);
      return data; // Returning the data including the user information
    } else {
      const error = await resp.json();
      console.error("User registration failed:", error.message);
      throw new Error(error.message);
    }
  } catch (error) {
    console.error("User registration failed:", error.message);
    throw error;
  }
};

const baseUrl = "http://0.0.0.0:5358/api/users"; // replace with your actual API address

// GET ALL THE USERS
async function getAllUsers() {
  const response = await fetch(baseUrl);
  return response.json();
}

// GET USER BY ID
async function getUserById(id) {
  const response = await fetch(`${baseUrl}/${id}`);
  return response.json();
}

// UPDATE THE USER
async function updateUser(id, userData) {
  const response = await fetch(`${baseUrl}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(userData)
  });

  return response;
}

// DELETE A USER
async function deleteUser(id) {
  const response = await fetch(`${baseUrl}/${id}`, {
    method: "DELETE"
  });
  return response;
}

//------------USER ROUTES END-------------------

//
//

// ---------------SUPPLIER ROUTES----------

//ADD SUPPLIER

const addSupplier = async (SupplierDto) => {
  // ...
  return fetch("http://0.0.0.0:5358/api/Suppliers", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(SupplierDto)
  })
    .then((response) => {
      return response.json();
    })
    .then((data) => {
      console.log(data);
      console.log("worked");
      return data; // Returning the data to the caller
    })
    .catch((error) => {
      console.error("Error:", error);
      throw error; // Propagating the error to the caller
    });
};

async function fetchOrderDetails(orderId) {
  const endpoint = `http://0.0.0.0:5358/api/SupplierOrders/supplier-order/${orderId}/details`;

  try {
      const response = await fetch(endpoint);
      if (response.ok) {
          const data = await response.json();
          return data;
      } else {
          console.error("Failed to fetch data: ", response.status, response.statusText);
          return null;
      }
  } catch (error) {
      console.error("Error fetching data:", error);
      return null;
  }
}



async function getSuppliers() {
  try {
    const response = await fetch("http://0.0.0.0:5358/api/Suppliers");

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const suppliersData = await response.json();

    return suppliersData.map((supplier) => ({
      supplierId: supplier.supplierId,
      supplierName: supplier.supplierName,
      emailAddress: supplier.emailAddress,
      phoneNumber: supplier.phoneNumber,
      contactPerson: supplier.contactPerson,
      address: {
        street: supplier.street,
        city: supplier.city,
        state: supplier.state,
        postalCode: supplier.postalCode,
      },
      status: supplier.status,
      rating: supplier.rating
    }));
  } catch (error) {
    console.error("There was an error fetching suppliers:", error);
  }
}

//GET A SUPPLIER BY ID
async function getSupplierById(id) {
  try {
    const response = await fetch(`http://0.0.0.0:5358/api/Suppliers/${id}`);

    if (!response.ok) {
      const errorDetails = await response.text(); // or response.json() if the error details are in JSON format
      console.error("Error details:", errorDetails);
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const supplier = await response.json();

    return {
      supplierId: supplier.supplierId,
      supplierName: supplier.supplierName,
      emailAddress: supplier.emailAddress,
      phoneNumber: supplier.phoneNumber,
      contactPerson: supplier.contactPerson,
      address: supplier.address,
      supplier_Active: supplier.supplier_Active,
      supplierOrders: supplier.supplierOrders,
      listOfProducts: supplier.listOfProducts,
      listOfComponents: supplier.listOfComponents.map((component) => ({
        componentId: component.componentId,
        componentName: component.componentName,
        componentDescription: component.componentDescription,
        itemUnits: component.itemUnits,
        unitOfMeasurement: component.unitOfMeasurement,
        sellByDate: component.sellByDate,
        minimumThreshold: component.minimumThreshold,
        maximumThreshold: component.maximumThreshold,
        sellingPrice: component.sellingPrice,
        purchasePrice: component.purchasePrice,
        status: component.status,
        productComponents: component.productComponents,
        supplierId: component.supplierId
      })),
      rating: supplier.rating
    };
  } catch (error) {}
}


async function GetProfits() {
  const response = await fetch(`http://0.0.0.0:5358/api/Stats/TotalProfit`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(ProductToOrder)
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  } else {
    return "product ordered";
  }
}

//ORDER A COMPONENT
async function orderComponent(ComponentToOrder) {
  const response = await fetch(
    `http://0.0.0.0:5358/api/SupplierOrders/orderComponents`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(ComponentToOrder)
    }
  );

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  } else {
    return "product ordered";
  }
}

//DELETE PRODUCT SUPPLIER OFFERS
async function deleteProductFromSupplier(supplierId, productId) {
  try {
    // Setting up the request options for a DELETE request
    const requestOptions = {
      method: "DELETE" // Specifying the HTTP method as DELETE
    };

    // Making the fetch request to the specified URL with the supplier and product IDs
    const response = await fetch(
      `http://localhost:5358/api/Supplier/${supplierId}/products/${productId}`,
      requestOptions
    );

    // Checking the response status; if it's not 204 (No Content), an error is thrown
    if (response.status !== 204) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    // Logging a success message
    console.log("Product deleted successfully from supplier");
    return response;
  } catch (error) {
    // Logging any errors that occur during the fetch
    console.error("Error during product deletion:", error.message);
    throw error;
  }
}

//DELETE PRODUCT SUPPLIER OFFERS
async function deleteSupplier(supplierId) {
  try {
    const response = await fetch(
      `http://0.0.0.0:5358/api/Supplier/${supplierId}`
    );

    if (response.status !== 204) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    console.log("supplier deleted successfully from database ");
    return response;
  } catch (error) {
    console.error(
      "Error during supplier deleted :",
      error.response?.data || error.message
    );
    throw error;
  }
}

//UPDATE SUPPLIER
async function updateSupplier(supplierId, updatedSupplier) {
  const url = `http://0.0.0.0:5358/api/Suppliers/${supplierId}`;

  fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(updatedSupplier)
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      return response.json();
    })
    .then((data) => {
      console.log("Supplier updated successfully:", data);
      // handle successful response
    })
    .catch((error) => {
      console.error("Fetch error:", error);
      // handle errors
    });
}

//DELETE SUPPLIER ORDER
function deleteSupplierOrder(orderId) {
  const endpoint = `http://0.0.0.0:5358/api/Supplier/orders/${orderId}`;

  fetch(endpoint, {
    method: "DELETE"
  })
    .then((response) => {
      if (response.status === 204) {
        // No content, successful deletion
        console.log("Order deleted successfully");
      } else {
        console.error("Error deleting the order");
        return response.text().then((text) => {
          throw new Error(text);
        });
      }
    })
    .catch((error) => console.error("Error:", error));
}

//--------------PRODUCTS ROUTE-------------
//EDIT PRODUCT INFORMATION
async function editProduct(productId, productData) {
  try {
    const response = await fetch(
      `http://0.0.0.0:5358/api/Products/${productId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(productData)
      }
    );

    const data = await response.json();

    if (data && data.message) {
      console.log("Product updated successfully.");
      return data.message; // Assuming the server responds with a message property
    } else {
      console.error("An error occurred while updating the product.");
      return "An error occurred while updating the product.";
    }
  } catch (error) {
    console.error("Failed to update product:", error);
    return "Failed to update product.";
  }
}

//GET PRODUCT BY ID
async function getProductById(productId) {
  try {
    const response = await fetch(
      `http://0.0.0.0:5358/GetProductWith${productId}`
    );

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const product = await response.json(); // Parse the response as JSON

    // Handle the retrieved product here
    console.log(product);
    return product;
  } catch (error) {
    console.error(`Failed to fetch product: ${error}`);
    // Optionally show an error message to the user
    return null; // Or handle the error as you see fit
  }
}

//DELETE PRODUCT
async function deleteProduct(productId) {
  await axios
    .delete(`http://.0.0.0.0:5358/api/Products/${productId}`)
    .then((response) => {
      if (response.status === "") {
        console.log("Product deleted successfully.");
      } else {
        console.error("An error occurred while deleting the product.");
      }
    })
    .catch((error) => {
      console.error("Failed to delete product:", error);
    });
}

// Add Products
async function addProducts(productsArray) {
  const apiUrl = "http://0.0.0.0:5358/CreateProductsWithoutComponents"; // Replace with your actual API URL
  console.log(productsArray + "hey");
  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
        // Add any other headers, like authentication tokens, if required
      },
      body: JSON.stringify(productsArray) // Pass an array of products
    });

    if (response.ok) {
      let r = [];
      r = await response.json(); // If you want to return JSON from your endpoint
      console.log("Products added successfully:", r);
      return result;
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error adding products:", error);
  }
}

async function addProductsWithComponent(productsArray) {
  const apiUrl = "http://0.0.0.0:5358/CreateProductWithComponents"; // Replace with your actual API URL

  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
        // Add any other headers, like authentication tokens, if required
      },
      body: JSON.stringify(productsArray) // Pass an array of products
    });

    if (response.ok) {
      const result = await response.json(); // If you want to return JSON from your endpoint
      console.log("Products added successfully:", result);
      return result;
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error adding products:", error);
  }
}

//GET ALL THE PRODUCTS
async function getAllProducts() {
  try {
    // Send GET request to the endpoint
    const response = await fetch(
      "http://0.0.0.0:5358/Product"
    );

    // Check if the response status is not in the 200-299 range (HTTP OK statuses)
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    // Parse the response body as JSON
    const products = await response.json();
    console.log(products + "Functiosn");

    return products;
  } catch (error) {
    console.error("There was a problem fetching the products:", error);
    throw error; // Re-throwing the error to handle it in the calling function if needed
  }
}
//---------- END OF PRODUCT ROUTE----------

//----------ORDERS ROUTE-------
async function getAllOrders() {
  const response = await fetch("http://0.0.0.0:5358/api/SupplierOrders/all-supplier-orders", {
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  });

  if (!response.ok) {
    console.error(`HTTP error! status: ${response.status}`);
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  const orders = await response.json();
  console.log("Orders fetched from API:", orders);
  return orders;
}

async function orderProduct(data) {
  try {
      const response = await fetch('http://0.0.0.0:5358/api/SupplierOrders/order-product', {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json',
          },
          body: JSON.stringify(data)
      });

      const result = await response.json();
      
      if (result) { // Simply checking for the presence of data
          return result; // Return the result on successful submission
      } else {
          console.error("Error: Received empty data or null from the server.");
          return null;
      }
  } catch (error) {
      console.error("Fetch error:", error);
      return null;
  }
}


async function orderFromSupplier(dto) {
  const response = await fetch('http://0.0.0.0:5358/api/SupplierOrders/create-supplier-order', {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json'
      },
      body: JSON.stringify(dto)
  });

  if (response.ok) {
      return await response.json();
  } else {
      const errorData = await response.text();
      throw new Error(`Failed to order from supplier: ${errorData}`);
  }
}
async function orderItem(data) {
  try {
      const response = await fetch('http://0.0.0.0:5358/api/SupplierOrders/order-item', {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json',
          },
          body: JSON.stringify(data)
      });

      const result = await response.json();
      
      if (result) { // Simply checking for the presence of data
          return result; // Return the result on successful submission
      } else {
          console.error("Error: Received empty data or null from the server.");
          return null;
      }
  } catch (error) {
      console.error("Fetch error:", error);
      return null;
  }
}


//------END ORDERS ROUTE-------------------

//---------COMPONENT ROUTES---------------

//Checked Is Working
async function createComponent(ComponentDto) {
  try {
    const response = await fetch(
      "http://0.0.0.0:5358/Components/CreateAComponent",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(ComponentDto)
      }
    );

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const component = await response.json();
    console.log("Component created successfully:", component);
    return component;
  } catch (error) {
    console.error("Error creating component:", error);
  }
}

async function getAllComponents() {
  try {
    const response = await fetch(
      "http://0.0.0.0:5358/Items/GetAllItems"
    );

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const components = await response.json();
    console.log("Components retrieved:", components);
    return components;
  } catch (error) {
    console.error("Error retrieving components:", error);
  }
}

async function getComponent(id) {
  try {
    const response = await fetch(
      `http://0.0.0.0:5358/Components/GetComponentWithId/${id}`
    );

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const component = await response.json();
    console.log("Component retrieved:", component);
    return component;
  } catch (error) {
    console.error("Error retrieving component:", error);
  }
}

async function updateComponent(id, componentDto) {
  try {
    const response = await fetch(
      `http://0.0.0.0:5358/Components/UpdateComponentwith/${id}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(componentDto)
      }
    );

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    console.log("Component updated successfully");
  } catch (error) {
    console.error("Error updating component:", error);
  }
}

async function deleteComponent(id) {
  try {
    const response = await fetch(
      `/Components/Delete%20Product%20With%20${id}`,
      {
        method: "DELETE"
      }
    );

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    console.log("Component deleted successfully");
  } catch (error) {
    console.error("Error deleting component:", error);
  }
}

//---------COMPONENT ROUTES END---------------

//--------CATEGORIES ROUTE---------------

async function getAllCategories() {
  try {
    const response = await fetch("http://0.0.0.0:5358/api/categories");

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const categories = await response.json();
    console.log("Categories retrieved:", categories);
    return categories;
  } catch (error) {
    console.error("Error retrieving categories:", error);
  }
}

async function getCategory(id) {
  try {
    const response = await fetch(`/api/categories/${id}`);

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const category = await response.json();
    console.log("Category retrieved:", category);
  } catch (error) {
    console.error("Error retrieving category:", error);
  }
}

async function createCategory(addCategoryDto) {
  try {
    const response = await fetch("http://0.0.0.0:5358/api/categories", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(addCategoryDto)
    });

    const category = await response.json();

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    console.log("Category created successfully:", category);

    return category;
  } catch (error) {
    console.error("Error creating category:", error);
  }
}

async function deleteCategory(id) {
  try {
    const response = await fetch(`/api/categories/${id}`, {
      method: "DELETE"
    });

    if (!response.ok) {
      const message = await response.text();
    }

    console.log("Category deleted successfully");
  } catch (error) {
    console.error("Error deleting category:", error);
  }
}

//--------CATEGORIES ROUTE END---------------

//--------POS SYSTEM -----------

async function sellItems(cartData) {
  const apiUrl = "http://localhost:5358/api/Products"; // Replace with your actual API URL
  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
        // Add any other headers, like authentication tokens, if required
      },
      body: JSON.stringify(cartData)
    });

    if (response.ok) {
      const data = await response.json(); // If you want to return JSON from your endpoint
      return data; // Return the data to the caller
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error selling items:", error);
    // Handle the error as needed, possibly by returning an error code or message to the caller
  }
}

async function fetchAllOrders() {
  const response = await fetch(
    "http://0.0.0.0:5358/api/SupplierOrders/all-supplier-orders "
  );
  if (response.ok) {
    const orders = await response.json();
    return orders;
  } else {
    throw new Error("Failed to fetch orders");
  }
}

async function fetchAllSales() {
  const response = await fetch("http://0.0.0.0:5358/api/Sales/GetAllSales");
  if (response.ok) {
    const orders = await response.json();
    return orders;
  } else {
    throw new Error("Failed to fetch orders");
  }
}

async function confirmOrder(orderId) {
  const response = await fetch(
    `http://0.0.0.0:5358/api/SupplierOrders/confirmOrderArrival/${orderId}`,
    {
      method: "PUT"
    }
  );

  if (response.ok) {
    return `Order ${orderId} has been confirmed.`;
  } else {
    throw new Error(`Failed to confirm order ${orderId}.`);
  }
}

async function fetchProductsThatNeedReordering() {
  const url = "http://0.0.0.0:5358/api/Stats/products-running-low";

  try {
    const response = await fetch(url);

    if (response.status !== 200) {
      console.error("Error fetching data:", response.status);
      return;
    }

    const data = await response.json();
    console.log("Products that need reordering:", data);
    return data;
    // Further processing here, e.g., updating the UI
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

async function fetchItemsRunningLow() {
  const url = "http://0.0.0.0:5358/api/Stats/items-running-low";

  try {
    const response = await fetch(url);

    if (response.status !== 200) {
      console.error("Error fetching data:", response.status);
      return;
    }

    const data = await response.json();
    console.log("Components that need reordering:", data);
    return data;
    // Further processing here, e.g., updating the UI
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

async function fetchProfitStats() {
  const url = "http://0.0.0.0:5358/api/Stats/TotalProfit";

  try {
    const response = await fetch(url);

    if (response.status !== 200) {
      console.error("Error fetching data:", response.status);
      return;
    }

    const data = await response.json();
    return data;
    // Further processing here, e.g., updating the UI
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

async function createSale(data) {
  const response = await fetch("http://0.0.0.0:5358/api/Sales/CreateSale", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  });

  if (response.ok) {
    const result = await response.json();
    return result;
  } else {
    const message = await response.json();
    throw new Error(message);
  }
}

//-------EXPORT ALL THE MODULES ---------

module.exports = {
  fetchOrderDetails,
  createSale,
  fetchItemsRunningLow,
  fetchProfitStats,
  fetchProductsThatNeedReordering,
  getComponent,
  fetchAllOrders,
  confirmOrder,
  getAllComponents,
  getAllCategories,
  addProducts,
  addProductsWithComponent,
  loginUser,
  registerUser,
  getSuppliers,
  getSupplierById,
  orderItem,
  orderProduct,
  getAllOrders,
  addSupplier,
  deleteProductFromSupplier,
  deleteSupplier,
  updateSupplier,
  orderFromSupplier,
  createCategory,
  getAllUsers,
  createComponent,
  deleteUser,
  getUserById,
  updateUser,
  deleteSupplierOrder,
  getAllProducts,
  editProduct,
  deleteProduct,
  getProductById,
  fetchAllSales,
  orderComponent
};
