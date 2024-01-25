import React from 'react';
import OwlCarousel from 'react-owl-carousel';
import 'owl.carousel/dist/assets/owl.carousel.css';
import 'owl.carousel/dist/assets/owl.theme.default.css';
import { Link } from 'react-router-dom';


const ProductCategories = ({ data }) => {
    //Functions
    //Create a backend Function that will dynamically fetch five of the best categories in the store and place them here
    // Creates Product Slug
    const createSlug = (name, id) => {
        const slug = name.toLowerCase().replace(/ /g, '-') + '-' + id;
        return `/viewProduct/${slug}`;
    };
    return (
        <div>
            {data.map((category, index) => (
                <section key={index} className="section" id="Carousel">
                    <div className="container">
                        <div className="row">
                            <div className="col-lg-6">
                                <div className="section-heading">
                                    <h2>{category.name}</h2>
                                    <span>{category.ShortDescription}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="container">
                        <div className="row">
                            <div className="col-lg-12">
                                <OwlCarousel
                                    className="owl-theme"
                                    loop
                                    margin={10}
                                    nav
                                    dots={false}
                                    navText={[
                                        '<i class="fa fa-chevron-left" aria-hidden="true"></i>',
                                        '<i class="fa fa-chevron-right" aria-hidden="true"></i>'
                                    ]}
                                    responsive={{
                                        0: {
                                            items: 1
                                        },
                                        600: {
                                            items: 3
                                        },
                                        1000: {
                                            items: 4
                                        }
                                    }}
                                >
                                    {category.products.map(product => (
                                        <div key={product.id} className="item">
                                            <div className="thumb">
                                                <div className="hover-content">
                                                    <ul>
                                                        <li>
                                                            <Link to={createSlug(product.name, product.id)}>
                                                                <i className="fa fa-eye"></i>
                                                            </Link>
                                                        </li>
                                                        <li><a href="single-product.html"><i className="fa fa-star"></i></a></li>
                                                    </ul>
                                                </div>
                                                <img src={product.imageUrl} alt={product.name} />
                                            </div>
                                            <div className="down-content">
                                                <h4>{product.name}</h4>
                                                <ul className="stars">
                                                    <li><i className="fa fa-star"></i></li>
                                                    <li><i className="fa fa-star"></i></li>
                                                    <li><i className="fa fa-star"></i></li>
                                                    <li><i className="fa fa-star"></i></li>
                                                    <li><i className="fa fa-star"></i></li>
                                                </ul>
                                                <span>{product.price}</span>
                                            </div>
                                        </div>
                                    ))}
                                </OwlCarousel>
                            </div>
                        </div>
                    </div>
                </section>
            ))}
        </div>
    );
}

export default ProductCategories;
