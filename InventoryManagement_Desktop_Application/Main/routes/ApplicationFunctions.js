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
      return data;
    }
  } catch (error) {
    console.error("Login failed:", error.message);
    throw error;
  }
};


const BaseUrlRegister = "http://localhost:5358/api/Auth/RegisterUser";
const registerUser = async (
  username,
  userPassword,
  userEmail,
  userContact,
  role
) => {
  try {
    const resp = await fetch(BaseUrlRegister, {
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

const baseUrl = "http://localhost:5358/api/users"; // replace with your actual API address

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

const RevenueUrl = "http://0.0.0.0:5358/api/Stats/totalrevenue";
// Get Sales Revenue
async function SalesRevenue(RevenueUrl) {
  try {
    // Make a GET request to the API endpoint
    const response = await fetch();

    if (!response.ok) {
      throw new Error('Request failed with status ' + response.status);
    }

    // Parse the response as JSON
    const data = await response.json();


    return data
    // You can perform further actions with the data here

  } catch (error) {
    console.error('Error:', error);
    // Handle the error as needed
  }
}

// Get Potential Profit

async function Potentialprofit() {
  try {
    // Make a GET request to the API endpoint
    const response = await fetch('http://0.0.0.0:5358/api/Stats/potential-profit');

    if (!response.ok) {
      throw new Error('Request failed with status ' + response.status);
    }

    // Parse the response as JSON
    const data = await response.json();


    return data
    // You can perform further actions with the data here

  } catch (error) {
    console.error('Error:', error);
    // Handle the error as needed
  }
}

// Get the Stores Narrative for the day
async function StoreNarrative() {
  try {
    // Make a GET request to the API endpoint
    const response = await fetch('http://0.0.0.0:5358/api/Stats/StorePerformanceNarrative');

    if (!response.ok) {
      throw new Error('Request failed with status ' + response.status);
    }

    // Parse the response as JSON
    const data = await response.json();


    return data
    // You can perform further actions with the data here

  } catch (error) {
    console.error('Error:', error);
    // Handle the error as needed
  }
}


// Get the sales Data
async function getSalesData() {
  try {
    // Make a GET request to the API endpoint
    const response = await fetch('http://0.0.0.0:5358/api/sales/GetSales');

    if (!response.ok) {
      throw new Error('Request failed with status ' + response.status);
    }

    // Parse the response as JSON
    const data = await response.json();

    // Use the data (e.g., log it to the console)
    console.log(data);

    // You can perform further actions with the data here

  } catch (error) {
    console.error('Error:', error);
    // Handle the error as needed
  }
}




//------------USER ROUTES END-------------------

//
//

// ---------------SUPPLIER ROUTES----------

//ADD SUPPLIER
// Your existing addSupplier function
async function addSupplier(SupplierDto) {
  try {
    const response = await fetch("http://0.0.0.0:5358/api/Suppliers", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(SupplierDto)
    });

    if (!response.ok) {
      throw new Error("Request failed with status: " + response.status);
    }

    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error:", error);
    throw error;
  }
}



//Get Order Details From supplier
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


// Get All the suppliers
async function getSuppliers() {
  try {
    const response = await fetch("http://0.0.0.0:5358/api/Suppliers");

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const suppliersData = await response.json();

    return suppliersData;
  } catch (error) {
    console.error("There was an error fetching suppliers:", error);
  }
}


// Get Order Details
async function getOrderDetails(id) {
  try {
    const response = await fetch(`http://0.0.0.0:5358/api/SupplierOrders/supplier-order-details/${id}`);

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const orderDetails = await response.json();
    return orderDetails;
  } catch (error) {
    console.error("There was an error fetching order details:", error);
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
  } catch (error) { }
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
      return data;
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




// ADD PRODUCTS
async function addProducts(productsArray) {
  const apiUrl = "http://localhost:5358/Product/CreateProductsWithoutComponents"; // Replace with your actual API URL

  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(productsArray) // Pass an array of products
    });

    const jsonData = await response.json();

    if (response.ok && jsonData.status === 'success') {
      return { status: 'success', productIds: jsonData.productIds };
    } else {
      throw new Error(jsonData.message || `HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error adding products:", error);
    return { status: 'error', message: error.message };
  }
}

// GET SUPPLIER ORDERS
async function fetchSupplierOrderDetails(orderId) {
  const apiUrl = `http://0.0.0.0:5358/api/SupplierOrders/supplier-order/${orderId}/details`;
  try {
    const response = await fetch(apiUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json"
      }
    });

    const jsonData = await response.json();

    if (response.ok) {
      return { status: 'success', data: jsonData };
    } else {
      throw new Error(jsonData.message || `HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error fetching supplier order details:", error);
    return { status: 'error', message: error.message };
  }
}


// UPDATE CATEGORY NAME
function updateCategoryName(categoryUpdateDto) {
  const apiUrl = `http://0.0.0.0:5358/api/categories/${categoryUpdateDto.CategoryId}`;

  fetch(apiUrl, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(categoryUpdateDto)
  })
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then(data => {
      console.log("Category updated successfully:", data);
    })
    .catch(error => {
      console.error("Error updating category:", error);
    });
}


// uPDATE SUPPLIER DETAILS
//Test THis to see if it works
function updateSupplier(supplierDto) {
  const apiUrl = `http://localhost:5358/api/Suppliers/${supplierDto.supplierId}`;

  fetch(apiUrl, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(supplierDto)
  })
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then(data => {
      console.log("Supplier updated successfully:", data);
    })
    .catch(error => {
      console.error("Error updating category:", error);
    });
}







// ADD A PRODUCT LIKE AN EDIBLE MEAL
async function addProductsWithComponent(productsArray) {
  const apiUrl = "http://0.0.0.0:5358/Product/CreateProductsWithComponents"; // Replace with your actual API URL
  console.log(JSON.stringify(productsArray))

  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"

      },
      body: JSON.stringify(productsArray)
      // Pass an array of products
    });

    const jsonData = await response.json();

    if (response.ok && jsonData.status === 'success') {
      return { status: 'success', productIds: jsonData.productIds };
    } else {
      throw new Error(jsonData.message || `HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error adding products:", error);
    return { status: 'error', message: error.message };
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


    return products;
  } catch (error) {
    console.error("There was a problem fetching the products:", error);
    throw error; // Re-throwing the error to handle it in the calling function if needed
  }
}
//---------- END OF PRODUCT ROUTE----------

//----------ORDERS ROUTE-------
async function getAllOrders() {
  const response = await fetch("http://0.0.0.0:5358/api/SupplierOrders/supplier-order-details", {
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

// ORDER A PRODUCT FROM THE SUPPLIER
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

// FETCH PREDITED STOCK FOR THE DAY
async function fetchPredictedStockForToday() {
  try {
    const endpoint = `http://0.0.0.0:5358/api/Stats/predicted-stock-for-today`;

    const response = await fetch(endpoint, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Server responded with ${response.status}: ${response.statusText}`);
    }

    const result = await response.json();

    if (result && Array.isArray(result) && result.length > 0) {
      return result;  // Return the fetched predicted stocks
    } else {
      console.warn("Warning: Received empty data or null from the server.");
      return [];
    }
  } catch (error) {
    console.error("Fetch error:", error);
    return [];
  }
}


//FETCH SALES IN THE DATE RANGE
async function fetchSalesInDateRange(startDate, endDate) {
  try {
    // Assuming your backend server is running on the same IP and port 5358
    const url = `http://0.0.0.0:5358/api/sales/GetSales?startDate=${startDate}&endDate=${endDate}`;

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const result = await response.json();

    if (result && result.length) {
      return result;
    } else {
      console.error("Error: Received empty data or null from the server.");
      return null;
    }
  } catch (error) {
    console.error("Fetch error:", error);
    return null;
  }
}


async function fetchAllOrderProducts() {
  try {
    const response = await fetch('http://0.0.0.0:5358/api/SupplierOrders/order-products', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const result = await response.json();

    if (result && result.length) { // Check if data is present and it's an array with elements
      return result; // Return the fetched order products
    } else {
      console.error("Error: Received empty data or null from the server.");
      return null;
    }
  } catch (error) {
    console.error("Fetch error:", error);
    return null;
  }
}

async function fetchAllOrderItems() {
  try {
    const response = await fetch('http://0.0.0.0:5358/api/SupplierOrders/order-items', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const result = await response.json();

    if (result && result.length) { // Check if data is present and it's an array with elements
      return result; // Return the fetched order products
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

  console.log(dto)
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
      "http://0.0.0.0:5358/Items/CreateItem",
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
    const response = await fetch("http://0.0.0.0:5358/api/categories/categories-with-details");

    if (!response.ok) {
      const message = await response.text();
      throw new Error(`Error: ${message}`);
    }

    const categories = await response.json();
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

function ConfirmOrder(dto) {
  const url = 'http://0.0.0.0:5358/api/SupplierOrders/ConfirmOrder';  // Replace with the actual endpoint URL.

  console.log("Before Sent:", JSON.stringify(dto));
  fetch(url, {
    method: 'POST', // Assuming this is a POST request.
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(dto)
  })
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      return response.json();  // This line can be adjusted based on what the API endpoint returns.
    })
    .then(data => {
      console.log(data);  // Log the response data (if any).
    })
    .catch(error => {
      console.error('There was a problem with the fetch operation:', error.message);
    });
}
async function fetchAllOrders() {
  const response = await fetch(
    "http://0.0.0.0:5358/api/SupplierOrders/supplier-order-details"
  );

  if (response.ok) {
    const orders = await response.json();
    return orders;
  } else if (response.status === 404) {  // Handling the "Not Found" status
    return [];  // Return an empty array if no orders are found
  } else {
    throw new Error("Failed to fetch orders");
  }
}























// Base URL for the API
const apiBaseUrl = 'http://0.0.0.0:5358';

// Function to get total number of products
async function getTotalProducts() {
  const response = await fetch(`${apiBaseUrl}/Product/TotalProducts`);

  if (!response.ok) {
    throw new Error(`Failed to fetch total products: ${response.statusText}`);
  }

  const totalProducts = await response.json();
  return totalProducts;
}

// Function to get number of low stock products
async function getLowStockProducts() {
  const response = await fetch(`${apiBaseUrl}/Product/LowStockProducts`);

  if (!response.ok) {
    throw new Error(`Failed to fetch low stock products: ${response.statusText}`);
  }

  const lowStockProducts = await response.json();
  return lowStockProducts;
}

// Function to get number of overstocked products
async function getOverstockedProducts() {
  const response = await fetch(`${apiBaseUrl}/Product/OverstockedProducts`);

  if (!response.ok) {
    throw new Error(`Failed to fetch overstocked products: ${response.statusText}`);
  }

  const overstockedProducts = await response.json();
  return overstockedProducts;
}

// Function to get products near expiration
async function getProductsNearExpiration(daysThreshold) {
  const response = await fetch(`${apiBaseUrl}/Product/near-expiration?daysThreshold=${daysThreshold}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch products near expiration: ${response.statusText}`);
  }

  const products = await response.json();
  return products;
}

// Function to get top selling products
async function getTopSellingProducts(top) {
  const response = await fetch(`${apiBaseUrl}/Product/top-selling?top=${top}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch top selling products: ${response.statusText}`);
  }

  const topSelling = await response.json();
  return topSelling;
}

// Function to get least selling products
async function getLeastSellingProducts(bottom) {
  const response = await fetch(`${apiBaseUrl}/Product/least-selling?bottom=${bottom}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch least selling products: ${response.statusText}`);
  }

  const leastSelling = await response.json();
  return leastSelling;
}

// Function to get products by stock levels
async function getProductsByStockLevels(level) {
  const response = await fetch(`${apiBaseUrl}/Product/stock-levels?level=${level}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch products by stock levels: ${response.statusText}`);
  }

  const products = await response.json();
  return products;
}





async function getProductInformation(productId) {
  const response = await fetch(
    `http://0.0.0.0:5358/Product/(ProductInformation)?productId=${productId}`
  );

  if (response.ok) {
    const productInfo = await response.json();
    return productInfo;
  } else if (response.status === 404) {  // Handling the "Not Found" status
    return null;  // Return null if the product is not found
  } else {
    throw new Error("Failed to fetch product information");
  }
}





async function fetchBestSellingProducts() {
  const url = `http://0.0.0.0:5358/api/Stats/best-performing-products?topN=10`;

  const response = await fetch(url);
  if (response.ok) {
    const BestSellingProducts = await response.json();
    return BestSellingProducts;
  } else {
    throw new Error("Failed to fetch sales");
  }
}



async function fetchTotalProducts() {
  const url = `http://0.0.0.0:5358/api/Stats/total-products`;

  const response = await fetch(url);
  if (response.ok) {
    const res = await response.json();
    return res;
  } else {
    throw new Error("Failed to fetch sales");
  }
}

async function fetchTotalWorthProducts() {
  const url = `http://0.0.0.0:5358/api/Stats/products-worth`;

  const response = await fetch(url);
  if (response.ok) {
    const res = await response.json();
    return res;
  } else {
    throw new Error("Failed to fetch sales");
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

    return data;
    // Further processing here, e.g., updating the UI
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

async function fetchProfitStats() {
  const url = "http://0.0.0.0:5358/api/Stats/totalprofit";

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

async function recommendSales() {
  const url = "http://0.0.0.0:5358/api/Stats/recommend-for-sale";

  try {
    const response = await fetch(url);

    if (response.status !== 200) {
      console.error("Error fetching data:", response.status);
      // Return a valid JSON object with an error message
      return JSON.stringify({ error: "Error fetching data", status: response.status });
    }

    const data = await response.text();

    // Check if the data is not empty
    if (data) {
      return data;
    } else {
      // Return a valid JSON object indicating no data was found
      return JSON.stringify({ message: "No product recommended for sale." });
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    // Return a valid JSON object with the error message
    return JSON.stringify({ error: "Error fetching data", message: error.message });
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















async function getMostUsedItems(topN = 10) {
  const response = await fetch(`${apiBaseUrl}/Items/most-used-items?topN=${topN}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch most-used items: ${response.statusText}`);
  }

  const mostUsedItems = await response.json();
  return mostUsedItems;
}


async function generateItemPerformanceNarrative(itemId) {
  const response = await fetch(`${apiBaseUrl}/Items/GenerateItemPerformanceNarrative/${itemId}`);

  if (!response.ok) {
    throw new Error(`Failed to generate item performance narrative: ${response.statusText}`);
  }

  const narrative = await response.json();
  return narrative;
}


async function getAggregateItemMetrics() {
  const response = await fetch(`${apiBaseUrl}/Items/GetAggregateItemMetrics`);

  if (!response.ok) {
    throw new Error(`Failed to fetch aggregate item metrics: ${response.statusText}`);
  }

  const aggregateMetrics = await response.json();
  return aggregateMetrics;
}


async function getItemsByStockLevels(level) {
  const response = await fetch(`${apiBaseUrl}/Items/stock-levels?level=${level}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch items by stock levels: ${response.statusText}`);
  }

  const items = await response.json();
  return items;
}


async function getItemsExpiringSoon(days) {
  const response = await fetch(`${apiBaseUrl}/Items/items-expiring-soon?days=${days}`);

  if (!response.ok) {
    throw new Error(`Failed to fetch items expiring soon: ${response.statusText}`);
  }

  const items = await response.json();
  return items;
}


async function buildCategoryNarrative(categoryId) {
  const response = await fetch(`${apiBaseUrl}/api/categories/build-narrative/${categoryId}`);

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(`Failed to build category narrative: ${errorData.error}`);
  }

  const narrative = await response.json();
  return narrative;
}

async function filterCategoriesByWorthLevel(level) {
  try {
    const response = await fetch(`${apiBaseUrl}/api/categories/filter-by-worth-level?level=${level}`);

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(`Failed to filter categories by worth level: ${errorData.error}`);
    }

    const filteredCategories = await response.json();
    return filteredCategories;
  } catch (error) {
    throw new Error(`Error: ${error.message}`);
  }
}





//-------EXPORT ALL THE MODULES ---------

module.exports = {
  filterCategoriesByWorthLevel,
  buildCategoryNarrative,
  getMostUsedItems,
  generateItemPerformanceNarrative,
  getAggregateItemMetrics,
  getItemsExpiringSoon,
  getItemsByStockLevels,
  StoreNarrative,
  fetchAllOrderItems,
  fetchTotalProducts,
  fetchTotalWorthProducts,
  SalesRevenue,
  fetchPredictedStockForToday,
  getOrderDetails,
  fetchSupplierOrderDetails,
  fetchOrderDetails,
  createSale,
  fetchItemsRunningLow,
  fetchProfitStats,
  fetchProductsThatNeedReordering,
  getComponent,
  fetchAllOrders,
  recommendSales,
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
  updateCategoryName,
  getProductById,
  getProductInformation,
  SalesRevenue,
  orderComponent,
  getSalesData,
  ConfirmOrder,
  Potentialprofit,
  fetchAllOrderProducts,
  fetchSalesInDateRange,
  fetchBestSellingProducts,
  getProductsByStockLevels,
  getLeastSellingProducts,
  getTopSellingProducts,
  getProductsNearExpiration,
  getOverstockedProducts,
  getLowStockProducts,
  getTotalProducts
};
