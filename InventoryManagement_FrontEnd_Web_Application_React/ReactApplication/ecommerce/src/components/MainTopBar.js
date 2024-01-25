import React from 'react';

//Create a backend Function that will dynamically fetch five of the best categories in the store and place them here pass them in through data
const MainTopBar = ({ data }) => {


    return (
        <section>
            <div className="main-banner" id="top">
                <div className="container-fluid">
                    <div className="row">
                        <div className="col-lg-6">
                            <div className="left-content">
                                <div className="thumb">
                                    <div className="inner-content">
                                        <h4>{data.heading}</h4>
                                        <span>{data.paragraph}</span>
                                        <div className="main-border-button">
                                            <a href={data.categoryLink + data.categoryId}>View More</a>
                                        </div>
                                    </div>
                                    <img src={data.leftBannerImage} alt="" />
                                </div>
                            </div>
                        </div>
                        <div className="col-lg-6">
                            <div className="right-content">
                                <div className="row">
                                    {data.categories.map((category, index) => (
                                        <div key={index} className="col-lg-6">
                                            <div className="right-first-image">
                                                <div className="thumb">
                                                    <div className="inner-content">
                                                        <h4>{category.name}</h4>
                                                        <span>{category.description}</span>
                                                    </div>
                                                    <div className="hover-content">
                                                        <div className="inner">
                                                            <h4>{category.name}</h4>
                                                            <p>{category.detail}</p>
                                                            <div className="main-border-button">
                                                                <a href={data.categoryLink + category.categoryId}>Discover More</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <img src={category.imageUrl} />
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default MainTopBar;
