import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useParams } from 'react-router-dom';

import Footer from '../components/Footer';


// Taking Products as a prop object THIS IS THE DATA OF ALL THE PRODUCTS WE HAVE
const AllProducts = ({ products, categories }) => {


    const params = useParams();

    let filteredProducts = products;
    let categoryName = "All Products"; // Default heading

    if ('categoryId' in params) {
        const { categoryId } = params;
        filteredProducts = products.filter(product => product.CategoryId.toString() === categoryId);

        // Find the category name
        const category = categories.find(cat => cat.id.toString() === categoryId);
        if (category) {
            categoryName = category.name;
        }
    }

    const createSlug = (name, id) => {
        const slug = name.toLowerCase().replace(/ /g, '-') + '-' + id;
        return `/viewProduct/${slug}`;
    };

    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    //PLACE TO VIEW ALL THE PRODUCTS
    return (


        <section>

            {/* Main Banner */}
            <div className="page-heading" id="top">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-12">
                            <div className="inner-content">
                                <h2>Header Text</h2>
                                <span>Text Below Header</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Products Section */}

            <section className="section" id="products">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-12">
                            <div class="section-heading">
                                <h2>{categoryName}</h2>
                                <span>Explore our range of products</span>
                            </div>
                        </div>
                    </div>
                    <div className="row">

                        <div>

                        </div>
                        {filteredProducts.map(product => ( // Use filteredProducts here
                            <div key={product.id} className="col-lg-4">
                                <div className="item">
                                    <div className="thumb">
                                        <div className="hover-content">
                                            <ul>
                                                <li><Link to={createSlug(product.name, product.id)}><i className="fa fa-eye"></i></Link></li>
                                                <li><a href="single-product.html"><i className="fa fa-star"></i></a></li>
                                                <li><a href="single-product.html"><i className="fa fa-shopping-cart"></i></a></li>
                                            </ul>
                                        </div>
                                        <img src={product.imageUrl} alt={product.name} />
                                    </div>
                                    <div className="down-content">
                                        <h4>{product.name}</h4>
                                        <span>{product.price}</span>
                                        <ul className="stars">
                                            <li><i className="fa fa-star"></i></li>
                                            <li><i className="fa fa-star"></i></li>
                                            <li><i className="fa fa-star"></i></li>
                                            <li><i className="fa fa-star"></i></li>
                                            <li><i className="fa fa-star"></i></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </section>




            <Footer />

        </section>
    );
}

export default AllProducts;
