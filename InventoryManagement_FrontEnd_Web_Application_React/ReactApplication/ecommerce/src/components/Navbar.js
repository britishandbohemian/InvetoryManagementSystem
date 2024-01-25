import React from 'react';
import { Link } from 'react-router-dom';




const Navbar = ({ categories }) => {

    //The State of my component goes up here

    return (
        <header class="header-area header-sticky">
            <div class="container">
                <div class="row">
                    <div class="col-12">
                        <nav class="main-nav">

                            <a href="index.html" class="logo">
                                <img width="20%" src="/assets/images/logo.png" />
                            </a>

                            <ul class="nav">
                                <li className="scroll-to-section"><a href="/" class="active">Home</a></li>


                                {/* Dynamically render categories based on their trending status */}
                                {categories.map((category, index) => (
                                    <li key={index} className="scroll-to-section">
                                        <Link to={`/category/${category.id}`}>{category.name}</Link>
                                    </li>
                                ))}
                                <li><a href="/products">All Products</a></li>
                                <li class="submenu">
                                    <a href="javascript:;">More</a>
                                    <ul>
                                        <li><a href="about.html">About Us</a></li>
                                        <li><a href="contact.html">Contact Us</a></li>
                                    </ul>
                                </li>
                                {/* <li class="submenu">
                                    <a href="javascript:;">Blog</a>
                                    <ul>
                                        <li><a href="#">Features Page 1</a></li>
                                        <li><a href="#">Features Page 2</a></li>
                                        <li><a href="#">Features Page 3</a></li>
                                    </ul>
                                </li> */}
                                <li class="scroll-to-section"><a href="#explore">Search</a></li>
                            </ul>
                            <a class='menu-trigger'>
                                <span>Menu</span>
                            </a>

                        </nav>
                    </div>
                </div>
            </div>
        </header>

    );
}


export default Navbar;