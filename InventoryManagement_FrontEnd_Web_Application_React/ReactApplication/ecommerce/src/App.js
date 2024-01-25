import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

// Pages That Can Be Called
import AllProducts from './views/AllProducts';
import Home from './views/Home';
import Navbar from './components/Navbar';
import ViewProduct from './views/ViewProduct';
import { CartProvider } from './components/Cart';

function App() {

  //FUCNTION TO PULL ALL THE PRODUCTS
  // PULL THE DATA FROM THE DATABASE AND YOU CAN DYNAMICALLY LOAD ALL YOUR CONTENT
  //PASSS THE PRODUCTS TO THE PAGES IN THE APP
  // Pull All the products and pass them to the Pages that need them
  // const products = [
  //   {
  //     id: 1,
  //     name: "Classic Spring",
  //     price: "$120.00",
  //     description: "Description",
  //     qoute: "Qoute Text",
  //     imageUrl: "/assets/images/men-01.jpg",
  //     CategoryId: 1,
  //   },
  //   {
  //     id: 5,
  //     name: "HEllo Spring",
  //     price: "$120.00",
  //     description: "Description",
  //     qoute: "Qoute Text",
  //     imageUrl: "/assets/images/men-01.jpg",
  //     CategoryId: 1,
  //   },
  //   {
  //     id: 2,
  //     name: "Classic Spring",
  //     price: "$120.00",
  //     description: "Description",
  //     qoute: "Qoute Text",
  //     imageUrl: "/assets/images/men-01.jpg",
  //     CategoryId: 2,
  //   },
  //   {
  //     id: 3,
  //     name: "Classic Spring",
  //     price: "$120.00",
  //     description: "Description",
  //     qoute: "Qoute Text",
  //     imageUrl: "/assets/images/men-01.jpg",
  //     CategoryId: 3,
  //   },
  // ];

  var products = [];


  async function getAllProducts() {
    try {
      // Send GET request to the endpoint
      const response = await fetch("http://localhost:5358/Product");

      // Check if the response status is not in the 200-299 range (HTTP OK statuses)
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      // Parse the response body as JSON and wait for it to resolve
      const data = await response.json();
      return data;

    } catch (error) {
      console.error("There was a problem fetching the products:", error);
      throw error; // Re-throwing the error to handle it in the calling function if needed
    }
  }
  getAllProducts().then(data => {
    console.log(data); // Your JSON data here
    products = data;
  }).catch(error => {
    console.error(error);
  });


  const categories = [
    {
      id: 1,
      name: "Men",
    },
    {
      id: 2,
      name: "Woman",
    },
    {
      id: 3,
      name: "Kids",
    },

  ];

  //Function in the backend get the trewnding categories and all the products that are in that caetgory and store it here
  //A List Of the Trending categories with their respective products in that array
  const TrendingCategories = [
    {
      name: 'Men', id: 1, ShortDescription: "Lorem", products: [{
        id: 1,
        name: "Classic Spring",
        price: "$120.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "/assets/images/men-01.jpg",
        CategoryId: 1,
      }, {
        id: 2,
        name: "Nike Tee",
        price: "$120.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "assets/images/men-01.jpg",
        CategoryId: 1,
      },], link: "Link"
    },
    {
      name: 'Kids', id: 2, ShortDescription: "Lorem", products: [{
        id: 3,
        name: "Classic Spring Summer Tee",
        price: "$20.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "/assets/images/men-01.jpg",
        CategoryId: 2,
      }, {
        id: 4,
        name: "Spring Shirt",
        price: "$120.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "assets/images/men-01.jpg",
        CategoryId: 2,
      },], link: "Link"
    },
    {
      name: 'Woman', id: 3, ShortDescription: "Lorem", products: [{
        id: 5,
        name: "Classic Spring Womans Tee",
        price: "$120.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "/assets/images/men-01.jpg",
        CategoryId: 3,
      }, {
        id: 6,
        name: "Mini Skirts",
        price: "$120.00",
        description: "Description",
        qoute: "Qoute Text",
        imageUrl: "assets/images/men-01.jpg",
        CategoryId: 3,
      },], link: "Link"
    },

  ]

  return (
    // Router For Navigation Purposes this allows the ap to Go from Page To Page via the url
    <CartProvider>
      <Router>
        <Navbar categories={TrendingCategories} />
        <Routes>
          <Route path="/" element={<Home TrendingCategories={TrendingCategories} products={products} />} />
          <Route path="/products" element={<AllProducts products={products} />} />
          {/* I want the category id to be place in the url and only the product from that category should be passed int his variable help me make the js to do so */}
          <Route path="/category/:categoryId" element={<AllProducts products={products} categories={categories} />} />

          <Route path="/viewProduct/:slug" element={<ViewProduct products={products} />} />
        </Routes>
      </Router>
    </CartProvider>



  );
}

export default App;