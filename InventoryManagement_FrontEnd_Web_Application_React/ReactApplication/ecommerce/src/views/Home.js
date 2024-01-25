import React, { useEffect } from 'react';
import MainTopBar from '../components/MainTopBar';
import ProductCategories from '../components/ProductCategories';
import Footer from '../components/Footer';


//Data Imported From The Database
// Create A fetch Function To Fetch The Data From the database to Populate this section
const mainTopBarData = {
    heading: "Hello Im Dynamic ME",
    paragraph: "Your paragraph content.",
    categoryLink: "/category/",
    leftBannerImage: "assets/images/left-banner-image.jpg",
    categoryId: "1",
    // Maxium 4 Categories For the main Top Bar
    categories: [
        {
            name: "Category 1",
            description: "Category Description",
            detail: "Detailed description for Category 1",
            imageUrl: "assets/images/baner-right-image-01.jpg",
            categoryId: "1",
        },

        {
            name: "Category 2",
            description: "Category Description",
            detail: "Detailed description for Category 1",
            imageUrl: "assets/images/baner-right-image-01.jpg",
            categoryId: "2",
        },

        {
            name: "Category 3",
            description: "Category Description",
            detail: "Detailed description for Category 1",
            imageUrl: "assets/images/baner-right-image-01.jpg",
            categoryId: "3",
        },
        {
            name: "Category 4",
            description: "Category Description",
            detail: "Detailed description for Category 1",
            imageUrl: "assets/images/baner-right-image-01.jpg",
            categoryId: "4",
        },


        // ... add more categories as needed
    ]
};


// Home Page
const HomePage = ({ products, TrendingCategories }) => {

    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);
    return (

        <section>
            {/* 
        // Top Bar For Advertisement DEtermine the best Categories at the momemnt and place them up here with their images */}
            <MainTopBar data={mainTopBarData} />
            {/* // Determine the best 3 categories and place them Here */}
            <ProductCategories data={TrendingCategories} />
            {/* Footer */}
            <Footer />

        </section>

    )
}



export default HomePage;