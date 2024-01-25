import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import Footer from '../components/Footer';
import { useCart } from '../components/Cart';

const ViewProduct = ({ products }) => {
    // These must be called at the top any constant for the state
    // Slug From The Url Parameters
    const { slug } = useParams();
    const { addToCart } = useCart();


    // Extract productId from slug
    const productId = parseInt(slug.split('-').pop(), 10);

    // Find the product in the products array
    const product = products.find(p => p.id === productId);

    // State for quantity and total
    const [quantity, setQuantity] = useState(1);
    const [total, setTotal] = useState(0);

    // Scroll to top on component mount
    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    // Update total based on product price and quantity
    useEffect(() => {
        if (product) {
            const newTotal = (parseFloat(product.price.replace('$', '')) * quantity).toFixed(2);
            setTotal(`$${newTotal}`);
        }
    }, [quantity, product]);

    const updateQuantity = (newQuantity) => {
        if (newQuantity >= 1) {
            setQuantity(newQuantity);
        }
    };

    // If the product Is not Found return this
    if (!product) {
        return <div>Product not found</div>;
    }

    //This Will Allow me to Have a cart


    const handleAddToCart = () => {
        addToCart(product, quantity);
    };

    // Render Page
    return (
        <section>


            {/* <!-- ***** Main Banner Area Start ***** --> */}
            <div class="page-heading" id="top">
                <div class="container">
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="inner-content">
                                <h2>{product.name}</h2>
                                <span>{product.description}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            {/* <!-- ***** Main Banner Area End ***** --> */}

            {/* 
            <!-- ***** Product Area Starts ***** --> */}
            <section class="section" id="product">
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8">
                            <div class="left-images">
                                <img src={product.imageUrl} alt="" />
                            </div>
                        </div>
                        <div class="col-lg-4">
                            <div class="right-content">
                                <h4>{product.name}</h4>
                                <span class="price">{product.price}</span>
                                <ul class="stars">
                                    <li><i class="fa fa-star"></i></li>
                                    <li><i class="fa fa-star"></i></li>
                                    <li><i class="fa fa-star"></i></li>
                                    <li><i class="fa fa-star"></i></li>
                                    <li><i class="fa fa-star"></i></li>
                                </ul>
                                <span>{product.description}</span>
                                <div class="quote">
                                    <i class="fa fa-quote-left"></i><p>{product.qoute}</p>
                                </div>
                                <div className="quantity-content">
                                    <div className="left-content">
                                        <h6>No. of Orders</h6>
                                    </div>
                                    <div className="right-content">
                                        <div className="quantity buttons_added">
                                            <input type="button" value="-" className="minus" onClick={() => updateQuantity(quantity - 1)} />
                                            <input type="number" step="1" min="1" value={quantity} className="input-text qty text" onChange={(e) => updateQuantity(parseInt(e.target.value, 10))} />
                                            <input type="button" value="+" className="plus" onClick={() => updateQuantity(quantity + 1)} />
                                        </div>
                                    </div>
                                </div>
                                <div className="total">
                                    <h4>Total: {total}</h4>
                                    <div className="main-border-button">     <a href="#" onClick={handleAddToCart}>Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <Footer />

        </section >

    );

}

export default ViewProduct;